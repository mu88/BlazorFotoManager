using System;
using System.Linq;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FotoManagerLogic.Business;
using FotoManagerLogic.IO;

namespace FotoManager
{
    public class ProjectService : IProjectService
    {
        /// <inheritdoc />
        public ProjectService(IFileHandler fileHandler)
        {
            FileHandler = fileHandler;
            CurrentProject = new Project(FileHandler);
        }

        /// <inheritdoc />
        public IProject CurrentProject { get; }

        private IFileHandler FileHandler { get; }

        /// <inheritdoc />
        public void LoadProject()
        {
            var openDialogOptions = new OpenDialogOptions
                                    {
                                        Title = "Bitte wählen Sie Ihre Projektdatei aus",
                                        Properties = new[] { OpenDialogProperty.openFile },
                                        Filters = new[] { new FileFilter { Extensions = new[] { "json" }, Name = "Projektdatei" } }
                                    };

            // When using async/await, Blazor will not refresh the UI.
            var projectFilePath = Electron.Dialog.ShowOpenDialogAsync(Electron.WindowManager.BrowserWindows.First(), openDialogOptions)
                                          .GetAwaiter()
                                          .GetResult()
                                          .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(projectFilePath))
            {
                // When using async/await, Blazor will not refresh the UI.
                CurrentProject.LoadAsync(projectFilePath).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public void LoadImages()
        {
            var openDialogOptions = new OpenDialogOptions
                                    {
                                        Title = "Bitte wählen Sie Ihre Bilder aus",
                                        Properties = new[] { OpenDialogProperty.openFile, OpenDialogProperty.multiSelections },
                                        Filters = new[] { new FileFilter { Extensions = new[] { "jpg", "png", "gif" }, Name = "Bilder" } }
                                    };

            // When using async/await, Blazor will not refresh the UI.
            var imageFilePaths = Electron.Dialog.ShowOpenDialogAsync(Electron.WindowManager.BrowserWindows.First(), openDialogOptions)
                                         .GetAwaiter()
                                         .GetResult();

            if (imageFilePaths != null && imageFilePaths.Any())
            {
                CurrentProject.AddImages(imageFilePaths);
            }
        }

        /// <inheritdoc />
        public void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(CurrentProject.ProjectPath))
            {
                var saveDialogOptions = new SaveDialogOptions
                                        {
                                            Title = "Bitte wählen Sie den Speicherort der Projektdatei aus",
                                            Filters = new[] { new FileFilter { Extensions = new[] { "json" }, Name = "Projektdatei" } }
                                        };
                // When using async/await, Blazor will not refresh the UI.
                var saveFilePath = Electron.Dialog.ShowSaveDialogAsync(Electron.WindowManager.BrowserWindows.First(), saveDialogOptions)
                                           .GetAwaiter()
                                           .GetResult();

                if (string.IsNullOrWhiteSpace(saveFilePath))
                {
                    return;
                }

                CurrentProject.ProjectPath = saveFilePath;
            }

            // When using async/await, Blazor will not refresh the UI.
            CurrentProject.SaveAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public void Export()
        {
            throw new NotImplementedException();
        }
    }
}