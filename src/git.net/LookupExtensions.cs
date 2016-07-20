using System.Collections.Generic;

namespace git.net
{
    internal static class LookupExtensions
    {
        public static TValue ValueOrDefault<TKey,TValue>(this Dictionary<TKey,TValue> dic, TKey key)
        {
            TValue value;
            if (dic.TryGetValue(key, out value))
                return value;
            return default(TValue);
        }
    }
}