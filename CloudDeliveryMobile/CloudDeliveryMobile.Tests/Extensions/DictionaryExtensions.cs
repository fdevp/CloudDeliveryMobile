using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Tests.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool CompareTo<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> compareDict)
        {
            return dictionary.OrderBy(kvp => kvp.Key)
                    .SequenceEqual(compareDict.OrderBy(kvp => kvp.Key))
                   &&
                   dictionary.OrderBy(kvp => kvp.Value)
                    .SequenceEqual(compareDict.OrderBy(kvp => kvp.Value));

        }
    }
}
