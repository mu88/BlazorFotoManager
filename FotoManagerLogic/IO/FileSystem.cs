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
        public async Task<string> ReadAllTextAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        /// <inheritdoc />
        public void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }
    }
}