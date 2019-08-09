using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO
{
    public class FileSystem : IFileSystem
    {
        /// <inheritdoc />
        public async Task WriteAllTextAsync(string path, string contents)
        {
            await File.WriteAllTextAsync(path, contents);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<string> ReadAllTextAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}