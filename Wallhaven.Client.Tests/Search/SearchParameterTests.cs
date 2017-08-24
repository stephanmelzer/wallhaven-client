using System.Collections.Generic;
using NUnit.Framework;
using Wallhaven.Client.Search;

namespace Wallhaven.Client.Tests.Search
{
    [TestFixture]
    public class SearchParameterTests
    {
        [Test]
        public void ToQueryString_Page_ReturnsQueryStringWithPageNumber()
        {
            const int pageNumber = 2;
            var searchParam = new SearchParameter { Page = 2 };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo($"?page={pageNumber}"));
        }

        [Test]
        public void ToQueryString_Sorting_ReturnsQueryStringWithSorting()
        {
            string sorting = Sorting.DateAdded;
            var searchParam = new SearchParameter { Sorting = sorting };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo($"?sorting={sorting}"));
        }

        [Test]
        public void ToQueryString_Order_ReturnsQueryStringWithOrder()
        {
            string order = Order.Ascending;
            var searchParam = new SearchParameter { Order = order };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo($"?order={order}"));
        }

        [Test]
        public void ToQueryString_Query_ReturnsQueryStringWithQuery()
        {
            string query = "test";
            var searchParam = new SearchParameter { Query = query };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo($"?q={query}"));
        }

        [Test]
        public void ToQueryString_GeneralCategory_ReturnsQueryStringWithGeneralCategory()
        {
            var category = new Category(CategoryValue.General);
            var searchParam = new SearchParameter { Categories = category };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo($"?categories={category}"));
        }

        [Test]
        public void ToQueryString_SfwPurity_ReturnsQueryStringWithSfwPurity()
        {
            var purity = new Purity(PurityValue.Sfw);
            var searchParam = new SearchParameter { Purity = purity };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo($"?purity={purity}"));
        }

        [Test]
        public void ToQueryString_RatioList_ReturnsQueryStringWithMultipleRatios()
        {
            var ratios = new List<string> { Ratio._4x3, Ratio._16x10 };
            var searchParam = new SearchParameter { Ratios = ratios };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Does.StartWith("?"));
            Assert.That(queryString, Contains.Substring($"ratios={Ratio._4x3}"));
            Assert.That(queryString, Contains.Substring($"ratios={Ratio._16x10}"));
        }

        [Test]
        public void ToQueryString_ResolutionList_ReturnsQueryStringWithMultipleResolutions()
        {
            var resolutions = new List<string> { Resolution._1280x800, Resolution._1280x960 };
            var searchParam = new SearchParameter { Resolutions = resolutions };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Does.StartWith("?"));
            Assert.That(queryString, Contains.Substring($"resolutions={Resolution._1280x800}"));
            Assert.That(queryString, Contains.Substring($"resolutions={Resolution._1280x960}"));
        }

        [Test]
        public void ToQueryString_MultipleParameters_ReturnsQueryStringWithSelectedParameters()
        {
            int page = 2;
            var category = new Category(CategoryValue.General);
            var sorting = Sorting.Views;

            var searchParam = new SearchParameter
            {
                Page = page,
                Categories = category,
                Sorting = sorting
            };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Does.StartWith("?"));
            Assert.That(queryString, Contains.Substring($"page={page}"));
            Assert.That(queryString, Contains.Substring($"categories={category}"));
            Assert.That(queryString, Contains.Substring($"sorting={sorting}"));
        }

        [Test]
        public void ToQueryString_WithNotEscapedCharacters_ReturnsUrlEscapedString()
        {
            var searchParam = new SearchParameter
            {
                Query = "Game of Thrones"
            };

            var queryString = searchParam.ToQueryString();

            Assert.That(queryString, Is.EqualTo("?q=Game%20of%20Thrones"));
        }
    }
}
