using FluentAssertions;
using FotoManagerLogic.Business;
using FotoManagerLogic.DTO;
using NUnit.Framework;

namespace Tests.Unit;

[Category("Unit")]
public class ImageTests
{
    [TestCase(0, 0)]
    [TestCase(2, 1)]
    public void Decrease(int initialNumberOfCopies, int expectedNumberOfCopies)
    {
        // Arrange
        var testee = new Image(@"C:\temp\myImage.png", initialNumberOfCopies);

        // Act
        testee.Decrease();

        // Assert
        testee.NumberOfCopies.Should().Be(expectedNumberOfCopies);
    }

    [Test]
    public void Increase()
    {
        // Arrange
        var testee = new Image(@"C:\temp\myImage.png", 2);

        // Act
        testee.Increase();

        // Assert
        testee.NumberOfCopies.Should().Be(3);
    }

    [Test]
    public void HasId()
    {
        // Arrange / Act
        var testee = new Image(@"C:\temp\myImage.png", 2);

        // Assert
        testee.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void ImageDto_DefaultPath_IsEmpty()
    {
        // Arrange / Act
        var dto = new ImageDto();

        // Assert
        dto.Path.Should().BeEmpty();
    }
}
