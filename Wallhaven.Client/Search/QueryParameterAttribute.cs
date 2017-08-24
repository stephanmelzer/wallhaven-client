using System;

namespace Wallhaven.Client.Search
{
    public class QueryParameterAttribute : Attribute
    {
        public string Name { get; private set; }

        public QueryParameterAttribute(string queryParameterName)
        {
            Name = queryParameterName;
        }
    }
}