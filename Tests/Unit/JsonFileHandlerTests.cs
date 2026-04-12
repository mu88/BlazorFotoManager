using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Unit;

[Category("Unit")]
public class JsonFileHandlerTests
{
    private readonly JsonFileHandler _testee;
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    public JsonFileHandlerTests() => _testee = new JsonFileHandler(_fileSystem);

    [Test]
    public async Task Deserialize()
    {
        // Arrange
        _fileSystem.ReadAllTextAsync("MyFile", Arg.Any<System.Threading.CancellationToken>())
            .Returns("""{"Path": "Bla", "NumberOfCopies": 3}""");

        // Act
        var result = await _testee.ReadAsync<ImageDto>("MyFile");

        // Assert
        result.NumberOfCopies.Should().Be(3);
        result.Path.Should().Be("Bla");
    }

    [Test]
    public async Task Serialize()
    {
        // Arrange / Act
        await _testee.WriteAsync(new { Id = 1, Name = "Foo" }, "MyFile");

        // Assert
        await _fileSystem.Received(1).WriteAllTextAsync("MyFile", Arg.Any<string>(), Arg.Any<System.Threading.CancellationToken>());
    }
}
