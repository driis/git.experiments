using System;
using dr.git.net;

namespace dr.git.net.console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(new GitRepository(".").RootPath);
        }
    }
}
