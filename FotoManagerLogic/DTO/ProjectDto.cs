using System.Collections.Generic;

namespace FotoManagerLogic.DTO
{
    public class ProjectDto
    {
        public string ProjectPath { get; set; }

        public int CurrentImageIndex { get; set; }

        public IEnumerable<ImageDto> Images { get; set; }
    }
}