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
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using IHttpClientFactory = FotoManagerLogic.Business.IHttpClientFactory;

namespace Tests
{
    public class ProjectTests
    {
        [TestCase(2, 1, 1)]
        [TestCase(2, 2, 1)]
        public async Task NextImage(int numberOfImages, int numberOfNextCalls, int expectedImageIndex)
        {
            var imageFilePaths = new Collection<string>();
            for (var i = 0; i < numberOfImages; i++)
            {
                imageFilePaths.Add($"Image {i}");
            }

            var autoMocker = new AutoMocker();
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = autoMocker.CreateInstance<Project>();
            await testee.AddImagesAsync(imageFilePaths);

            for (var i = 0; i < numberOfNextCalls; i++)
            {
                testee.NextImage();
            }

            testee.CurrentImageIndex.Should().Be(expectedImageIndex);
        }

        [TestCase(2, 1, 0)]
        [TestCase(2, 2, 0)]
        [TestCase(3, 1, 1)]
        public async Task PreviousImage(int numberOfImages, int numberOfPreviousCalls, int expectedImageIndex)
        {
            var imageFilePaths = new Collection<string>();
            for (var i = 0; i < numberOfImages; i++)
            {
                imageFilePaths.Add($"Image {i}");
            }

            var autoMocker = new AutoMocker();
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = autoMocker.CreateInstance<Project>();
            await testee.AddImagesAsync(imageFilePaths);
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
        public async Task AddImages()
        {
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var autoMocker = new AutoMocker();
            var httpMock = new MockHttpMessageHandler();
            var mockedRequest = httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(httpMock.ToHttpClient);
            var testee = autoMocker.CreateInstance<Project>();

            await testee.AddImagesAsync(imageFilePaths);

            testee.NumberOfImages.Should().Be(2);
            httpMock.GetMatchCount(mockedRequest).Should().Be(2);
        }

        [Test]
        public async Task Export()
        {
            var imageFilePaths = new Collection<string> { @"D:\input\Path1.jpg", @"D:\input\Path2.jpg" };
            var exportPath = @"C:\temp";
            var progressActionMock = new Mock<Action<double>>();
            var autoMocker = new AutoMocker();
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = autoMocker.CreateInstance<Project>();
            await testee.AddImagesAsync(imageFilePaths);
            testee.CurrentImage.Increase();

            testee.ExportImages(exportPath, progressActionMock.Object);

            autoMocker.GetMock<IFileSystem>().Verify(x => x.Copy(@"D:\input\Path1.jpg", @"C:\temp\Path1_0.jpg", true), Times.Once);
            progressActionMock.Verify(x => x.Invoke(1), Times.Once);
        }

        [Test]
        public async Task GetCurrentImageUrl()
        {
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var autoMocker = new AutoMocker();
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = autoMocker.CreateInstance<Project>();
            testee.ProjectPath = "MyPath";
            await testee.AddImagesAsync(imageFilePaths);

            var result = testee.GetCurrentImageUrl();

            result.Should().MatchRegex("http:\\/\\/localhost:8001\\/api\\/images\\/\\S+");
        }

        [Test]
        public async Task Load()
        {
            var projectFilePath = "MyProjectPath";
            var autoMocker = new AutoMocker();
            autoMocker.Setup<IFileHandler, Task<ProjectDto>>(x => x.ReadAsync<ProjectDto>(projectFilePath))
                      .ReturnsAsync(new ProjectDto
                                    {
                                        CurrentImageIndex = 1, Images = new Collection<ImageDto> { new ImageDto(), new ImageDto() }
                                    });
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = autoMocker.CreateInstance<Project>();

            await testee.LoadAsync(projectFilePath);

            testee.CurrentImageIndex.Should().Be(1);
            testee.ProjectPath.Should().Be(projectFilePath);
        }

        [Test]
        public async Task Save()
        {
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var projectPath = "MyPath";
            var autoMocker = new AutoMocker();
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = autoMocker.CreateInstance<Project>();
            testee.ProjectPath = projectPath;
            await testee.AddImagesAsync(imageFilePaths);

            await testee.SaveAsync();

            autoMocker.GetMock<IFileHandler>()
                      .Verify(x => x.WriteAsync(It.Is<ProjectDto>(d => d.Images.Count() == 2), projectPath), Times.Once);
        }
    }
}