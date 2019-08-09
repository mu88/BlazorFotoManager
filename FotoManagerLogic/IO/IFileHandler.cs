using System.Threading.Tasks;

namespace FotoManagerLogic.IO
{
    public interface IFileHandler
    {
        Task<T> ReadAsync<T>(string filePath);

        Task WriteAsync<T>(T o, string filePath);
    }
}