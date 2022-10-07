using FluentAssertions;
using FotoManagerLogic.API;
using NUnit.Framework;

namespace Tests.API
{
    public class ServerImageRepositoryTests
    {
        [Test]
        public void GetImageContent()
        {
            var testee = new ServerImageRepository();
            testee.Add(new ServerImage { Id = "123", Path = "MyPath1" });

            var result = testee.GetPath("123");

            result.Should().Be("MyPath1");
        }
    }
}