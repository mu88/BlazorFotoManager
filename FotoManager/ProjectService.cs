using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FotoManagerLogic.Business;
using FotoManagerLogic.IO;

namespace FotoManager;

public class ProjectService : IProjectService
{
    /// <inheritdoc />
    public ProjectService(
        IFileHandler fileHandler,
        IFileSystem fileSystem,
        IElectronHelper electronHelper,
        ITranslator translator)
    {
        FileHandler = fileHandler;
        ElectronHelper = electronHelper;
        Translator = translator;
        CurrentProject = new Project(FileHandler, fileSystem);
        ExportStatus = ExportStatus.NotExporting;
    }

    /// <inheritdoc />
    public IProject CurrentProject { get; }

    /// <inheritdoc />
    public ExportStatus ExportStatus { get; private set; }

    private IFileHandler FileHandler { get; }

    private IElectronHelper ElectronHelper { get; }

    private ITranslator Translator { get; }

    /// <inheritdoc />
    public async Task LoadProjectAsync(CancellationToken cancellationToken = default)
    {
        var openDialogOptions = new OpenDialogOptions
        {
            Title = Translator.Translate("Please choose your Project File"),
            Properties = [OpenDialogProperty.openFile],
            Filters = [new FileFilter { Extensions = ["json"], Name = Translator.Translate("Project File") }]
        };

        var projectFilePath = (await ElectronHelper.ShowOpenDialogAsync(ElectronHelper.GetBrowserWindow(), openDialogOptions))
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(projectFilePath))
        {
            await CurrentProject.LoadAsync(projectFilePath, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task LoadImagesAsync(CancellationToken cancellationToken = default)
    {
        var openDialogOptions = new OpenDialogOptions
        {
            Title = Translator.Translate("Please choose your Images"),
            Properties = [OpenDialogProperty.openFile, OpenDialogProperty.multiSelections],
            Filters = [new FileFilter { Extensions = ["jpg", "png", "gif"], Name = Translator.Translate("Images") }]
        };
        var imageFilePaths = await ElectronHelper.ShowOpenDialogAsync(ElectronHelper.GetBrowserWindow(), openDialogOptions);

        if (imageFilePaths != null && imageFilePaths.Length != 0)
        {
            CurrentProject.AddImages(imageFilePaths);
        }
    }

    /// <inheritdoc />
    public async Task SaveProjectAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(CurrentProject.ProjectPath))
        {
            var saveDialogOptions = new SaveDialogOptions
            {
                Title = Translator.Translate("Please choose where to save your Project File"),
                Filters = [new FileFilter { Extensions = ["json"], Name = Translator.Translate("Project File") }]
            };
            var saveFilePath = await ElectronHelper.ShowSaveDialogAsync(ElectronHelper.GetBrowserWindow(), saveDialogOptions);

            if (string.IsNullOrWhiteSpace(saveFilePath))
            {
                return;
            }

            CurrentProject.ProjectPath = saveFilePath;
        }

        await CurrentProject.SaveAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task ExportAsync(CancellationToken cancellationToken = default)
    {
        var openDialogOptions = new OpenDialogOptions { Title = Translator.Translate("Please choose the Export location"), Properties = [OpenDialogProperty.openDirectory] };
        var exportPath =
            (await ElectronHelper.ShowOpenDialogAsync(ElectronHelper.GetBrowserWindow(), openDialogOptions)).FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(exportPath))
        {
            ExportStatus = ExportStatus.Exporting;

            // this is really not nice, but otherwise,
            // the UI won't be refreshed and no status message is displayed.
            ElectronHelper.ReloadBrowserWindow();

            CurrentProject.ExportImages(exportPath, progress => ElectronHelper.SetProgressBar(progress));

            ExportStatus = ExportStatus.ExportSuccessful;
            ElectronHelper.SetProgressBar(-1); // remove progress bar

            // this is really not nice, but otherwise,
            // the UI won't be refreshed and no status message is displayed.
            ElectronHelper.ReloadBrowserWindow();
        }
    }

    /// <inheritdoc />
    public string GetCurrentImageUrl()
    {
        if (CurrentProject.CurrentImage is null)
            return string.Empty;

        var encodedPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(CurrentProject.CurrentImage.Path));
        return $"/api/images?path={Uri.EscapeDataString(encodedPath)}";
    }
}
