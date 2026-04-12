using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FluentAssertions;
using FotoManager;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Unit;

[Category("Unit")]
public class ProjectServiceTests
{
    private readonly IFileHandler _fileHandler = Substitute.For<IFileHandler>();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IElectronHelper _electronHelper = Substitute.For<IElectronHelper>();
    private readonly ITranslator _translator = Substitute.For<ITranslator>();

    [Test]
    public async Task Export()
    {
        // Arrange
        _electronHelper
            .ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>())
            .Returns([Path.Combine("temp", "subdir")]);
        var testee = CreateTestee();
        testee.CurrentProject.AddImages(["MyImage.jpg"]);
        testee.CurrentProject.CurrentImage!.Increase();

        // Act
        await testee.ExportAsync();

        // Assert
        _fileSystem.Received(1).Copy(Arg.Any<string>(), Path.Combine("temp", "subdir", "MyImage_0.jpg"), true);
        _electronHelper.Received(1).SetProgressBar(-1);
        _electronHelper.Received(2).ReloadBrowserWindow();
        testee.ExportStatus.Should().Be(ExportStatus.ExportSuccessful);
        await _electronHelper.Received().ShowOpenDialogAsync(
            Arg.Any<BrowserWindow>(),
            Arg.Is<OpenDialogOptions>(o => o.Properties.Contains(OpenDialogProperty.openDirectory)));
        _translator.Received().Translate("Please choose the Export location");
    }

    [Test]
    public async Task Export_WhenDialogReturnedEmptyArray_DoesNotExport()
    {
        // Arrange
        _electronHelper
            .ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>())
            .Returns([]);
        var testee = CreateTestee();

        // Act
        await testee.ExportAsync();

        // Assert
        _fileSystem.DidNotReceive().Copy(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
    }

    [Test]
    public async Task LoadImages()
    {
        // Arrange
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns(["MyImage"]);
        var testee = CreateTestee();

        // Act
        await testee.LoadImagesAsync();

        // Assert
        testee.CurrentProject.NumberOfImages.Should().Be(1);
        await _electronHelper.Received().ShowOpenDialogAsync(
            Arg.Any<BrowserWindow>(),
            Arg.Is<OpenDialogOptions>(o =>
                o.Properties.Contains(OpenDialogProperty.openFile) &&
                o.Properties.Contains(OpenDialogProperty.multiSelections) &&
                o.Filters.Any(f => f.Extensions.Contains("jpg") && f.Extensions.Contains("png") && f.Extensions.Contains("gif"))));
        _translator.Received().Translate("Please choose your Images");
        _translator.Received().Translate("Images");
    }

    [Test]
    public async Task LoadProject()
    {
        // Arrange
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns(["MyProject"]);
        _fileHandler.ReadAsync<ProjectDto>("MyProject", Arg.Any<CancellationToken>())
            .Returns(new ProjectDto { Images = new Collection<ImageDto>(), CurrentImageIndex = 3 });
        var testee = CreateTestee();

        // Act
        await testee.LoadProjectAsync();

        // Assert
        testee.CurrentProject.CurrentImageIndex.Should().Be(3);
        await _electronHelper.Received().ShowOpenDialogAsync(
            Arg.Any<BrowserWindow>(),
            Arg.Is<OpenDialogOptions>(o =>
                o.Properties.Contains(OpenDialogProperty.openFile) &&
                o.Filters.Any(f => f.Extensions.Contains("json"))));
        _translator.Received().Translate("Please choose your Project File");
        _translator.Received().Translate("Project File");
    }

    [Test]
    public async Task LoadProject_WhenDialogReturnedEmptyArray_DoesNotLoad()
    {
        // Arrange
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns([]);
        var testee = CreateTestee();

        // Act
        await testee.LoadProjectAsync();

        // Assert
        testee.CurrentProject.CurrentImageIndex.Should().Be(0);
        await _fileHandler.DidNotReceive().ReadAsync<ProjectDto>(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SaveExistingProject()
    {
        // Arrange
        var testee = CreateTestee();
        testee.CurrentProject.ProjectPath = "MyProject";

        // Act
        await testee.SaveProjectAsync();

        // Assert
        await _fileHandler.Received(1).WriteAsync(Arg.Any<ProjectDto>(), "MyProject", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SaveNewProject()
    {
        // Arrange
        _electronHelper.ShowSaveDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<SaveDialogOptions>()).Returns("MyProject");
        var testee = CreateTestee();

        // Act
        await testee.SaveProjectAsync();

        // Assert
        testee.CurrentProject.ProjectPath.Should().Be("MyProject");
        await _fileHandler.Received(1).WriteAsync(Arg.Any<ProjectDto>(), "MyProject", Arg.Any<CancellationToken>());
        await _electronHelper.Received().ShowSaveDialogAsync(
            Arg.Any<BrowserWindow>(),
            Arg.Is<SaveDialogOptions>(o => o.Filters.Any(f => f.Extensions.Contains("json"))));
        _translator.Received().Translate("Please choose where to save your Project File");
        _translator.Received().Translate("Project File");
    }

    [Test]
    public async Task StopSavingNewProjectIfUserAborts()
    {
        // Arrange
        _electronHelper.ShowSaveDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<SaveDialogOptions>()).Returns("");
        var testee = CreateTestee();

        // Act
        await testee.SaveProjectAsync();

        // Assert
        testee.CurrentProject.ProjectPath.Should().BeNullOrEmpty();
        await _fileHandler.DidNotReceive().WriteAsync(Arg.Any<ProjectDto>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task LoadImages_WhenDialogReturnedNull_DoesNotAddImages()
    {
        // Arrange
        _electronHelper
            .ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>())
            .Returns((string[])null!);
        var testee = CreateTestee();

        // Act
        await testee.LoadImagesAsync();

        // Assert
        testee.CurrentProject.NumberOfImages.Should().Be(0);
    }

    [Test]
    public async Task LoadImages_WhenDialogReturnedEmptyArray_DoesNotAddImages()
    {
        // Arrange
        _electronHelper
            .ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>())
            .Returns([]);
        var testee = CreateTestee();

        // Act
        await testee.LoadImagesAsync();

        // Assert
        testee.CurrentProject.NumberOfImages.Should().Be(0);
    }

    [Test]
    public void GetCurrentImageUrl()
    {
        // Arrange
        var testee = CreateTestee();
        testee.CurrentProject.AddImages(["Path1", "Path2"]);

        // Act
        var result = testee.GetCurrentImageUrl();

        // Assert
        result.Should().StartWith("/api/images?path=");
    }

    [Test]
    public void GetCurrentImageUrl_WithNoCurrentImage_ReturnsEmpty()
    {
        // Arrange
        var testee = CreateTestee();

        // Act
        var result = testee.GetCurrentImageUrl();

        // Assert
        result.Should().BeEmpty();
    }

    private ProjectService CreateTestee()
        => new(_fileHandler,
            _fileSystem,
            _electronHelper,
            _translator);
}
