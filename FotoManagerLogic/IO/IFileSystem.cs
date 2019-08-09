using System.Collections.Generic;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO
{
    public interface IFileSystem
    {
        Task WriteAllTextAsync(string path, string contents);

        IAsyncEnumerable<string> ReadAllTextAsync(string path);
    }
}