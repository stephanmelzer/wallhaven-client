using System;

namespace Wallhaven.Client.Net
{
    public interface IWebClient
    {
        string DownloadString(Uri address);
    }
}