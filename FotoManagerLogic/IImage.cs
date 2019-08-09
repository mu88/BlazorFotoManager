namespace FotoManagerLogic
{
    public interface IImage
    {
        string Path { get; }

        int NumberOfCopies { get; }

        void Increase();

        void Decrease();

        string FileName { get; }
    }
}