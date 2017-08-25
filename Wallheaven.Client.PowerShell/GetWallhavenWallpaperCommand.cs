using System;
using System.Collections.Generic;
using System.Management.Automation;
using Wallhaven.Client;
using Wallhaven.Client.Net;
using Wallhaven.Client.Search;
using System.Linq;

namespace Wallheaven.Client.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "WallhavenWallpaper")]
    public class GetWallhavenWallpaperCommand : Cmdlet
    {
        [Parameter]
        public int PageNumber { get; set; }

        [Parameter(ParameterSetName = "Latest", Mandatory = true)]
        public SwitchParameter Latest { get; set; }

        [Parameter(ParameterSetName = "Random", Mandatory = true)]
        public SwitchParameter Random { get; set; }

        [Parameter(ParameterSetName = "Search", Mandatory = true)]
        public SwitchParameter Search { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [Alias("Sorting")]
        [ValidateSet(Sorting.Random, Sorting.DateAdded, Sorting.Favorites, Sorting.Relevance, Sorting.Views, IgnoreCase = true)]
        public string SearchSorting { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [Alias("Order")]
        [ValidateSet(Order.Ascending, Order.Descending, IgnoreCase = true)]
        public string SearchOrder { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [Alias("Query")]
        public string SearchQuery { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [ValidateSet(nameof(PurityValue.Sfw), nameof(PurityValue.Sketchy), IgnoreCase = true)]
        public List<string> Purity { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [ValidateSet(nameof(CategoryValue.General), nameof(CategoryValue.Anime), nameof(CategoryValue.People), IgnoreCase = true)]
        public List<string> Category { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [ValidateSet(Ratio._10x16, Ratio._16x9, Ratio._16x10, Ratio._21x9, Ratio._32x9, Ratio._48x9, Ratio._4x3, Ratio._5x4, Ratio._9x16, IgnoreCase = true)]
        public List<string> Ratios { get; set; }

        [Parameter(ParameterSetName = "Search")]
        [ValidateSet(Resolution._1280x800, Resolution._1280x960, Resolution._1366x768, Resolution._1440x900,
                     Resolution._1600x1200, Resolution._1600x900, Resolution._1680x1050, Resolution._1920x1080,
                     Resolution._1920x1200, Resolution._2560x1440, Resolution._1280x1024, Resolution._2560x1600,
                     Resolution._3840x1080, Resolution._5120x2880, Resolution._5760x1080, IgnoreCase = true)]
        public List<string> Resolutions { get; set; }

        protected override void ProcessRecord()
        {
            var wallheavenClient = new WallheavenClient(new WebClient());
            List<WallpaperInfo> wallpaperInfos = new List<WallpaperInfo>();

            if (Latest.IsPresent)
            {
                wallpaperInfos = wallheavenClient.GetLatest(PageNumber);
            }
            else if (Random.IsPresent)
            {
                wallpaperInfos = wallheavenClient.GetRandom(PageNumber);
            }
            else if (Search.IsPresent)
            {
                var searchParams = new SearchParameter
                {
                    Page = PageNumber,
                    Categories = GetCategory(),
                    Sorting = SearchSorting,
                    Order = SearchOrder,
                    Query = SearchQuery,
                    Purity = GetPurity(),
                    Ratios = Ratios,
                    Resolutions = Resolutions
                };
                wallpaperInfos = wallheavenClient.Search(searchParams);
            }

            wallpaperInfos.ForEach(WriteObject);
        }

        private Category GetCategory()
        {
            if (Category == null || !Category.Any()) return null;

            CategoryValue categoryValue = (CategoryValue)0;
            foreach (string categoryName in Category)
            {
                categoryValue |= (CategoryValue)Enum.Parse(typeof(CategoryValue), categoryName);
            }

            WriteDebug($"Converted category value: {categoryValue}");
            return new Category(categoryValue);
        }

        private Purity GetPurity()
        {
            if (Purity == null || !Purity.Any()) return null;

            PurityValue purityValue = (PurityValue)0;
            foreach (string purityName in Purity)
            {
                purityValue |= (PurityValue)Enum.Parse(typeof(PurityValue), purityName);
            }

            WriteDebug($"Converted purity value: {purityValue}");
            return new Purity(purityValue);
        }

        //private R GetEnumValue<T, R>(List<string> enumValueNames)
        //{
        //    if (enumValueNames == null || !enumValueNames.Any()) return default(R);

        //    T purityValue = (T)0;
        //    foreach (string purityName in Purity)
        //    {
        //        purityValue |= (T)Enum.Parse(typeof(T), purityName);
        //    }

        //    WriteDebug($"Converted purity value: {purityValue}");
        //    return new R(purityValue);
        //}
    }
}
