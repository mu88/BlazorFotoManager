using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NUnit.Framework;

namespace Tests.Integration;

[Category("Integration")]
public class JsonFileHandlerIntegrationTests
{
    private string _tempFilePath = string.Empty;

    [SetUp]
    public void SetUp() => _tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    [Test]
    public async Task WriteAsync_Then_ReadAsync_ReturnsOriginalObject()
    {
        // Arrange
        var original = new ProjectDto
        {
            CurrentImageIndex = 42,
            Images = [new ImageDto { Path = "test.jpg", NumberOfCopies = 3 }]
        };
        var testee = new JsonFileHandler(new FileSystem());

        // Act
        await testee.WriteAsync(original, _tempFilePath);
        var result = await testee.ReadAsync<ProjectDto>(_tempFilePath);

        // Assert
        result.CurrentImageIndex.Should().Be(42);
        result.Images.Should().HaveCount(1);
        result.Images.Should().ContainSingle(image => image.Path == "test.jpg" && image.NumberOfCopies == 3);
    }

    [Test]
    public async Task WriteAsync_CreatesFileOnDisk()
    {
        // Arrange
        var dto = new ProjectDto { CurrentImageIndex = 0, Images = [] };
        var testee = new JsonFileHandler(new FileSystem());

        // Act
        await testee.WriteAsync(dto, _tempFilePath);

        // Assert
        File.Exists(_tempFilePath).Should().BeTrue();
    }

    [Test]
    public async Task WriteAsync_Then_ReadAsync_WithCancellation_PropagatesCancellationToken()
    {
        // Arrange
        var dto = new ProjectDto { CurrentImageIndex = 1, Images = [] };
        var testee = new JsonFileHandler(new FileSystem());
        await testee.WriteAsync(dto, _tempFilePath);
        using var cts = new System.Threading.CancellationTokenSource();

        // Act
        var result = await testee.ReadAsync<ProjectDto>(_tempFilePath, cts.Token);

        // Assert
        result.CurrentImageIndex.Should().Be(1);
    }
}
