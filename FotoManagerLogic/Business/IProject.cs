using System.Collections.Generic;
using System.Threading.Tasks;

namespace FotoManagerLogic.Business
{
    public interface IProject
    {
        IEnumerable<IImage> Images { get; }

        int NumberOfImages { get; }

        int SumOfCopies { get; }

        IImage CurrentImage { get; }

        int CurrentImageIndex { get; }

        bool DuringExport { get; }

        string ExportStatus { get; }

        int ExportProgressValue { get; }

        string ProjectPath { get; set; }

        Task SaveAsync();

        void ExportImages(string exportPath);

        void NextImage();

        void PreviousImage();

        Task LoadAsync(string projectFilePath);

        void AddImages(IEnumerable<string> imageFilePaths);
    }
}