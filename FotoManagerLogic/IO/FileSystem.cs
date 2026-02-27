using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO;

[ExcludeFromCodeCoverage]
public class FileSystem : IFileSystem
{
    /// <inheritdoc />
    public Task WriteAllTextAsync(string path, string contents) => File.WriteAllTextAsync(path, contents);

    /// <inheritdoc />
    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

    /// <inheritdoc />
    public void Copy(string sourceFileName, string destFileName, bool overwrite)
    {
        File.Copy(sourceFileName, destFileName, overwrite);
    }
}
