using System.Threading;
using System.Threading.Tasks;
using FotoManagerLogic.Business;

namespace FotoManager;

public interface IProjectService
{
    IProject CurrentProject { get; }

    ExportStatus ExportStatus { get; }

    Task LoadProjectAsync(CancellationToken cancellationToken = default);

    Task LoadImagesAsync(CancellationToken cancellationToken = default);

    Task SaveProjectAsync(CancellationToken cancellationToken = default);

    Task ExportAsync(CancellationToken cancellationToken = default);

    string GetCurrentImageUrl();
}
