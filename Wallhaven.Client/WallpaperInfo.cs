using System;

namespace Wallhaven.Client
{
    public class WallpaperInfo
    {
        public int Id { get; set; }
        public string Resolution { get; set; }
        public Uri Thumbnail { get; set; }
        public Uri Source { get; set; }
        public string FileName { get; set; }
    }
}
