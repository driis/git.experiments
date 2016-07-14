using System;
using System.IO;
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
            _nativeGit.Config("user.email john@doe.dk");
            _nativeGit.Config("user.name \"John Doe\"");
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

        [Fact]
        public async Task CanParseCommitAuthor()
        {
            Commit head = await _gitRepository.Head();
            
            Assert.Equal("john@doe.dk", head.Author.Email);    
            Assert.Equal("John Doe", head.Author.Name);
        }
    }
}

