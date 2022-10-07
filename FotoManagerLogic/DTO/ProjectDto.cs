using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FotoManagerLogic.DTO;

public class ProjectDto
{
    /// <inheritdoc />
    public ProjectDto()
    {
        Images = new Collection<ImageDto>();
    }

    public int CurrentImageIndex { get; set; }

    public IEnumerable<ImageDto> Images { get; set; }
}