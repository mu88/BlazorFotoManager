namespace FotoManagerLogic.Business
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