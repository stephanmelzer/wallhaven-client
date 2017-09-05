using System;
using System.Linq;
using AngleSharp.Parser.Html;
using Wallhaven.Client.Net;

namespace Wallhaven.Client
{
    public class WallpaperInfo
    {
        private readonly Uri _baseUrl = new Uri("https://alpha.wallhaven.cc");
        private IWebClientFactory _webClientFactory;

        private Uri _source;
        private string _fileName;

        public int Id { get; set; }

        public string Resolution { get; set; }

        public Uri Thumbnail { get; set; }

        public Uri Source
        {
            get
            {
                if (_source == null) FetchRemainingData();
                return _source;
            }
            set => _source = value;
        }

        public string FileName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_fileName)) FetchRemainingData();
                return _fileName;
            }
            set => _fileName = value;
        }

        public WallpaperInfo()
        {
            _webClientFactory = new WebClientFactory();
        }

        public WallpaperInfo(IWebClientFactory webClientFactory)
        {
            _webClientFactory = webClientFactory;
        }

        private void FetchRemainingData()
        {
            var webClient = _webClientFactory.CreateWebClient();
            var result = webClient.DownloadString(new Uri(_baseUrl, $"wallpaper/{Id}"));
            var htmlParser = new HtmlParser();
            var dom = htmlParser.Parse(result);
            var wallpaperElement = dom.QuerySelector("#wallpaper");
            Source = new Uri(_baseUrl, wallpaperElement.GetAttribute("src"));
            FileName = Source.Segments.LastOrDefault();
        }
    }
}
