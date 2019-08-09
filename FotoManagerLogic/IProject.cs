using System.Collections.Generic;

namespace FotoManagerLogic
{
    public interface IProject
    {
        string ProjectPath { get; }

        IEnumerable<IImage> Images { get; }

        int NumberOfImages { get; }

        int SumOfCopies { get; }

        IImage CurrentImage { get; }

        int CurrentImageIndex { get; }

        bool DuringExport { get; }

        string ExportStatus { get; }

        void Save();

        void ExportImages(string exportPath);

        void NextImage();

        void PreviousImage();
    }
}