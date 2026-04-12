using System.Threading;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO;

public interface IFileSystem
{
    Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default);

    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

    void Copy(string sourceFileName, string destFileName, bool overwrite);
}
