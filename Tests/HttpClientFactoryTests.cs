using FluentAssertions;
using FotoManagerLogic.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HttpClientFactoryTests
    {
        [TestMethod]
        public void CreateClient()
        {
            var testee = new HttpClientFactory();

            testee.CreateClient().Should().NotBeNull();
        }
    }
}