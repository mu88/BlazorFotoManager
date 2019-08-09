using System.Threading.Tasks;

namespace FotoManagerLogic.IO
{
    public interface IFileHandler
    {
        T Read<T>(string filePath);

        Task WriteAsync<T>(T o, string filePath);
    }
}