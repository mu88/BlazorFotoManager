using FotoManager.API;
using FotoManagerLogic.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.API
{
    [TestClass]
    public class ImageControllerTests
    {
        [TestMethod]
        public void Add()
        {
            var image = new ServerImage { Id = "123", Path = "MyPath" };
            var serverImageRepositoryMock = new Mock<IServerImageRepository>();
            var testee = new ImageController(serverImageRepositoryMock.Object);

            testee.Post(image);

            serverImageRepositoryMock.Verify(x => x.Add(image), Times.Once);
        }
    }
}