namespace FotoManagerLogic.API;

public interface IServerImageRepository
{
    void Add(ServerImage entry);

    string GetPath(string id);
}