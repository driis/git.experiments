using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using git.net;

namespace git.net.test.Integration
{
    public class GitRepositoryStructureTests
    {
        private readonly NativeGit _nativeGit;
        private readonly GitRepository _gitRepository;
        public GitRepositoryStructureTests()
        {
            string name = $"native-git-test-{System.Guid.NewGuid().ToString("N")}";
            string repoDirectory = Path.Combine(Path.GetTempPath(), name);
            Console.WriteLine($"Temporary Git repo for testing: {repoDirectory}");
            _nativeGit = new NativeGit(repoDirectory);
            _nativeGit.Init();
            _nativeGit.WriteFile("test.txt", "test content");
            _nativeGit.Add(); 
            _nativeGit.Commit("First test commit.");
             _gitRepository = new GitRepository(repoDirectory);
        }

        ~GitRepositoryStructureTests()
        {
            _nativeGit.Dispose();
        }

        [Fact]
        public async Task CanGetHeadCommit()
        {
            Commit head = await _gitRepository.Head();
            Assert.NotNull(head);
        }
    }

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

        public void Dispose()
        {
            Directory.Delete(_root, true);
        }
    }
}

