using System.Threading;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO;

public interface IFileHandler
{
    Task<T> ReadAsync<T>(string filePath, CancellationToken cancellationToken = default);

    Task WriteAsync<T>(T o, string filePath, CancellationToken cancellationToken = default);
}
