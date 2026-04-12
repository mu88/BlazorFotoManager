using System.Collections.Generic;

namespace FotoManagerLogic.DTO;

public record ProjectDto
{
    public int CurrentImageIndex { get; init; }

    public IEnumerable<ImageDto> Images { get; init; } = [];
}
