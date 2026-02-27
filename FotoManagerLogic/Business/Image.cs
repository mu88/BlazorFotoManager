using System;

namespace FotoManagerLogic.Business;

public class Image : IImage
{
    /// <inheritdoc />
    public Image(string path, int numberOfCopies = 0)
    {
        Path = path;
        NumberOfCopies = numberOfCopies;
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; }

    /// <inheritdoc />
    public string Path { get; }

    /// <inheritdoc />
    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    /// <inheritdoc />
    public int NumberOfCopies { get; private set; }

    /// <inheritdoc />
    public void Increase()
    {
        NumberOfCopies++;
    }

    /// <inheritdoc />
    public void Decrease()
    {
        if (NumberOfCopies > 0)
        {
            NumberOfCopies--;
        }
    }
}
