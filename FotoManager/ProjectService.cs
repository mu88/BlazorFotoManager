﻿using System.Linq;
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
        public ProjectService(IFileHandler fileHandler,
                              IFileSystem fileSystem,
                              IHttpClientFactory httpClientFactory,
                              IElectronHelper electronHelper)
        {
            FileHandler = fileHandler;
            ElectronHelper = electronHelper;
            CurrentProject = new Project(FileHandler, fileSystem, httpClientFactory);
            ExportStatus = ExportStatus.NotExporting;
        }

        /// <inheritdoc />
        public IProject CurrentProject { get; }

        /// <inheritdoc />
        public ExportStatus ExportStatus { get; private set; }

        private IFileHandler FileHandler { get; }

        private IElectronHelper ElectronHelper { get; }

        /// <inheritdoc />
        public async Task LoadProjectAsync()
        {
            var openDialogOptions = new OpenDialogOptions
                                    {
                                        Title = "Bitte wählen Sie Ihre Projektdatei aus",
                                        Properties = new[] { OpenDialogProperty.openFile },
                                        Filters = new[] { new FileFilter { Extensions = new[] { "json" }, Name = "Projektdatei" } }
                                    };

            var projectFilePath = (await ElectronHelper.ShowOpenDialogAsync(ElectronHelper.GetBrowserWindow(), openDialogOptions))
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(projectFilePath))
            {
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
            var imageFilePaths = await ElectronHelper.ShowOpenDialogAsync(ElectronHelper.GetBrowserWindow(), openDialogOptions);

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
                var saveFilePath = await ElectronHelper.ShowSaveDialogAsync(ElectronHelper.GetBrowserWindow(), saveDialogOptions);

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
            var exportPath =
                (await ElectronHelper.ShowOpenDialogAsync(ElectronHelper.GetBrowserWindow(), openDialogOptions)).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exportPath))
            {
                ExportStatus = ExportStatus.Exporting;

                // this is really not nice, but otherwise,
                // the UI won't be refreshed and no status message is displayed.
                ElectronHelper.ReloadBrowserWindow();

                CurrentProject.ExportImages(exportPath, i => ElectronHelper.SetProgressBar(i));

                ExportStatus = ExportStatus.ExportSuccessful;
                ElectronHelper.SetProgressBar(-1); // remove progress bar

                // this is really not nice, but otherwise,
                // the UI won't be refreshed and no status message is displayed.
                ElectronHelper.ReloadBrowserWindow();
            }
        }
    }
}