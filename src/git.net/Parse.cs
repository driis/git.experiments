using System;
using System.Net.Http.Headers;

namespace git.net
{
    internal static class Parse
    {
        public static class String
        {
            public static int? ToIntNullable(string value)
            {
                int x;
                if (!Int32.TryParse(value, out x))
                    return null;
                return x;
            }

            public static Int64? ToInt64Nullable(string value)
            {
                Int64 x;
                if (!Int64.TryParse(value, out x))
                    return null;
                return x;
            }

            public static DateTimeOffset? ToTimestamp(string timePart)
            {
                long? sinceEpoch = ToInt64Nullable(timePart);
                if (sinceEpoch == null)
                    return null;
                return DateTimeOffset.FromUnixTimeSeconds(sinceEpoch.Value);
            }
        }
    }
}