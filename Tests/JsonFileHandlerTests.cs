using System.Threading.Tasks;
using FluentAssertions;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace Tests
{
    public class JsonFileHandlerTests
    {
        [Test]
        public async Task Deserialize()
        {
            var autoMocker = new AutoMocker();
            autoMocker.Setup<IFileSystem, Task<string>>(x => x.ReadAllTextAsync("MyFile"))
                      .ReturnsAsync(@"{""Path"": ""Bla"", ""NumberOfCopies"": 3}");
            var testee = autoMocker.CreateInstance<JsonFileHandler>();

            var result = await testee.ReadAsync<ImageDto>("MyFile");

            result.NumberOfCopies.Should().Be(3);
            result.Path.Should().Be("Bla");
        }

        [Test]
        public async Task Serialize()
        {
            var autoMocker = new AutoMocker();
            var testee = autoMocker.CreateInstance<JsonFileHandler>();

            await testee.WriteAsync(new { Id = 1, Name = "Foo" }, "MyFile");

            autoMocker.GetMock<IFileSystem>().Verify(x => x.WriteAllTextAsync("MyFile", It.IsAny<string>()), Times.Once);
        }
    }
}