using System;
using System.Collections.Generic;
using System.Linq;

namespace FotoManagerLogic
{
    public class Project : IProject
    {
        /// <inheritdoc />
        public Project(IEnumerable<IImage> images)
        {
            Images = images;
            CurrentImageIndex = 0;
            DuringExport = false;
            ExportProgressValue = 0;
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
        public int ExportProgressValue { get; }

        /// <inheritdoc />
        public bool DuringExport { get; }

        /// <inheritdoc />
        public string ExportStatus { get; }

        /// <inheritdoc />
        public int CurrentImageIndex { get; private set; }

        /// <param name="projectPath"></param>
        /// <inheritdoc />
        public void Save(string projectPath)
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
    }
}