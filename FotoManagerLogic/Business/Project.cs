using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FotoManagerLogic.API;
using FotoManagerLogic.DTO;
using FotoManagerLogic.IO;

namespace FotoManagerLogic.Business;

public class Project : IProject
{
    private const string ApiEndpoint = "http://localhost:8001/api/images";

    public Project(IFileHandler fileHandler, IFileSystem fileSystem, IHttpClientFactory httpClientFactory)
    {
        FileHandler = fileHandler;
        FileSystem = fileSystem;

        HttpClient = httpClientFactory.CreateClient();
        Images = [];
        CurrentImageIndex = 0;
        ProjectPath = string.Empty;
    }

    /// <inheritdoc />
    public int NumberOfImages => Images.Count;

    /// <inheritdoc />
    public int SumOfCopies => Images.Sum(x => x.NumberOfCopies);

    /// <inheritdoc />
    public IImage CurrentImage => Images.ElementAtOrDefault(CurrentImageIndex);

    /// <inheritdoc />
    public string ProjectPath { get; set; }

    /// <inheritdoc />
    public int CurrentImageIndex { get; private set; }

    private Collection<IImage> Images { get; }

    private IFileHandler FileHandler { get; }

    private IFileSystem FileSystem { get; }

    private HttpClient HttpClient { get; }

    /// <inheritdoc />
    public Task SaveAsync()
    {
        var projectDto = GetProjectDto();
        return FileHandler.WriteAsync(projectDto, ProjectPath);
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
    public async Task LoadAsync(string projectFilePath)
    {
        var projectDto = await FileHandler.ReadAsync<ProjectDto>(projectFilePath);
        ProjectPath = projectFilePath;
        CurrentImageIndex = projectDto.CurrentImageIndex;
        foreach (var imageDto in projectDto.Images)
        {
            await AddImageAsync(new Image(imageDto.Path, imageDto.NumberOfCopies));
        }
    }

    /// <inheritdoc />
    public async Task AddImagesAsync(IEnumerable<string> imageFilePaths)
    {
        // TODO mu88: Add status indicator

        foreach (var imageFilePath in imageFilePaths)
        {
            await AddImageAsync(new Image(imageFilePath));
        }
    }

    public string GetCurrentImageUrl() => $"{ApiEndpoint}/{CurrentImage.Id}";

    private async Task AddImageAsync(Image image)
    {
        Images.Add(image);
        await HttpClient.PostAsync(ApiEndpoint,
            new StringContent(JsonSerializer.Serialize(new ServerImage { Id = image.Id, Path = image.Path }),
                Encoding.UTF8,
                "application/json"));
    }

    private ProjectDto GetProjectDto()
    {
        var result = new ProjectDto { CurrentImageIndex = CurrentImageIndex };

        var images = new Collection<ImageDto>();
        foreach (var image in Images)
        {
            images.Add(new ImageDto { Path = image.Path, NumberOfCopies = image.NumberOfCopies });
        }

        result.Images = images;

        return result;
    }
}