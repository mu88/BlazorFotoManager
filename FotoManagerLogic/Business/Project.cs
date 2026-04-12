using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;

namespace FotoManagerLogic.Business;

public class Project : IProject
{
    public Project(IFileHandler fileHandler, IFileSystem fileSystem)
    {
        FileHandler = fileHandler;
        FileSystem = fileSystem;

        Images = [];
        CurrentImageIndex = 0;
        ProjectPath = string.Empty;
    }

    /// <inheritdoc />
    public int NumberOfImages => Images.Count;

    /// <inheritdoc />
    public int SumOfCopies => Images.Sum(image => image.NumberOfCopies);

    /// <inheritdoc />
    public IImage? CurrentImage => Images.ElementAtOrDefault(CurrentImageIndex);

    /// <inheritdoc />
    public string ProjectPath { get; set; }

    /// <inheritdoc />
    public int CurrentImageIndex { get; private set; }

    private Collection<IImage> Images { get; }

    private IFileHandler FileHandler { get; }

    private IFileSystem FileSystem { get; }

    /// <inheritdoc />
    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var projectDto = GetProjectDto();
        return FileHandler.WriteAsync(projectDto, ProjectPath, cancellationToken);
    }

    /// <inheritdoc />
    public void ExportImages(string exportPath, Action<double> progressAction)
    {
        float localImageCounter = 0;

        foreach (var image in Images)
        {
            for (var i = 0; i < image.NumberOfCopies; i++)
            {
                progressAction(++localImageCounter / SumOfCopies);

                var destinationFile = Path.Combine(exportPath, $"{image.FileName}_{i}{Path.GetExtension(image.Path)}");
                FileSystem.Copy(image.Path, destinationFile, true);
            }
        }
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
    public async Task LoadAsync(string projectFilePath, CancellationToken cancellationToken = default)
    {
        var projectDto = await FileHandler.ReadAsync<ProjectDto>(projectFilePath, cancellationToken);
        ProjectPath = projectFilePath;
        CurrentImageIndex = projectDto.CurrentImageIndex;
        foreach (var imageDto in projectDto.Images)
        {
            Images.Add(new Image(imageDto.Path, imageDto.NumberOfCopies));
        }
    }

    /// <inheritdoc />
    public void AddImages(IEnumerable<string> imageFilePaths)
    {
        foreach (var imageFilePath in imageFilePaths)
        {
            Images.Add(new Image(imageFilePath));
        }
    }

    private ProjectDto GetProjectDto()
    {
        var images = Images.Select(image => new ImageDto { Path = image.Path, NumberOfCopies = image.NumberOfCopies });
        return new ProjectDto { CurrentImageIndex = CurrentImageIndex, Images = images };
    }
}
