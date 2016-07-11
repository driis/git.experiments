using System;
using git.net;

namespace git.net.console
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
