using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using git.net;
using Xunit.Sdk;
using FluentAssertions;

namespace git.net.test.Unit
{
    public class HashTests
    {
        [Fact]
        public void FromString_CanCreateHashWithoutError()
        {
            const string hashString = "d670460b4b4aece5915caf5c68d12f560a9fe3e4";

            var hash = Hash.FromString(hashString);

            hash.Should().NotBeNull();
            hash.ToString().Should().Be(hashString);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("d670460b4b4aece5915caf5c68d12f560a9fe3e41")]    // length off by one
        [InlineData("d670460b4b4aece5915caf5c68d12f560x9fe3e4")]     // non-hex digit
        public void FromString_ValidatesInput(string badInput)
        {
            Action act = () => Hash.FromString(badInput);

            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(ObjectType.Blob, "hello\n",         "ce013625030ba8dba906f756967f9e9ca394464a")]
        [InlineData(ObjectType.Blob, "test content\n",  "d670460b4b4aece5915caf5c68d12f560a9fe3e4")]
        public void Calculate(ObjectType type, string content, string expected)
        {
            var actual = Hash.Calculate(type, content);
            var expectedHash = Hash.FromString(expected);

            actual.Should().Be(expectedHash);
        }

        [Fact]
        public void CanActuallyDoSha1()
        {
            var sha = SHA1.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes("blob 5\0hello")).AsHexString();

            hash.Should().Be("b6fc4c620b67d95f953a5c1c1230aaab5db5a1b0");
        }
    }
}
