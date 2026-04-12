namespace FotoManagerLogic.DTO;

public record ImageDto
{
    public string Path { get; init; } = string.Empty;

    public int NumberOfCopies { get; init; }
}
