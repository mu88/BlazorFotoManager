using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO
{
    public class JsonFileHandler : IFileHandler
    {
        /// <inheritdoc />
        public JsonFileHandler(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        private IFileSystem FileSystem { get; }

        /// <inheritdoc />
        public T Read<T>(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task WriteAsync<T>(T o, string filePath)
        {
            var s = JsonSerializer.Serialize(o);
            await FileSystem.WriteAllTextAsync(filePath, s);
        }
    }
}