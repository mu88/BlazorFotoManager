using FluentAssertions;
using FotoManagerLogic.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.API
{
    [TestClass]
    public class ServerImageRepositoryTests
    {
        [TestMethod]
        public void GetImageContent()
        {
            var testee = new ServerImageRepository();
            testee.Add(new ServerImage { Id = "123", Path = "MyPath1" });

            var result = testee.GetPath("123");

            result.Should().Be("MyPath1");
        }
    }
}