using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FotoManagerLogic.IO;

public class JsonFileHandler : IFileHandler
{
    /// <inheritdoc />
    public JsonFileHandler(IFileSystem fileSystem) => FileSystem = fileSystem;

    private IFileSystem FileSystem { get; }

    /// <inheritdoc />
    public async Task<T> ReadAsync<T>(string filePath, CancellationToken cancellationToken = default)
    {
        var s = await FileSystem.ReadAllTextAsync(filePath, cancellationToken);

        return JsonSerializer.Deserialize<T>(s)!;
    }

    /// <inheritdoc />
    public async Task WriteAsync<T>(T o, string filePath, CancellationToken cancellationToken = default)
    {
        var s = JsonSerializer.Serialize(o);
        await FileSystem.WriteAllTextAsync(filePath, s, cancellationToken);
    }
}
