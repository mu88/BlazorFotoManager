using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FotoManagerLogic.Business;

public interface IProject
{
    int NumberOfImages { get; }

    int SumOfCopies { get; }

    IImage? CurrentImage { get; }

    int CurrentImageIndex { get; }

    string ProjectPath { get; set; }

    Task SaveAsync(CancellationToken cancellationToken = default);

    void ExportImages(string exportPath, Action<double> progressAction);

    void NextImage();

    void PreviousImage();

    Task LoadAsync(string projectFilePath, CancellationToken cancellationToken = default);

    void AddImages(IEnumerable<string> imageFilePaths);
}
