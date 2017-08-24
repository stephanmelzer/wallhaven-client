using System;
using System.Collections.Generic;
using System.Text;

namespace Wallhaven.Client
{
    public class WallpaperInfo
    {
        public int Id { get; set; }
        public string Resolution { get; set; }
        public Uri Thumbnail { get; set; }
        public Uri Source { get; set; }
    }
}
