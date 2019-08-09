namespace FotoManagerLogic
{
    public interface IImage
    {
        string Path { get; }

        int NumberOfCopies { get; }

        string FileName { get; }

        void Increase();

        void Decrease();
    }
}