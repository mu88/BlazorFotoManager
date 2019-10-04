using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FotoManagerLogic.Business;
using FotoManagerLogic.IO;

namespace FotoManager
{
    public class ProjectService : IProjectService
    {
        /// <inheritdoc />
        public ProjectService(IFileHandler fileHandler, IFileSystem fileSystem, IHttpClientFactory httpClientFactory)
        {
            FileHandler = fileHandler;
            CurrentProject = new Project(FileHandler, fileSystem, httpClientFactory);
            ExportStatus = ExportStatus.NotExporting;
        }

        /// <inheritdoc />
        public IProject CurrentProject { get; }

        /// <inheritdoc />
        public ExportStatus ExportStatus { get; private set; }

        private IFileHandler FileHandler { get; }

        /// <inheritdoc />
        public async Task LoadProjectAsync()
        {
            var openDialogOptions = new OpenDialogOptions
                                    {
                                        Title = "Bitte wählen Sie Ihre Projektdatei aus",
                                        Properties = new[] { OpenDialogProperty.openFile },
                                        Filters = new[] { new FileFilter { Extensions = new[] { "json" }, Name = "Projektdatei" } }
                                    };

            // When using async/await, Blazor will not refresh the UI.
            var projectFilePath =
                (await Electron.Dialog.ShowOpenDialogAsync(Electron.WindowManager.BrowserWindows.First(), openDialogOptions))
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(projectFilePath))
            {
                // When using async/await, Blazor will not refresh the UI.
                await CurrentProject.LoadAsync(projectFilePath);
            }
        }

        /// <inheritdoc />
        public async Task LoadImagesAsync()
        {
            var openDialogOptions = new OpenDialogOptions
                                    {
                                        Title = "Bitte wählen Sie Ihre Bilder aus",
                                        Properties = new[] { OpenDialogProperty.openFile, OpenDialogProperty.multiSelections },
                                        Filters = new[] { new FileFilter { Extensions = new[] { "jpg", "png", "gif" }, Name = "Bilder" } }
                                    };
            var imageFilePaths =
                await Electron.Dialog.ShowOpenDialogAsync(Electron.WindowManager.BrowserWindows.First(), openDialogOptions);

            if (imageFilePaths != null && imageFilePaths.Any())
            {
                await CurrentProject.AddImagesAsync(imageFilePaths);
            }
        }

        /// <inheritdoc />
        public async Task SaveProjectAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentProject.ProjectPath))
            {
                var saveDialogOptions = new SaveDialogOptions
                                        {
                                            Title = "Bitte wählen Sie den Speicherort der Projektdatei aus",
                                            Filters = new[] { new FileFilter { Extensions = new[] { "json" }, Name = "Projektdatei" } }
                                        };
                var saveFilePath =
                    await Electron.Dialog.ShowSaveDialogAsync(Electron.WindowManager.BrowserWindows.First(), saveDialogOptions);

                if (string.IsNullOrWhiteSpace(saveFilePath))
                {
                    return;
                }

                CurrentProject.ProjectPath = saveFilePath;
            }

            await CurrentProject.SaveAsync();
        }

        /// <inheritdoc />
        public async Task ExportAsync()
        {
            var openDialogOptions = new OpenDialogOptions
                                    {
                                        Title = "Bitte wählen Sie den Speicherort aus",
                                        Properties = new[] { OpenDialogProperty.openDirectory }
                                    };
            var browserWindow = Electron.WindowManager.BrowserWindows.First();
            var exportPath = (await Electron.Dialog.ShowOpenDialogAsync(browserWindow, openDialogOptions)).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exportPath))
            {
                ExportStatus = ExportStatus.Exporting;

                // this is really not nice, but otherwise,
                // the UI won't be refreshed and no status message is displayed.
                browserWindow.Reload();

                CurrentProject.ExportImages(exportPath, browserWindow.SetProgressBar);

                ExportStatus = ExportStatus.ExportSuccessful;
                browserWindow.SetProgressBar(-1); // remove progress bar

                // this is really not nice, but otherwise,
                // the UI won't be refreshed and no status message is displayed.
                browserWindow.Reload();
            }
        }
    }
}