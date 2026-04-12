using Bunit;
using FluentAssertions;
using FotoManager;
using FotoManagerLogic.Business;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using BunitContext = Bunit.BunitContext;
using IndexPage = FotoManager.Pages.Index;

namespace Tests.Unit.Pages;

[TestFixture]
[Category("Unit")]
public sealed class IndexTests
{
    private BunitContext _ctx = null!;
    private IProjectService _projectServiceMock = null!;
    private IProject _projectMock = null!;
    private ITranslator _translatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _ctx = new BunitContext();
        _projectServiceMock = Substitute.For<IProjectService>();
        _projectMock = Substitute.For<IProject>();
        _translatorMock = Substitute.For<ITranslator>();

        _projectMock.NumberOfImages.Returns(0);
        _projectMock.SumOfCopies.Returns(0);
        _projectMock.CurrentImage.Returns(default(IImage));
        _projectServiceMock.CurrentProject.Returns(_projectMock);
        _projectServiceMock.ExportStatus.Returns(ExportStatus.NotExporting);
        _translatorMock.Translate(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>());

        _ctx.Services.AddSingleton(_projectServiceMock);
        _ctx.Services.AddSingleton(_translatorMock);
    }

    [TearDown]
    public void TearDown() => _ctx.Dispose();

    [Test]
    public void OnInitialized_WithNoImages_SaveProjectButtonIsHidden()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(0);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".fa-save").Should().BeEmpty();
    }

    [Test]
    public void OnInitialized_WithImages_SaveProjectButtonIsVisible()
    {
        // Arrange
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.NumberOfImages.Returns(1);
        _projectMock.CurrentImage.Returns(imageMock);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".fa-save").Should().HaveCount(1);
    }

    [Test]
    public void OnInitialized_WithNoCopies_ExportButtonIsHidden()
    {
        // Arrange
        _projectMock.SumOfCopies.Returns(0);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".fa-clone").Should().BeEmpty();
    }

    [Test]
    public void OnInitialized_WithCopies_ExportButtonIsVisible()
    {
        // Arrange
        _projectMock.SumOfCopies.Returns(1);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".fa-clone").Should().HaveCount(1);
    }

    [Test]
    public void OnInitialized_WithExportSuccessful_ShowsSuccessAlert()
    {
        // Arrange
        _projectServiceMock.ExportStatus.Returns(ExportStatus.ExportSuccessful);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".alert-success").Should().HaveCount(1);
    }

    [Test]
    public void OnInitialized_WithExporting_ShowsExportingAlert()
    {
        // Arrange
        _projectServiceMock.ExportStatus.Returns(ExportStatus.Exporting);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".alert-warning").Should().HaveCount(1);
    }

    [Test]
    public void OnInitialized_WithNotExporting_ShowsNoAlert()
    {
        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".alert-success").Should().BeEmpty();
        cut.FindAll(".alert-warning").Should().BeEmpty();
    }

    [Test]
    public void OnInitialized_WithImagesAndNotExporting_ShowsImageCard()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(1);
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.CurrentImage.Returns(imageMock);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".card").Should().HaveCount(2);
    }

    [Test]
    public void OnInitialized_WithExporting_HidesImageCard()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(1);
        _projectServiceMock.ExportStatus.Returns(ExportStatus.Exporting);

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll(".card").Should().BeEmpty();
    }

    [Test]
    public void OnInitialized_WithCurrentImage_ShowsImage()
    {
        // Arrange
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.CurrentImage.Returns(imageMock);
        _projectServiceMock.GetCurrentImageUrl().Returns("http://localhost/image.jpg");

        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll("img.contain").Should().HaveCount(1);
    }

    [Test]
    public void OnInitialized_WithNoCurrentImage_DoesNotShowImage()
    {
        // Act
        var cut = _ctx.Render<IndexPage>();

        // Assert
        cut.FindAll("img.contain").Should().BeEmpty();
    }

    [Test]
    public void LoadImagesButton_WhenClicked_CallsLoadImagesAsync()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-plus-square").Closest("button")!.Click();

        // Assert
        _projectServiceMock.Received(1).LoadImagesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void OpenProjectButton_WhenClicked_CallsLoadProjectAsync()
    {
        // Arrange
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-folder-open").Closest("button")!.Click();

        // Assert
        _projectServiceMock.Received(1).LoadProjectAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void SaveProjectButton_WhenClicked_CallsSaveProjectAsync()
    {
        // Arrange
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.NumberOfImages.Returns(1);
        _projectMock.CurrentImage.Returns(imageMock);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-save").Closest("button")!.Click();

        // Assert
        _projectServiceMock.Received(1).SaveProjectAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void ExportButton_WhenClicked_CallsExportAsync()
    {
        // Arrange
        _projectMock.SumOfCopies.Returns(1);
        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-clone").Closest("button")!.Click();

        // Assert
        _projectServiceMock.Received(1).ExportAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public void PreviousImageButton_WhenClicked_CallsPreviousImage()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(1);
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.CurrentImage.Returns(imageMock);

        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-chevron-circle-left").Closest("button")!.Click();

        // Assert
        _projectMock.Received(1).PreviousImage();
    }

    [Test]
    public void NextImageButton_WhenClicked_CallsNextImage()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(1);
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.CurrentImage.Returns(imageMock);

        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-chevron-circle-right").Closest("button")!.Click();

        // Assert
        _projectMock.Received(1).NextImage();
    }

    [Test]
    public void DecreaseCopiesButton_WhenClicked_CallsDecrease()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(1);
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.CurrentImage.Returns(imageMock);

        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-minus-circle").Closest("button")!.Click();

        // Assert
        imageMock.Received(1).Decrease();
    }

    [Test]
    public void IncreaseCopiesButton_WhenClicked_CallsIncrease()
    {
        // Arrange
        _projectMock.NumberOfImages.Returns(1);
        var imageMock = Substitute.For<IImage>();
        imageMock.FileName.Returns("test.jpg");
        _projectMock.CurrentImage.Returns(imageMock);

        var cut = _ctx.Render<IndexPage>();

        // Act
        cut.Find(".fa-plus-circle").Closest("button")!.Click();

        // Assert
        imageMock.Received(1).Increase();
    }
}
