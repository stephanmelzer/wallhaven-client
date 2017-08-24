using NUnit.Framework;
using Wallhaven.Client.Search;

namespace Wallhaven.Client.Tests.Search
{
    [TestFixture]
    public class CategoryExtensionsTests
    {
        [Test]
        [TestCase(CategoryValue.General, "100")]
        [TestCase(CategoryValue.General | CategoryValue.Anime, "110")]
        [TestCase(CategoryValue.General | CategoryValue.Anime | CategoryValue.People, "111")]
        public void ToString_Categories_ReturnsSelectedCategoriesAsBinaryStringFlags(CategoryValue values, string expectedResult)
        {
            var category = new Category(values);

            Assert.That(category.ToString(), Is.EqualTo(expectedResult));
        }
    }
}
