namespace FotoManagerLogic.DTO
{
    public class ImageDto
    {
        /// <inheritdoc />
        public ImageDto()
        {
            Path = string.Empty;
        }

        public string Path { get; set; }

        public int NumberOfCopies { get; set; }
    }
}