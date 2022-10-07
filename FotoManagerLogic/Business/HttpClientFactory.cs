using System.Net.Http;

namespace FotoManagerLogic.Business;

public class HttpClientFactory : IHttpClientFactory
{
    /// <inheritdoc />
    public HttpClient CreateClient()
    {
        return new HttpClient();
    }
}