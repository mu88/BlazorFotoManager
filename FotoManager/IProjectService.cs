using FotoManagerLogic;

namespace FotoManager
{
    public interface IProjectService
    {
        IProject LoadProject(string projectPath);

        IProject CurrentProject { get; }
    }
}