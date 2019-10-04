using FotoManager.API;
using FotoManagerLogic.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace Tests.API
{
    [TestClass]
    public class ImageControllerTests
    {
        [TestMethod]
        public void Add()
        {
            var image = new ServerImage { Id = "123", Path = "MyPath" };
            var autoMocker = new AutoMocker();
            var testee = autoMocker.CreateInstance<ImageController>();

            testee.Post(image);

            autoMocker.GetMock<IServerImageRepository>().Verify(x => x.Add(image), Times.Once);
        }
    }
}