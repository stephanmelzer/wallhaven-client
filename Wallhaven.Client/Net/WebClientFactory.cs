namespace Wallhaven.Client.Net
{
    public class WebClientFactory : IWebClientFactory
    {
        public IWebClient CreateWebClient()
        {
            return new WebClient();
        }
    }
}
