using System;

namespace git.net
{
    internal static class ParseInt
    {
        public static int? ToNullable(string value)
        {
            int x;
            if (!Int32.TryParse(value, out x))
                return null;
            return x;
        }
    }
}