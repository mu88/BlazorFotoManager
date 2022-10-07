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
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using IHttpClientFactory = FotoManagerLogic.Business.IHttpClientFactory;

namespace Tests
{
    public class ProjectServiceTests
    {
        [Test]
        public async Task Export()
        {
            var autoMocker = new AutoMocker();
            autoMocker
                .Setup<IElectronHelper, Task<string[]>
                >(x => x.ShowOpenDialogAsync(It.IsAny<BrowserWindow>(), It.IsAny<OpenDialogOptions>()))
                .ReturnsAsync(new[] { @"C:\temp" });
            var httpMock = new MockHttpMessageHandler();
            httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(httpMock.ToHttpClient);
            var testee = autoMocker.CreateInstance<ProjectService>();
            await testee.CurrentProject.AddImagesAsync(new[] { "MyImage.jpg" });
            testee.CurrentProject.CurrentImage.Increase();

            await testee.ExportAsync();

            autoMocker.Verify<IFileSystem>(x => x.Copy(It.IsAny<string>(), @"C:\temp\MyImage_0.jpg", true), Times.Once);
            autoMocker.Verify<IElectronHelper>(x => x.SetProgressBar(It.IsAny<double>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task LoadImages()
        {
            var autoMocker = new AutoMocker();
            autoMocker
                .Setup<IElectronHelper, Task<string[]>
                >(x => x.ShowOpenDialogAsync(It.IsAny<BrowserWindow>(), It.IsAny<OpenDialogOptions>()))
                .ReturnsAsync(new[] { "MyImage" });
            var httpMock = new MockHttpMessageHandler();
            var mockedRequest = httpMock.When(HttpMethod.Post, "/api/images").Respond(HttpStatusCode.OK);
            autoMocker.Setup<IHttpClientFactory, HttpClient>(x => x.CreateClient()).Returns(httpMock.ToHttpClient);
            var testee = autoMocker.CreateInstance<ProjectService>();

            await testee.LoadImagesAsync();

            testee.CurrentProject.NumberOfImages.Should().Be(1);
            httpMock.GetMatchCount(mockedRequest).Should().Be(1);
        }

        [Test]
        public async Task LoadProject()
        {
            var autoMocker = new AutoMocker();
            autoMocker
                .Setup<IElectronHelper, Task<string[]>
                >(x => x.ShowOpenDialogAsync(It.IsAny<BrowserWindow>(), It.IsAny<OpenDialogOptions>()))
                .ReturnsAsync(new[] { "MyProject" });
            autoMocker.Setup<IFileHandler, Task<ProjectDto>>(x => x.ReadAsync<ProjectDto>("MyProject"))
                      .ReturnsAsync(new ProjectDto() { Images = new Collection<ImageDto>(), CurrentImageIndex = 3 });
            var testee = autoMocker.CreateInstance<ProjectService>();

            await testee.LoadProjectAsync();

            testee.CurrentProject.CurrentImageIndex.Should().Be(3);
        }

        [Test]
        public async Task SaveExistingProject()
        {
            var autoMocker = new AutoMocker();
            var testee = autoMocker.CreateInstance<ProjectService>();
            testee.CurrentProject.ProjectPath = "MyProject";

            await testee.SaveProjectAsync();

            autoMocker.Verify<IFileHandler>(x => x.WriteAsync(It.IsAny<ProjectDto>(), "MyProject"), Times.Once);
        }

        [Test]
        public async Task SaveNewProject()
        {
            var autoMocker = new AutoMocker();
            autoMocker
                .Setup<IElectronHelper, Task<string>>(x => x.ShowSaveDialogAsync(It.IsAny<BrowserWindow>(), It.IsAny<SaveDialogOptions>()))
                .ReturnsAsync("MyProject");
            var testee = autoMocker.CreateInstance<ProjectService>();

            await testee.SaveProjectAsync();

            testee.CurrentProject.ProjectPath.Should().Be("MyProject");
            autoMocker.Verify<IFileHandler>(x => x.WriteAsync(It.IsAny<ProjectDto>(), "MyProject"), Times.Once);
        }

        [Test]
        public async Task StopSavingNewProjectIfUserAborts()
        {
            var autoMocker = new AutoMocker();
            autoMocker
                .Setup<IElectronHelper, Task<string>>(x => x.ShowSaveDialogAsync(It.IsAny<BrowserWindow>(), It.IsAny<SaveDialogOptions>()))
                .ReturnsAsync("");
            var testee = autoMocker.CreateInstance<ProjectService>();

            await testee.SaveProjectAsync();

            testee.CurrentProject.ProjectPath.Should().BeNullOrEmpty();
            autoMocker.Verify<IFileHandler>(x => x.WriteAsync(It.IsAny<ProjectDto>(), "MyProject"), Times.Never);
        }
    }
}