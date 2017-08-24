using System;

namespace Wallhaven.Client.Search
{
    [Flags]
    public enum CategoryValue
    {
        General = 4,
        Anime = 2,
        People = 1
    }

    public class Category
    {
        public CategoryValue Value { get; set; }

        public Category(CategoryValue value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Convert.ToString((int)Value, 2);
        }
    }
}