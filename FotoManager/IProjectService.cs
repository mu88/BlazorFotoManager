using System.Threading.Tasks;
using FotoManagerLogic;

namespace FotoManager
{
    public interface IProjectService
    {
        IProject CurrentProject { get; }

        bool ProjectLoaded { get; }

        void LoadProject();

        void LoadImages();

        void SaveProject();

        void Export();
    }
}