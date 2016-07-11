using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace git.net
{
    public class Hash
    {
        public const int Size = 20;
        private const int BaseHex = 16;
        private readonly byte[] _raw;
        private static readonly SHA1 sha = SHA1.Create();

        private Hash(byte[] raw)
        {
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            _raw = raw;
            StringValue = _raw.AsHexString();
        }

        public string StringValue { get; }

        public static Hash Calculate(ObjectType type, string content)
        {
            byte [] raw = Encoding.UTF8.GetBytes(content);
            string typeSpec = $"{type.ToString().ToLowerInvariant()} {raw.Length}\0";
            var data = Encoding.UTF8.GetBytes(typeSpec).Concat(raw).ToArray();
            
            return new Hash(sha.ComputeHash(data));
        }

        public static Hash FromString(string rawHash)
        {
            if (rawHash == null)
                throw new ArgumentNullException(nameof(rawHash));
            if (rawHash.Length != Size*2)
                throw new ArgumentException(
                    $"Expected a hexadecimal hash representation with exactly {Size*2} digits " +
                    $"({Size} bytes), got {rawHash?.Length} digits.", nameof(rawHash));
            if (rawHash.Any(ch => ch < '0' || ch > 'f'))
                throw new ArgumentException(
                    $"Expected a hexadecimal hash representation but encountered non-hex digit in value: '{rawHash}'",
                    nameof(rawHash));

            var length = rawHash.Length/2;
            var byteParts = Enumerable.Range(0, length).Select(n => rawHash.Substring(n*2, 2));
            var bytes = byteParts.Select(bs => (byte) Convert.ToInt32(bs, BaseHex));
            return new Hash(bytes.ToArray());
        }

        public override string ToString() => StringValue;

        protected bool Equals(Hash other)
        {
            return string.Equals(StringValue, other.StringValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Hash) obj);
        }

        public override int GetHashCode()
        {
            return StringValue?.GetHashCode() ?? 0;
        }
    }

    public static class HashExtensions
    {
        public static string AsHexString(this byte[] data)
        {
            return string.Join("", data.Select(x => x.ToString("x2")));
        }
    }
}