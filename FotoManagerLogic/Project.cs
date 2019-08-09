using System;
using System.Collections.Generic;
using System.Linq;

namespace FotoManagerLogic
{
    public class Project : IProject
    {
        /// <inheritdoc />
        public Project(string projectPath, IEnumerable<IImage> images)
        {
            ProjectPath = projectPath;
            Images = images;
            CurrentImageIndex = 0;
            DuringExport = false;
        }

        /// <inheritdoc />
        public string ProjectPath { get; }

        /// <inheritdoc />
        public IEnumerable<IImage> Images { get; }

        /// <inheritdoc />
        public int NumberOfImages => Images.Count();

        /// <inheritdoc />
        public int SumOfCopies => Images.Sum(x => x.NumberOfCopies);

        /// <inheritdoc />
        public IImage CurrentImage => Images.ElementAt(CurrentImageIndex);

        /// <inheritdoc />
        public int CurrentImageIndex { get; private set; }

        /// <inheritdoc />
        public void Save()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void ExportImages(string exportPath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void NextImage()
        {
            if (CurrentImageIndex < NumberOfImages - 1)
            {
                CurrentImageIndex++;
            }
        }

        /// <inheritdoc />
        public void PreviousImage()
        {
            if (CurrentImageIndex > 0)
            {
                CurrentImageIndex--;
            }
        }

        /// <inheritdoc />
        public bool DuringExport { get; }

        /// <inheritdoc />
        public string ExportStatus { get; }
    }
}