using System;
using System.Collections.ObjectModel;
using System.Linq;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FotoManagerLogic;

namespace FotoManager
{
    public class ProjectService : IProjectService
    {
        /// <inheritdoc />
        public ProjectService()
        {
            CurrentProject = new Project(new Collection<IImage> { new Image(@"D:\temp\P1020173.JPG") });
        }

        /// <inheritdoc />
        public bool ProjectLoaded => CurrentProject != null;

        /// <inheritdoc />
        public IProject CurrentProject { get; private set; }

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

            CurrentProject = new Project(images);
        }

        /// <inheritdoc />
        public void SaveProject()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Export()
        {
            throw new NotImplementedException();
        }
    }
}