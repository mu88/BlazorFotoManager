using FotoManager.API;
using FotoManagerLogic.API;
using NSubstitute;
using NUnit.Framework;

namespace Tests.API;

[Category("Unit")]
public class ImageControllerTests
{
    private readonly IServerImageRepository _serverImageRepository = Substitute.For<IServerImageRepository>();

    [Test]
    public void Add()
    {
        var image = new ServerImage { Id = "123", Path = "MyPath" };
        var testee = new ImageController(_serverImageRepository);

        testee.Post(image);

        _serverImageRepository.Received(1).Add(image);
    }
}