using System.Threading.Tasks;
using FotoManagerLogic.Business;

namespace FotoManager
{
    public interface IProjectService
    {
        IProject CurrentProject { get; }

        ExportStatus ExportStatus { get; }

        Task LoadProjectAsync();

        Task LoadImagesAsync();

        Task SaveProjectAsync();

        Task ExportAsync();
    }
}