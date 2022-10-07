using FluentAssertions;
using FotoManagerLogic.Business;
using NUnit.Framework;

namespace Tests
{
    public class ImageTests
    {
        [TestCase(0, 0)]
        [TestCase(2, 1)]
        public void Decrease(int initialNumberOfCopies, int expectedNumberOfCopies)
        {
            var testee = new Image(@"C:\temp\myImage.png", initialNumberOfCopies);

            testee.Decrease();

            testee.NumberOfCopies.Should().Be(expectedNumberOfCopies);
        }

        [Test]
        public void Increase()
        {
            var testee = new Image(@"C:\temp\myImage.png", 2);

            testee.Increase();

            testee.NumberOfCopies.Should().Be(3);
        }

        [Test]
        public void HasId()
        {
            var testee = new Image(@"C:\temp\myImage.png", 2);

            testee.Id.Should().NotBeNullOrWhiteSpace();
        }
    }
}