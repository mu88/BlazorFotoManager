using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;

namespace FotoManagerLogic.Business
{
    public class Project : IProject
    {
        /// <inheritdoc />
        public Project(IEnumerable<IImage> images, IFileHandler fileHandler)
        {
            Images = images;
            FileHandler = fileHandler;
            CurrentImageIndex = 0;
            DuringExport = false;
            ExportProgressValue = 0;
        }

        /// <inheritdoc />
        public string ProjectPath { get; set; }

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

        private IFileHandler FileHandler { get; }

        /// <inheritdoc />
        public async Task SaveAsync()
        {
            var projectDto = GetProjectDto();
            await FileHandler.WriteAsync(projectDto, ProjectPath);
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

        private ProjectDto GetProjectDto()
        {
            var result = new ProjectDto { ProjectPath = ProjectPath, CurrentImageIndex = CurrentImageIndex };

            var images = new Collection<ImageDto>();
            foreach (var image in Images)
            {
                images.Add(new ImageDto { Path = image.Path, NumberOfCopies = image.NumberOfCopies });
            }

            result.Images = images;

            return result;
        }
    }
}