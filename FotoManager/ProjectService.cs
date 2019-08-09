using System;
using System.Collections.ObjectModel;
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
            CurrentProject = new Project(new Collection<IImage> { new Image(@"D:\temp\P1020173.JPG") }, FileHandler);
        }

        /// <inheritdoc />
        public bool ProjectLoaded => CurrentProject != null;

        /// <inheritdoc />
        public IProject CurrentProject { get; private set; }

        private IFileHandler FileHandler { get; }

        /// <inheritdoc />
        public void LoadProject()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void LoadImages()
        {
            var images = new Collection<IImage>();

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
            foreach (var imageFilePath in imageFilePaths)
            {
                images.Add(new Image(imageFilePath));
            }

            CurrentProject = new Project(images, FileHandler);
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
                CurrentProject.ProjectPath =
                    Electron.Dialog.ShowSaveDialogAsync(Electron.WindowManager.BrowserWindows.First(), saveDialogOptions)
                            .GetAwaiter()
                            .GetResult();
            }

            CurrentProject.SaveAsync()
                          .GetAwaiter()
                          .GetResult();
        }

        /// <inheritdoc />
        public void Export()
        {
            throw new NotImplementedException();
        }
    }
}