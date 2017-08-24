using System;

namespace Wallhaven.Client.Search
{
    [Flags]
    public enum PurityValue
    {
        Sfw = 4,
        Sketchy = 2
    }

    public class Purity
    {
        public PurityValue Value { get; set; }

        public Purity(PurityValue value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Convert.ToString((int)Value, 2);
        }
    }
}