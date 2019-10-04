namespace FotoManagerLogic.Business
{
    public interface IImage
    {
        string Path { get; }

        int NumberOfCopies { get; }

        string FileName { get; }

        string Orientation { get; }

        string Id { get; }

        void Increase();

        void Decrease();
    }
}