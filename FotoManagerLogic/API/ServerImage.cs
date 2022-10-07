namespace FotoManagerLogic.API;

public class ServerImage
{
    /// <inheritdoc />
    public ServerImage()
    {
        Id = string.Empty;
        Path = string.Empty;
    }

    public string Id { get; set; }

    public string Path { get; set; }
}