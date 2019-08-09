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
        public Project(IFileHandler fileHandler)
        {
            Images = new Collection<IImage>();
            FileHandler = fileHandler;
            CurrentImageIndex = 0;
            DuringExport = false;
            ExportProgressValue = 0;
        }

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
        public string ProjectPath { get; set; }

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

        /// <inheritdoc />
        public async Task LoadAsync(string projectFilePath)
        {
            var projectDto = await FileHandler.ReadAsync<ProjectDto>(projectFilePath);
            ProjectPath = projectDto.ProjectPath;
            CurrentImageIndex = projectDto.CurrentImageIndex;
            foreach (var imageDto in projectDto.Images)
            {
                ((Collection<IImage>)Images).Add(new Image(imageDto.Path, imageDto.NumberOfCopies));
            }
        }

        /// <inheritdoc />
        public void AddImages(IEnumerable<string> imageFilePaths)
        {
            foreach (var imageFilePath in imageFilePaths)
            {
                ((Collection<IImage>)Images).Add(new Image(imageFilePath));
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