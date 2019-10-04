using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;
using IHttpClientFactory = FotoManagerLogic.Business.IHttpClientFactory;

namespace Tests
{
    [TestClass]
    public class ProjectTests
    {
        [DataTestMethod]
        [DataRow(2, 1, 1)]
        [DataRow(2, 2, 1)]
        public async Task NextImage(int numberOfImages, int numberOfNextCalls, int expectedImageIndex)
        {
            var imageFilePaths = new Collection<string>();
            for (var i = 0; i < numberOfImages; i++)
            {
                imageFilePaths.Add($"Image {i}");
            }

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object, httpClientFactoryMock.Object);
            await testee.AddImagesAsync(imageFilePaths);

            for (var i = 0; i < numberOfNextCalls; i++)
            {
                testee.NextImage();
            }

            testee.CurrentImageIndex.Should().Be(expectedImageIndex);
        }

        [DataTestMethod]
        [DataRow(2, 1, 0)]
        [DataRow(2, 2, 0)]
        [DataRow(3, 1, 1)]
        public async Task PreviousImage(int numberOfImages, int numberOfPreviousCalls, int expectedImageIndex)
        {
            var imageFilePaths = new Collection<string>();
            for (var i = 0; i < numberOfImages; i++)
            {
                imageFilePaths.Add($"Image {i}");
            }

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object, httpClientFactoryMock.Object);
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

        [TestMethod]
        public async Task AddImages()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Post, "/api/images");
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(mockHttp.ToHttpClient());
            var testee = new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object, httpClientFactoryMock.Object);

            await testee.AddImagesAsync(imageFilePaths);

            testee.NumberOfImages.Should().Be(2);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async void Export()
        {
            var imageFilePaths = new Collection<string> { @"D:\input\Path1.jpg", @"D:\input\Path2.jpg" };
            var exportPath = @"C:\temp";
            var progressActionMock = new Mock<Action<double>>();
            var fileSystemMock = new Mock<IFileSystem>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee = new Project(new Mock<IFileHandler>().Object, fileSystemMock.Object, httpClientFactoryMock.Object);
            await testee.AddImagesAsync(imageFilePaths);
            testee.CurrentImage.Increase();

            testee.ExportImages(exportPath, progressActionMock.Object);

            fileSystemMock.Verify(x => x.Copy(@"D:\input\Path1.jpg", @"C:\temp\Path1_0.jpg", true), Times.Once);
            progressActionMock.Verify(x => x.Invoke(1), Times.Once);
        }

        [TestMethod]
        public async Task GetCurrentImageUrl()
        {
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var projectPath = "MyPath";
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee =
                new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object, httpClientFactoryMock.Object)
                {
                    ProjectPath = projectPath
                };
            await testee.AddImagesAsync(imageFilePaths);

            var result = testee.GetCurrentImageUrl();

            result.Should().MatchRegex("http:\\/\\/localhost:8001\\/api\\/images\\/\\S+");
        }

        [TestMethod]
        public async Task Load()
        {
            var projectFilePath = "MyProjectPath";
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var fileHandlerMock = new Mock<IFileHandler>();
            fileHandlerMock.Setup(x => x.ReadAsync<ProjectDto>(projectFilePath))
                           .ReturnsAsync(new ProjectDto
                                         {
                                             CurrentImageIndex = 1, Images = new Collection<ImageDto> { new ImageDto(), new ImageDto() }
                                         });
            var testee = new Project(fileHandlerMock.Object, new Mock<IFileSystem>().Object, httpClientFactoryMock.Object);

            await testee.LoadAsync(projectFilePath);

            testee.CurrentImageIndex.Should().Be(1);
            testee.ProjectPath.Should().Be(projectFilePath);
        }

        [TestMethod]
        public async Task Save()
        {
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var projectPath = "MyPath";
            var fileHandlerMock = new Mock<IFileHandler>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(new MockHttpMessageHandler().ToHttpClient());
            var testee =
                new Project(fileHandlerMock.Object, new Mock<IFileSystem>().Object, httpClientFactoryMock.Object)
                {
                    ProjectPath = projectPath
                };
            await testee.AddImagesAsync(imageFilePaths);

            await testee.SaveAsync();

            fileHandlerMock.Verify(x => x.WriteAsync(It.Is<ProjectDto>(d => d.Images.Count() == 2), projectPath), Times.Once);
        }
    }
}