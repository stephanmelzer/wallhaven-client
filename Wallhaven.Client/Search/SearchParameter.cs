using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Wallhaven.Client.Search
{
    public class SearchParameter
    {
        public int? Page { get; set; }
        public Category Categories { get; set; }
        public Purity Purity { get; set; }
        public List<string> Resolutions { get; set; }
        public List<string> Ratios { get; set; }
        public string Sorting { get; set; }
        public string Order { get; set; }
        [QueryParameter("q")]
        public string Query { get; set; }

        public string ToQueryString()
        {
            var queryStringParts = new List<string>();
            var propertyInfos = GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(this);
                if (value != null)
                {
                    var attribute = propertyInfo.GetCustomAttribute(typeof(QueryParameterAttribute)) as QueryParameterAttribute;
                    var name = attribute != null ? attribute.Name : propertyInfo.Name;
                    if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        foreach (var listElement in (IEnumerable)value)
                        {
                            queryStringParts.Add(Uri.EscapeUriString($"{name.ToLower()}={listElement}"));
                        }
                    }
                    else
                    {
                        queryStringParts.Add(Uri.EscapeUriString($"{name.ToLower()}={value}"));
                    }
                }
            }

            return "?" + String.Join("&", queryStringParts);
        }
    }
}
