using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace git.net.test.Integration
{
    public class NativeGit : IDisposable
    {
        private const int NativeTimeout = 500;
        private readonly string _root;

        public NativeGit(string root)
        {
            if (!Path.IsPathRooted(root))
                throw new ArgumentException($"Expected full path, got {_root}", nameof(root));
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            _root = root;
        }

        public void Init()
        {
            RunGitCommand("init");
        }

        public void Add(string path = ".")
        {
            RunGitCommand($"add {path}");
        }

        public void Commit(string message)
        {
            RunGitCommand($"commit -m \"{message}\"");
        }

        public void Config(string configuration)
        {
            RunGitCommand($"config {configuration}");
        }

        private void RunGitCommand(string gitArguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo("git", gitArguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = _root
            };
            Process p = Process.Start(psi);
            if (!p.WaitForExit(NativeTimeout))
                throw new Exception("Native git operation timed out.");

            Console.WriteLine(p.StandardOutput.ReadToEnd());
            Console.WriteLine(p.StandardError.ReadToEnd());
            if (p.ExitCode != 0)
            {
                throw new Exception($"Process exited with non-zero exitcode: {p.ExitCode}");
            }
        }

        public void WriteFile(string path, string testContent)
        {
            File.WriteAllText(Path.Combine(_root, path), testContent, Encoding.UTF8);
        }

        public void WriteFileAndCommit(string path, string testContent, string commitMessage)
        {
            WriteFile(path, testContent);
            Add(path);
            Commit(commitMessage);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_root, true);    
            }
            catch(UnauthorizedAccessException) { /* empty catch by design */ }
        }

        public void NewBranch(string branchName)
        {
            RunGitCommand($"checkout -b {branchName}");
        }

        public void Checkout(string branchName)
        {
            RunGitCommand($"checkout {branchName}");
        }

        public void Merge(string branchName)
        {
            RunGitCommand($"merge {branchName}");
        }
    }
}