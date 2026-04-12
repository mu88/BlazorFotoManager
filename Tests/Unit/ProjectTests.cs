using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Unit;

[Category("Unit")]
public class ProjectTests
{
    private readonly IFileHandler _fileHandler = Substitute.For<IFileHandler>();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    [TestCase(2, 1, 1)]
    [TestCase(2, 2, 1)]
    public void NextImage(int numberOfImages, int numberOfNextCalls, int expectedImageIndex)
    {
        // Arrange
        var imageFilePaths = new Collection<string>();
        for (var i = 0; i < numberOfImages; i++)
        {
            imageFilePaths.Add($"Image {i}");
        }

        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);

        // Act
        for (var i = 0; i < numberOfNextCalls; i++)
        {
            testee.NextImage();
        }

        // Assert
        testee.CurrentImageIndex.Should().Be(expectedImageIndex);
    }

    [TestCase(2, 1, 0)]
    [TestCase(2, 2, 0)]
    [TestCase(3, 1, 1)]
    public void PreviousImage(int numberOfImages, int numberOfPreviousCalls, int expectedImageIndex)
    {
        // Arrange
        var imageFilePaths = new Collection<string>();
        for (var i = 0; i < numberOfImages; i++)
        {
            imageFilePaths.Add($"Image {i}");
        }

        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);
        for (var i = 0; i < numberOfImages; i++)
        {
            testee.NextImage();
        }

        // Act
        for (var i = 0; i < numberOfPreviousCalls; i++)
        {
            testee.PreviousImage();
        }

        // Assert
        testee.CurrentImageIndex.Should().Be(expectedImageIndex);
    }

    [Test]
    public void SumOfCopies_WithMultipleImages_ReturnsSumNotMax()
    {
        // Arrange
        var testee = CreateTestee();
        testee.AddImages(["Path1.jpg", "Path2.jpg"]);
        testee.CurrentImage!.Increase();
        testee.CurrentImage!.Increase(); // Path1: 2 copies
        testee.NextImage();
        testee.CurrentImage!.Increase();
        testee.CurrentImage!.Increase();
        testee.CurrentImage!.Increase(); // Path2: 3 copies

        // Act
        var result = testee.SumOfCopies;

        // Assert
        result.Should().Be(5); // Sum(2,3)=5, NOT Max(2,3)=3
    }

    [Test]
    public void Export_WithTwoCopies_CallsCopyExactlyTwice()
    {
        // Arrange
        var imageFilePaths = new Collection<string> { Path.Combine("input", "Path1.jpg") };
        var exportPath = "output";
        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);
        testee.CurrentImage!.Increase();
        testee.CurrentImage!.Increase(); // 2 copies

        // Act
        testee.ExportImages(exportPath, _ => { });

        // Assert
        _fileSystem.Received(1).Copy(Path.Combine("input", "Path1.jpg"), Path.Combine(exportPath, "Path1_0.jpg"), true);
        _fileSystem.Received(1).Copy(Path.Combine("input", "Path1.jpg"), Path.Combine(exportPath, "Path1_1.jpg"), true);
        _fileSystem.DidNotReceive().Copy(Path.Combine("input", "Path1.jpg"), Path.Combine(exportPath, "Path1_2.jpg"), true);
    }

    [Test]
    public void Export_WithMultipleCopies_ReportsProgressCorrectly()
    {
        // Arrange
        var imageFilePaths = new Collection<string> { Path.Combine("input", "Path1.jpg") };
        var progressValues = new System.Collections.Generic.List<double>();
        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);
        testee.CurrentImage!.Increase();
        testee.CurrentImage!.Increase(); // 2 copies → SumOfCopies=2

        // Act
        testee.ExportImages("output", progress => progressValues.Add(progress));

        // Assert
        progressValues.Should().HaveCount(2);
        progressValues[0].Should().BeApproximately(0.5, 0.001); // 1/2
        progressValues[1].Should().BeApproximately(1.0, 0.001); // 2/2
    }

    [Test]
    public void AddImages()
    {
        // Arrange
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var testee = CreateTestee();

        // Act
        testee.AddImages(imageFilePaths);

        // Assert
        testee.NumberOfImages.Should().Be(2);
    }

    [Test]
    public void Export()
    {
        // Arrange
        var imageFilePaths = new Collection<string> { Path.Combine("input", "Path1.jpg"), Path.Combine("input", "Path2.jpg") };
        var exportPath = "output";
        var progressActionMock = Substitute.For<System.Action<double>>();
        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);
        testee.CurrentImage!.Increase();

        // Act
        testee.ExportImages(exportPath, progressActionMock);

        // Assert
        _fileSystem.Received(1).Copy(Path.Combine("input", "Path1.jpg"), Path.Combine(exportPath, "Path1_0.jpg"), true);
        progressActionMock.Received(1).Invoke(1);
    }

    [Test]
    public async Task Load()
    {
        // Arrange
        var projectFilePath = "MyProjectPath";
        _fileHandler.ReadAsync<ProjectDto>(projectFilePath, Arg.Any<CancellationToken>())
            .Returns(new ProjectDto { CurrentImageIndex = 1, Images = new Collection<ImageDto> { new(), new() } });
        var testee = CreateTestee();

        // Act
        await testee.LoadAsync(projectFilePath);

        // Assert
        testee.CurrentImageIndex.Should().Be(1);
        testee.ProjectPath.Should().Be(projectFilePath);
        testee.NumberOfImages.Should().Be(2);
    }

    [Test]
    public async Task Load_PreservesImagePathsAndCopies()
    {
        // Arrange
        var projectFilePath = "MyProjectPath";
        _fileHandler.ReadAsync<ProjectDto>(projectFilePath, Arg.Any<CancellationToken>())
            .Returns(new ProjectDto
            {
                CurrentImageIndex = 0,
                Images = [new ImageDto { Path = "Image1.jpg", NumberOfCopies = 3 }]
            });
        var testee = CreateTestee();

        // Act
        await testee.LoadAsync(projectFilePath);

        // Assert
        testee.CurrentImage!.Path.Should().Be("Image1.jpg");
        testee.CurrentImage!.NumberOfCopies.Should().Be(3);
    }

    [Test]
    public async Task Save()
    {
        // Arrange
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var projectPath = "MyPath";
        var testee = CreateTestee();
        testee.ProjectPath = projectPath;
        testee.AddImages(imageFilePaths);

        // Act
        await testee.SaveAsync();

        // Assert
        await _fileHandler.Received(1).WriteAsync(Arg.Is<ProjectDto>(d => d.Images.Count() == 2), projectPath, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Save_PreservesImageData()
    {
        // Arrange
        var testee = CreateTestee();
        testee.ProjectPath = "MyPath";
        testee.AddImages(["Path1.jpg"]);
        testee.CurrentImage!.Increase();
        testee.CurrentImage!.Increase(); // 2 copies

        // Act
        await testee.SaveAsync();

        // Assert
        await _fileHandler.Received(1).WriteAsync(
            Arg.Is<ProjectDto>(d =>
                d.Images.Any(img => img.Path == "Path1.jpg" && img.NumberOfCopies == 2)),
            "MyPath",
            Arg.Any<CancellationToken>());
    }

    private Project CreateTestee() => new(_fileHandler, _fileSystem);
}
