using System;
using System.Collections.ObjectModel;
using FotoManagerLogic;

namespace FotoManager
{
    public class ProjectService : IProjectService
    {
        /// <inheritdoc />
        public ProjectService()
        {
            CurrentProject = new Project("Test",
                                         new Collection<IImage>()
                                         {
                                             new Image(@"D:\Digitalkamera Veronika Mirko\Bilder\Archiv\2017\10_Oktober\P1020173.JPG")
                                         });
        }

        /// <inheritdoc />
        public IProject CurrentProject { get; private set; }

        /// <inheritdoc />
        public IProject LoadProject(string projectPath)
        {
            throw new NotImplementedException();
        }
    }
}