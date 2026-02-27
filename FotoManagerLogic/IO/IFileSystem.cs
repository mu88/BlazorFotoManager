using System.Threading.Tasks;

namespace FotoManagerLogic.IO;

public interface IFileSystem
{
    Task WriteAllTextAsync(string path, string contents);

    Task<string> ReadAllTextAsync(string path);

    void Copy(string sourceFileName, string destFileName, bool overwrite);
}
