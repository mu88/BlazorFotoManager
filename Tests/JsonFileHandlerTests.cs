using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using NSubstitute;
using NUnit.Framework;

namespace Tests;

[Category("Unit")]
public class JsonFileHandlerTests
{
    private readonly JsonFileHandler _testee;
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    public JsonFileHandlerTests() => _testee = new JsonFileHandler(_fileSystem);

    [Test]
    public async Task Deserialize()
    {
        _fileSystem.ReadAllTextAsync("MyFile").Returns("""{"Path": "Bla", "NumberOfCopies": 3}""");

        var result = await _testee.ReadAsync<ImageDto>("MyFile");

        result.NumberOfCopies.Should().Be(3);
        result.Path.Should().Be("Bla");
    }

    [Test]
    public async Task Serialize()
    {
        await _testee.WriteAsync(new { Id = 1, Name = "Foo" }, "MyFile");

        await _fileSystem.Received(1).WriteAllTextAsync("MyFile", Arg.Any<string>());
    }
}