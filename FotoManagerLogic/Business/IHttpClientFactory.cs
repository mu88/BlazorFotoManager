using System.Net.Http;

namespace FotoManagerLogic.Business;

public interface IHttpClientFactory
{
    HttpClient CreateClient();
}