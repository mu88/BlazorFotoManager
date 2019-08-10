using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests
{
    [TestClass]
    public class ProjectTests
    {
        [DataTestMethod]
        [DataRow(2, 1, 1)]
        [DataRow(2, 2, 1)]
        public void NextImage(int numberOfImages, int numberOfNextCalls, int expectedImageIndex)
        {
            var imageFilePaths = new Collection<string>();
            for (var i = 0; i < numberOfImages; i++)
            {
                imageFilePaths.Add($"Image {i}");
            }

            var testee = new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object);
            testee.AddImages(imageFilePaths);

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
        public void PreviousImage(int numberOfImages, int numberOfPreviousCalls, int expectedImageIndex)
        {
            var imageFilePaths = new Collection<string>();
            for (var i = 0; i < numberOfImages; i++)
            {
                imageFilePaths.Add($"Image {i}");
            }

            var testee = new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object);
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

        [TestMethod]
        public void AddImages()
        {
            var imageFilePaths = new Collection<string> { "Path1", "Path2" };
            var testee = new Project(new Mock<IFileHandler>().Object, new Mock<IFileSystem>().Object);

            testee.AddImages(imageFilePaths);

            testee.NumberOfImages.Should().Be(2);
        }

        [TestMethod]
        public void Export()
        {
            var imageFilePaths = new Collection<string> { @"D:\input\Path1.jpg", @"D:\input\Path2.jpg" };
            var exportPath = @"C:\temp";
            var progressActionMock = new Mock<Action<double>>();
            var fileSystemMock = new Mock<IFileSystem>();
            var testee = new Project(new Mock<IFileHandler>().Object, fileSystemMock.Object);
            testee.AddImages(imageFilePaths);
            testee.CurrentImage.Increase();

            testee.ExportImages(exportPath, progressActionMock.Object);

            fileSystemMock.Verify(x => x.Copy(@"D:\input\Path1.jpg", @"C:\temp\Path1_0.jpg", true), Times.Once);
            progressActionMock.Verify(x => x.Invoke(1), Times.Once);
        }

        [TestMethod]
        public async Task Load()
        {
            var projectFilePath = "MyProjectPath";
            var fileHandlerMock = new Mock<IFileHandler>();
            fileHandlerMock.Setup(x => x.ReadAsync<ProjectDto>(projectFilePath))
                           .ReturnsAsync(new ProjectDto
                                         {
                                             CurrentImageIndex = 1, Images = new Collection<ImageDto> { new ImageDto(), new ImageDto() }
                                         });
            var testee = new Project(fileHandlerMock.Object, new Mock<IFileSystem>().Object);

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
            var testee = new Project(fileHandlerMock.Object, new Mock<IFileSystem>().Object) { ProjectPath = projectPath };
            testee.AddImages(imageFilePaths);

            await testee.SaveAsync();

            fileHandlerMock.Verify(x => x.WriteAsync(It.Is<ProjectDto>(d => d.Images.Count() == 2), projectPath), Times.Once);
        }
    }
}