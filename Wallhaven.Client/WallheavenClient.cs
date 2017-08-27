using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Wallhaven.Client.Net;
using Wallhaven.Client.Search;

namespace Wallhaven.Client
{
    public class WallheavenClient
    {
        private readonly Uri _baseUrl = new Uri("https://alpha.wallhaven.cc");
        private IWebClient _webClient;

        public WallheavenClient(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public List<WallpaperInfo> GetLatest(int pageNumber = 1)
        {
            var pageQueryParam = pageNumber > 1 ? $"?page={pageNumber}" : String.Empty;
            var latestUri = new Uri(_baseUrl, "latest" + pageQueryParam);
            var wallpaperInfos = GetWallpaperInfosFromPage(latestUri);

            return wallpaperInfos;
        }

        public List<WallpaperInfo> GetRandom(int pageNumber = 1)
        {
            var pageQueryParam = pageNumber > 1 ? $"?page={pageNumber}" : String.Empty;
            var randomUri = new Uri(_baseUrl, "random" + pageQueryParam);
            var wallpaperInfos = GetWallpaperInfosFromPage(randomUri);

            return wallpaperInfos;
        }

        public List<WallpaperInfo> Search(SearchParameter searchParam)
        {
            if (!searchParam.Page.HasValue || searchParam.Page == 0) searchParam.Page = 1;

            var searchUri = new Uri(_baseUrl, "search" + searchParam.ToQueryString());
            var wallpaperInfos = GetWallpaperInfosFromPage(searchUri);

            return wallpaperInfos;
        }

        private List<WallpaperInfo> GetWallpaperInfosFromPage(Uri uri)
        {
            var result = _webClient.DownloadString(uri);

            var htmlParser = new HtmlParser();
            var dom = htmlParser.Parse(result);
            var figureElements = dom.QuerySelectorAll("figure");
            var wallpaperInfos = CreateWallpaperInfo(figureElements);

            return wallpaperInfos;
        }

        private List<WallpaperInfo> CreateWallpaperInfo(IHtmlCollection<IElement> elements)
        {
            return elements.Select(CreateWallpaperInfo).ToList();
        }

        private WallpaperInfo CreateWallpaperInfo(IElement element)
        {
            var wallpaperInfo = new WallpaperInfo
            {
                Id = int.Parse(element.GetAttribute("data-wallpaper-id")),
                Resolution = element.QuerySelector(".wall-res").TextContent,
                Thumbnail = new Uri(element.QuerySelector(">img").GetAttribute("data-src"))
            };

            // TODO: Add extra property for filename.
            // TODO: Add test for different file types. There are probably more than jpg.
            wallpaperInfo.Source =
                new Uri(String.Format("https://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-{0}.jpg",
                    wallpaperInfo.Id));

            return wallpaperInfo;
        }
    }
}