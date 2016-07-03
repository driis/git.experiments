using System;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1() 
        {
            new dr.git.net.GitRepository(".");
        }
    }
}
