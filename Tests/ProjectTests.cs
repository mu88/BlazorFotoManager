using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;

namespace Tests;

[Category("Unit")]
public class ProjectTests
{
    private readonly IFileHandler _fileHandler = Substitute.For<IFileHandler>();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    [TestCase(2, 1, 1)]
    [TestCase(2, 2, 1)]
    public void NextImage(int numberOfImages, int numberOfNextCalls, int expectedImageIndex)
    {
        var imageFilePaths = new Collection<string>();
        for (var i = 0; i < numberOfImages; i++)
        {
            imageFilePaths.Add($"Image {i}");
        }

        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);

        for (var i = 0; i < numberOfNextCalls; i++)
        {
            testee.NextImage();
        }

        testee.CurrentImageIndex.Should().Be(expectedImageIndex);
    }

    [TestCase(2, 1, 0)]
    [TestCase(2, 2, 0)]
    [TestCase(3, 1, 1)]
    public void PreviousImage(int numberOfImages, int numberOfPreviousCalls, int expectedImageIndex)
    {
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

        for (var i = 0; i < numberOfPreviousCalls; i++)
        {
            testee.PreviousImage();
        }

        testee.CurrentImageIndex.Should().Be(expectedImageIndex);
    }

    [Test]
    public void AddImages()
    {
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var testee = CreateTestee();

        testee.AddImages(imageFilePaths);

        testee.NumberOfImages.Should().Be(2);
    }

    [Test]
    public void Export()
    {
        var imageFilePaths = new Collection<string> { Path.Combine("input", "Path1.jpg"), Path.Combine("input", "Path2.jpg") };
        var exportPath = "output";
        var progressActionMock = Substitute.For<Action<double>>();
        var testee = CreateTestee();
        testee.AddImages(imageFilePaths);
        testee.CurrentImage.Increase();

        testee.ExportImages(exportPath, progressActionMock);

        _fileSystem.Received(1).Copy(Path.Combine("input", "Path1.jpg"), Path.Combine(exportPath, "Path1_0.jpg"), true);
        progressActionMock.Received(1).Invoke(1);
    }

    [Test]
    public void GetCurrentImageUrl()
    {
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var testee = CreateTestee();
        testee.ProjectPath = "MyPath";
        testee.AddImages(imageFilePaths);

        var result = testee.GetCurrentImageUrl();

        result.Should().StartWith("/api/images?path=");
    }

    [Test]
    public async Task Load()
    {
        var projectFilePath = "MyProjectPath";
        _fileHandler.ReadAsync<ProjectDto>(projectFilePath).Returns(new ProjectDto { CurrentImageIndex = 1, Images = new Collection<ImageDto> { new(), new() } });
        var testee = CreateTestee();

        await testee.LoadAsync(projectFilePath);

        testee.CurrentImageIndex.Should().Be(1);
        testee.ProjectPath.Should().Be(projectFilePath);
    }

    [Test]
    public async Task Save()
    {
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var projectPath = "MyPath";
        var testee = CreateTestee();
        testee.ProjectPath = projectPath;
        testee.AddImages(imageFilePaths);

        await testee.SaveAsync();

        await _fileHandler.Received(1).WriteAsync(Arg.Is<ProjectDto>(d => d.Images.Count() == 2), projectPath);
    }

    private Project CreateTestee() => new(_fileHandler, _fileSystem);
}
