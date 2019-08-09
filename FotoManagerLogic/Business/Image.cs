namespace FotoManagerLogic.Business
{
    public class Image : IImage
    {
        /// <inheritdoc />
        public Image(string path)
        {
            Path = path;
            NumberOfCopies = 0;
        }

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        public string FileName => System.IO.Path.GetFileName(Path);

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
}