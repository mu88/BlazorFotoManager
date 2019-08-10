using System.Collections.Generic;

namespace FotoManagerLogic.DTO
{
    public class ProjectDto
    {
        public int CurrentImageIndex { get; set; }

        public IEnumerable<ImageDto> Images { get; set; }
    }
}