using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FluentAssertions;
using FotoManager;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using IHttpClientFactory = FotoManagerLogic.Business.IHttpClientFactory;

namespace Tests;

public class ProjectServiceTests
{
    private readonly IFileHandler _fileHandler = Substitute.For<IFileHandler>();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
    private readonly IElectronHelper _electronHelper = Substitute.For<IElectronHelper>();
    private readonly ITranslator _translator = Substitute.For<ITranslator>();

    [Test]
    public async Task Export()
    {
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns([@"C:\temp"]);
        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        var httpClient = httpMock.ToHttpClient();
        _httpClientFactory.CreateClient().Returns(httpClient);
        var testee = CreateTestee();
        await testee.CurrentProject.AddImagesAsync(["MyImage.jpg"]);
        testee.CurrentProject.CurrentImage.Increase();

        await testee.ExportAsync();

        _fileSystem.Received(1).Copy(Arg.Any<string>(), @"C:\temp\MyImage_0.jpg", true);
        _electronHelper.Received().SetProgressBar(Arg.Any<double>());
    }

    [Test]
    public async Task LoadImages()
    {
        _electronHelper.ShowOpenDialogAsync(Arg.Any<BrowserWindow>(), Arg.Any<OpenDialogOptions>()).Returns(["MyImage"]);
        var httpMock = new MockHttpMessageHandler();
        var mockedRequest = httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();

        await testee.LoadImagesAsync();

        testee.CurrentProject.NumberOfImages.Should().Be(1);
        httpMock.GetMatchCount(mockedRequest).Should().Be(1);
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

    private ProjectService CreateTestee() =>
        new(_fileHandler,
            _fileSystem,
            _httpClientFactory,
            _electronHelper,
            _translator);
}