using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Wallhaven.Client.Net;
using Wallhaven.Client.Search;

namespace Wallhaven.Client.Tests
{
    [TestFixture]
    public class WallheavenClientTests
    {
        private Dictionary<string, string> _resourceFile = new Dictionary<string, string>();
        private const string WallpaperListPage = "Wallhaven.Client.Tests.TestData.WallpaperListPage.html";
        private const string WallpaperDetailPage = "Wallhaven.Client.Tests.TestData.WallpaperDetailPage.html";

        [OneTimeSetUp]
        public void Init()
        {
            LoadTestData();
        }

        [Test]
        public void GetLatest_NoPageNumber_ReturnsTheImagesOfFirstPage()
        {
            var webClientMock = CreateWebClientMock();
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var client = new WallhavenClient(webClientFactory);

            List<WallpaperInfo> latestWallpapers = client.GetLatest();

            Assert.That(latestWallpapers, Is.Not.Empty);
            Assert.That(latestWallpapers.All(IsValidWallpaperInfo), Is.True);
        }

        [Test]
        public void GetLatest_NoPageNumber_CallsUrlWithoutPageNumber()
        {
            var webClientMock = CreateWebClientMock(new Uri("https://alpha.wallhaven.cc/latest"));
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var client = new WallhavenClient(webClientFactory);
            client.GetLatest();

            webClientMock.VerifyAll();
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void GetLatest_PageNumber_CallsUrlWithPageNumber(int pageNumber)
        {
            var webClientMock = CreateWebClientMock(new Uri($"https://alpha.wallhaven.cc/latest?page={pageNumber}"));
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var client = new WallhavenClient(webClientFactory);
            client.GetLatest(pageNumber);

            webClientMock.VerifyAll();
        }

        [Test]
        public void GetRandom_NoPageNumber_ReturnsTheImagesOfFirstPage()
        {
            var webClientMock = CreateWebClientMock();
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var client = new WallhavenClient(webClientFactory);

            List<WallpaperInfo> latestWallpapers = client.GetRandom();

            Assert.That(latestWallpapers, Is.Not.Empty);
            Assert.That(latestWallpapers.All(IsValidWallpaperInfo), Is.True);
        }

        [Test]
        public void GetRandom_NoPageNumber_CallsUrlWithoutPageNumber()
        {
            var webClientMock = CreateWebClientMock(new Uri("https://alpha.wallhaven.cc/random"));
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var client = new WallhavenClient(webClientFactory);

            client.GetRandom();

            webClientMock.VerifyAll();
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void GetRandom_NoPageNumber_CallsUrlWithPageNumber(int pageNumber)
        {
            var webClientMock = CreateWebClientMock(new Uri($"https://alpha.wallhaven.cc/random?page={pageNumber}"));
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var client = new WallhavenClient(webClientFactory);

            client.GetRandom(pageNumber);

            webClientMock.VerifyAll();
        }

        [Test]
        public void Search_SearchParams_ReturnsWallpaperInfos()
        {
            var webClientMock = CreateWebClientMock();
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var wallhavenClient = new WallhavenClient(webClientFactory);
            var searchParam = new SearchParameter
            {
                Sorting = Sorting.Views,
                Order = Order.Descending,
                Categories = new Category(CategoryValue.General)
            };

            List<WallpaperInfo> wallpaperInfos = wallhavenClient.Search(searchParam);

            Assert.That(wallpaperInfos, Is.Not.Empty);
            Assert.That(wallpaperInfos.All(IsValidWallpaperInfo), Is.True);
        }

        [Test]
        public void Search_SearchParams_CallsSearchUrlWithQueryParameter()
        {
            var searchParam = new SearchParameter
            {
                Page = 2,
                Sorting = Sorting.Views
            };
            var uri = new Uri($"https://alpha.wallhaven.cc/search?page={searchParam.Page}&sorting={searchParam.Sorting}");
            var webClientMock = CreateWebClientMock(uri);
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var wallhavenClient = new WallhavenClient(webClientFactory);

            wallhavenClient.Search(searchParam);

            webClientMock.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase(0)]
        [TestCase(-1)]
        public void Search_SearchParamsWithIllegalPageNumber_CallsSearchUrlWithPageNumberOne(int? pageNumber)
        {
            var searchParam = new SearchParameter
            {
                Page = pageNumber
            };

            var webClientMock = CreateWebClientMock(new Uri("https://alpha.wallhaven.cc/search?page=1"));
            var webClientFactory = CreateWebClientFactory(webClientMock.Object);
            var wallhavenClient = new WallhavenClient(webClientFactory);

            wallhavenClient.Search(searchParam);

            webClientMock.VerifyAll();
        }

        private void LoadTestData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (string resourceName in resourceNames)
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                StreamReader reader = new StreamReader(stream);
                var resourceContent = reader.ReadToEnd();

                _resourceFile.Add(resourceName, resourceContent);
            }
        }

        private IWebClientFactory CreateWebClientFactory(IWebClient webClientMock)
        {
            var webClientFactoryMock = new Mock<IWebClientFactory>();
            webClientFactoryMock.Setup(factory => factory.CreateWebClient()).Returns(webClientMock);

            return webClientFactoryMock.Object;
        }

        private Mock<IWebClient> CreateWebClientMock(Uri wallpaperListPageUri = null)
        {
            var wallpaperListPageData = _resourceFile[WallpaperListPage];
            var wallpaperDetailPageData = _resourceFile[WallpaperDetailPage];

            var webClientMock = new Mock<IWebClient>();
            webClientMock.When(() => wallpaperListPageUri == null)
                         .Setup(webClient => webClient.DownloadString(It.IsAny<Uri>()))
                         .Returns(wallpaperListPageData);

            webClientMock.When(() => wallpaperListPageUri != null)
                         .Setup(webClient => webClient.DownloadString(wallpaperListPageUri))
                         .Returns(wallpaperListPageData);

            webClientMock.Setup(webClient => webClient.DownloadString(It.Is<Uri>(uri => uri.AbsolutePath.Contains("/wallpaper"))))
                         .Returns(wallpaperDetailPageData);

            return webClientMock;
        }

        private bool IsValidWallpaperInfo(WallpaperInfo wallpaperInfo)
        {
            var isValid = true;

            isValid &= wallpaperInfo.Id > 0;
            isValid &= !String.IsNullOrWhiteSpace(wallpaperInfo.Resolution);
            isValid &= wallpaperInfo.Thumbnail != null &&
                       !String.IsNullOrWhiteSpace(wallpaperInfo.Thumbnail.ToString()) &&
                       wallpaperInfo.Thumbnail.Scheme.StartsWith("http");
            isValid &= wallpaperInfo.Source != null &&
                       !String.IsNullOrWhiteSpace(wallpaperInfo.Source.ToString()) &&
                       wallpaperInfo.Source.Scheme.StartsWith("http");
            isValid &= !String.IsNullOrWhiteSpace(wallpaperInfo.FileName);

            if (wallpaperInfo.Source != null && wallpaperInfo.FileName != null)
            {
                isValid &= wallpaperInfo.Source.Segments.LastOrDefault() == wallpaperInfo.FileName;
            }

            return isValid;
        }
    }
}
