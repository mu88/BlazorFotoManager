using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using IHttpClientFactory = FotoManagerLogic.Business.IHttpClientFactory;

namespace Tests;

public class ProjectTests
{
    private readonly IFileHandler _fileHandler = Substitute.For<IFileHandler>();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();

    [TestCase(2, 1, 1)]
    [TestCase(2, 2, 1)]
    public async Task NextImage(int numberOfImages, int numberOfNextCalls, int expectedImageIndex)
    {
        var imageFilePaths = new Collection<string>();
        for (var i = 0; i < numberOfImages; i++) { imageFilePaths.Add($"Image {i}"); }

        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();
        await testee.AddImagesAsync(imageFilePaths);

        for (var i = 0; i < numberOfNextCalls; i++) { testee.NextImage(); }

        testee.CurrentImageIndex.Should().Be(expectedImageIndex);
    }

    [TestCase(2, 1, 0)]
    [TestCase(2, 2, 0)]
    [TestCase(3, 1, 1)]
    public async Task PreviousImage(int numberOfImages, int numberOfPreviousCalls, int expectedImageIndex)
    {
        var imageFilePaths = new Collection<string>();
        for (var i = 0; i < numberOfImages; i++) { imageFilePaths.Add($"Image {i}"); }

        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();
        await testee.AddImagesAsync(imageFilePaths);
        for (var i = 0; i < numberOfImages; i++) { testee.NextImage(); }

        for (var i = 0; i < numberOfPreviousCalls; i++) { testee.PreviousImage(); }

        testee.CurrentImageIndex.Should().Be(expectedImageIndex);
    }

    [Test]
    public async Task AddImages()
    {
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var httpMock = new MockHttpMessageHandler();
        var mockedRequest = httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();

        await testee.AddImagesAsync(imageFilePaths);

        testee.NumberOfImages.Should().Be(2);
        httpMock.GetMatchCount(mockedRequest).Should().Be(2);
    }

    [Test]
    public async Task Export()
    {
        var imageFilePaths = new Collection<string> { @"D:\input\Path1.jpg", @"D:\input\Path2.jpg" };
        var exportPath = @"C:\temp";
        var progressActionMock = Substitute.For<Action<double>>();
        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();
        await testee.AddImagesAsync(imageFilePaths);
        testee.CurrentImage.Increase();

        testee.ExportImages(exportPath, progressActionMock);

        _fileSystem.Received(1).Copy(@"D:\input\Path1.jpg", @"C:\temp\Path1_0.jpg", true);
        progressActionMock.Received(1).Invoke(1);
    }

    [Test]
    public async Task GetCurrentImageUrl()
    {
        var imageFilePaths = new Collection<string> { "Path1", "Path2" };
        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();
        testee.ProjectPath = "MyPath";
        await testee.AddImagesAsync(imageFilePaths);

        var result = testee.GetCurrentImageUrl();

        result.Should().MatchRegex("http:\\/\\/localhost:8001\\/api\\/images\\/\\S+");
    }

    [Test]
    public async Task Load()
    {
        var projectFilePath = "MyProjectPath";
        _fileHandler.ReadAsync<ProjectDto>(projectFilePath).Returns(new ProjectDto { CurrentImageIndex = 1, Images = new Collection<ImageDto> { new(), new() } });
        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
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
        var httpMock = new MockHttpMessageHandler();
        httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
        _httpClientFactory.CreateClient().Returns(httpMock.ToHttpClient());
        var testee = CreateTestee();
        testee.ProjectPath = projectPath;
        await testee.AddImagesAsync(imageFilePaths);

        await testee.SaveAsync();

        await _fileHandler.Received(1).WriteAsync(Arg.Is<ProjectDto>(d => d.Images.Count() == 2), projectPath);
    }

    private Project CreateTestee() => new(_fileHandler, _fileSystem, _httpClientFactory);
}