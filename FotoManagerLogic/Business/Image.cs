using System.Linq;
using MetadataExtractor;

namespace FotoManagerLogic.Business
{
    public class Image : IImage
    {
        /// <inheritdoc />
        public Image(string path, int numberOfCopies = 0)
        {
            Path = path;
            NumberOfCopies = numberOfCopies;
        }

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

        /// <inheritdoc />
        public string Orientation
        {
            get
            {
                var result = string.Empty;

                foreach (var directory in ImageMetadataReader.ReadMetadata(Path))
                {
                    var tag = directory.Tags.FirstOrDefault(x => x.Name == "Orientation");
                    if (tag != null)
                    {
                        result = tag.Description;
                        break;
                    }
                }

                return result;
            }
        }

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