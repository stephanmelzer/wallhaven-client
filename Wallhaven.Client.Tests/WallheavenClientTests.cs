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
        [Test]
        public void GetLatest_NoPageNumber_ReturnsTheImagesOfFirstPage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Wallhaven.Client.Tests.TestData.PageData.html");
            StreamReader reader = new StreamReader(stream);
            var testPageData = reader.ReadToEnd();

            var webClient = new Mock<IWebClient>();
            webClient.Setup(x => x.DownloadString(It.IsAny<Uri>())).Returns(testPageData);

            var client = new WallheavenClient(webClient.Object);

            List<WallpaperInfo> latestWallpapers = client.GetLatest();

            Assert.That(latestWallpapers, Is.Not.Empty);
            Assert.That(latestWallpapers.All(IsValidWallpaperInfo), Is.True);
        }

        [Test]
        public void GetLatest_NoPageNumber_CallsUrlWithoutPageNumber()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(x => x.DownloadString(new Uri("https://alpha.wallhaven.cc/latest")));
            var client = new WallheavenClient(webClient.Object);
            client.GetLatest();

            webClient.VerifyAll();
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void GetLatest_PageNumber_CallsUrlWithPageNumber(int pageNumber)
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(x => x.DownloadString(new Uri($"https://alpha.wallhaven.cc/latest?page={pageNumber}")));
            var client = new WallheavenClient(webClient.Object);
            client.GetLatest(pageNumber);

            webClient.VerifyAll();
        }

        [Test]
        public void GetRandom_NoPageNumber_ReturnsTheImagesOfFirstPage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Wallhaven.Client.Tests.TestData.PageData.html");
            StreamReader reader = new StreamReader(stream);
            var testPageData = reader.ReadToEnd();

            var webClient = new Mock<IWebClient>();
            webClient.Setup(x => x.DownloadString(It.IsAny<Uri>())).Returns(testPageData);

            var client = new WallheavenClient(webClient.Object);

            List<WallpaperInfo> latestWallpapers = client.GetRandom();

            Assert.That(latestWallpapers, Is.Not.Empty);
            Assert.That(latestWallpapers.All(IsValidWallpaperInfo), Is.True);
        }

        [Test]
        public void GetRandom_NoPageNumber_CallsUrlWithoutPageNumber()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(x => x.DownloadString(new Uri("https://alpha.wallhaven.cc/random")));
            var client = new WallheavenClient(webClient.Object);
            client.GetRandom();

            webClient.VerifyAll();
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void GetRandom_NoPageNumber_CallsUrlWithPageNumber(int pageNumber)
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(x => x.DownloadString(new Uri($"https://alpha.wallhaven.cc/random?page={pageNumber}")));
            var client = new WallheavenClient(webClient.Object);
            client.GetRandom(pageNumber);

            webClient.VerifyAll();
        }

        [Test]
        public void Search_SearchParams_ReturnsWallpaperInfos()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Wallhaven.Client.Tests.TestData.PageData.html");
            StreamReader reader = new StreamReader(stream);
            var testPageData = reader.ReadToEnd();

            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.DownloadString(It.IsAny<Uri>())).Returns(testPageData);
            var wallhavenClient = new WallheavenClient(webClient.Object);

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

            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.DownloadString(new Uri($"https://alpha.wallhaven.cc/search?page={searchParam.Page}&sorting={searchParam.Sorting}")));
            var wallhavenClient = new WallheavenClient(webClient.Object);

            wallhavenClient.Search(searchParam);

            webClient.VerifyAll();
        }

        private bool IsValidWallpaperInfo(WallpaperInfo wallpaperInfo)
        {
            var isValid = true;

            isValid &= wallpaperInfo.Id > 0;
            isValid &= !String.IsNullOrWhiteSpace(wallpaperInfo.Resolution);
            isValid &= wallpaperInfo.Thumbnail != null && !String.IsNullOrWhiteSpace(wallpaperInfo.Thumbnail.ToString());
            isValid &= wallpaperInfo.Source != null && !String.IsNullOrWhiteSpace(wallpaperInfo.Source.ToString());

            return isValid;
        }
    }
}
