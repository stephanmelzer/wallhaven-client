using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Wallhaven.Client.Net;
using Wallhaven.Client.Search;

namespace Wallhaven.Client
{
    public class WallheavenClient
    {
        private readonly Uri _baseUrl = new Uri("https://alpha.wallhaven.cc");
        private IWebClientFactory _webClientFactory;
        private HtmlParser _htmlParser;

        public WallheavenClient(IWebClientFactory webClientFactorybClient)
        {
            _webClientFactory = webClientFactorybClient;
            _htmlParser = new HtmlParser();
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
            if (!searchParam.Page.HasValue || searchParam.Page <= 0) searchParam.Page = 1;

            var searchUri = new Uri(_baseUrl, "search" + searchParam.ToQueryString());
            var wallpaperInfos = GetWallpaperInfosFromPage(searchUri);

            return wallpaperInfos;
        }

        private List<WallpaperInfo> GetWallpaperInfosFromPage(Uri uri)
        {
            var webClient = _webClientFactory.CreateWebClient();
            var result = webClient.DownloadString(uri);

            var dom = _htmlParser.Parse(result);
            var figureElements = dom.QuerySelectorAll("figure");
            var wallpaperInfos = CreateWallpaperInfoFromPage(figureElements);
            AddSourceUri(wallpaperInfos);

            return wallpaperInfos;
        }

        private void AddSourceUri(List<WallpaperInfo> wallpaperInfos)
        {
            Parallel.ForEach(wallpaperInfos, wallpaperInfo =>
            {
                var webClient = _webClientFactory.CreateWebClient();
                var result = webClient.DownloadString(new Uri(_baseUrl, $"wallpaper/{wallpaperInfo.Id}"));
                var dom = _htmlParser.Parse(result);
                var wallpaperElement = dom.QuerySelector("#wallpaper");
                wallpaperInfo.Source = new Uri(_baseUrl, wallpaperElement.GetAttribute("src"));
                wallpaperInfo.FileName = wallpaperInfo.Source.Segments.LastOrDefault();
            });
        }

        private List<WallpaperInfo> CreateWallpaperInfoFromPage(IHtmlCollection<IElement> elements)
        {
            return elements.Select(CreateWallpaperInfoFromPage).ToList();
        }

        private WallpaperInfo CreateWallpaperInfoFromPage(IElement element)
        {
            var wallpaperInfo = new WallpaperInfo
            {
                Id = int.Parse(element.GetAttribute("data-wallpaper-id")),
                Resolution = element.QuerySelector(".wall-res").TextContent,
                Thumbnail = new Uri(element.QuerySelector(">img").GetAttribute("data-src"))
            };

            return wallpaperInfo;
        }
    }
}