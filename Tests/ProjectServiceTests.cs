using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FluentAssertions;
using FotoManager;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;

namespace Tests;

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
        _electronHelper
            .ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>())
            .Returns([Path.Combine("temp", "subdir")]);
        var testee = CreateTestee();
        testee.CurrentProject.AddImages(["MyImage.jpg"]);
        testee.CurrentProject.CurrentImage.Increase();

        await testee.ExportAsync();

        _fileSystem.Received(1).Copy(Arg.Any<string>(), Path.Combine("temp", "subdir", "MyImage_0.jpg"), true);
        _electronHelper.Received().SetProgressBar(Arg.Any<double>());
    }

    [Test]
    public async Task LoadImages()
    {
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns(["MyImage"]);
        var testee = CreateTestee();

        await testee.LoadImagesAsync();

        testee.CurrentProject.NumberOfImages.Should().Be(1);
    }

    [Test]
    public async Task LoadProject()
    {
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns(["MyProject"]);
        _fileHandler.ReadAsync<ProjectDto>("MyProject").Returns(new ProjectDto { Images = new Collection<ImageDto>(), CurrentImageIndex = 3 });
        var testee = CreateTestee();

        await testee.LoadProjectAsync();

        testee.CurrentProject.CurrentImageIndex.Should().Be(3);
    }

    [Test]
    public async Task SaveExistingProject()
    {
        var testee = CreateTestee();
        testee.CurrentProject.ProjectPath = "MyProject";

        await testee.SaveProjectAsync();

        await _fileHandler.Received(1).WriteAsync(Arg.Any<ProjectDto>(), "MyProject");
    }

    [Test]
    public async Task SaveNewProject()
    {
        _electronHelper.ShowSaveDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<SaveDialogOptions>()).Returns("MyProject");
        var testee = CreateTestee();

        await testee.SaveProjectAsync();

        testee.CurrentProject.ProjectPath.Should().Be("MyProject");
        await _fileHandler.Received(1).WriteAsync(Arg.Any<ProjectDto>(), "MyProject");
    }

    [Test]
    public async Task StopSavingNewProjectIfUserAborts()
    {
        _electronHelper.ShowSaveDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<SaveDialogOptions>()).Returns("");
        var testee = CreateTestee();

        await testee.SaveProjectAsync();

        testee.CurrentProject.ProjectPath.Should().BeNullOrEmpty();
        await _fileHandler.DidNotReceive().WriteAsync(Arg.Any<ProjectDto>(), "MyProject");
    }

    private ProjectService CreateTestee()
        => new(_fileHandler,
            _fileSystem,
            _electronHelper,
            _translator);
}
