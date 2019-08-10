using FluentAssertions;
using FotoManagerLogic.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ImageTests
    {
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(2, 1)]
        public void Decrease(int initialNumberOfCopies, int expectedNumberOfCopies)
        {
            var testee = new Image(@"C:\temp\myImage.png", initialNumberOfCopies);

            testee.Decrease();

            testee.NumberOfCopies.Should().Be(expectedNumberOfCopies);
        }

        [TestMethod]
        public void Increase()
        {
            var testee = new Image(@"C:\temp\myImage.png", 2);

            testee.Increase();

            testee.NumberOfCopies.Should().Be(3);
        }
    }
}