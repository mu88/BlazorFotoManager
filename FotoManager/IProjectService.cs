using FotoManagerLogic.Business;

namespace FotoManager
{
    public interface IProjectService
    {
        IProject CurrentProject { get; }

        void LoadProject();

        void LoadImages();

        void SaveProject();

        void Export();
    }
}