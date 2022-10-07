using FotoManager.API;
using FotoManagerLogic.API;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace Tests.API;

public class ImageControllerTests
{
    [Test]
    public void Add()
    {
        var image = new ServerImage { Id = "123", Path = "MyPath" };
        var autoMocker = new AutoMocker();
        var testee = autoMocker.CreateInstance<ImageController>();

        testee.Post(image);

        autoMocker.GetMock<IServerImageRepository>().Verify(x => x.Add(image), Times.Once);
    }
}