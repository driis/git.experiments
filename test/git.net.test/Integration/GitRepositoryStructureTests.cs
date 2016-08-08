using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using git.net;
using System.Linq;

namespace git.net.test.Integration
{
    public class GitRepositoryStructureTests : IDisposable
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

        public void Dispose()
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

        [Fact]
        public async Task CanParseCommitParent()
        {
            Commit head = await _gitRepository.Head();
            
            Assert.NotEmpty(head.Parents);    
        }

        [Fact]
        public async Task CanParseAuthoredTime()
        {
            Commit head = await _gitRepository.Head();

            TimeSpan difference = DateTime.Now - head.Author.Time;
            Assert.InRange(difference.TotalMinutes, -5, 5);
        }

        [Fact]
        public async Task CanParseCommitMessage() 
        {
            Commit head = await _gitRepository.Head();

            Assert.Equal("First test commit.\n", head.Message);            
        }

        [Fact]
        public void CanWalkCommitHistory()
        {
            _nativeGit.WriteFileAndCommit("test1.txt", "blah", "commit1");
            _nativeGit.WriteFileAndCommit("test2.txt", "more blah blah", "commit2");

            var history = _gitRepository.History();

            var messages = history.Select(x => x.Message).ToArray();
            Assert.True(messages.SequenceEqual(new [] {"commit2\n", "commit1\n", "First test commit.\n"}));
        }

        [Fact]
        public void CanWalkCommitHistoryWithMultipleParents()
        {
            _nativeGit.NewBranch("test");
            _nativeGit.WriteFileAndCommit("test1.txt", "freom test branch", "01");
            _nativeGit.Checkout("master");
            _nativeGit.WriteFileAndCommit("test2.txt", "from master branch", "02");
            _nativeGit.WriteFileAndCommit("test1.txt", "this shouæld make a conflict", "03");
            _nativeGit.Merge("test");

            _nativeGit.WriteFileAndCommit("test1.txt", "resolved", "04 merge commit");

            var history = _gitRepository.History();
            var messages = history.Select(x => x.Message).ToArray();
            Assert.True(messages.SequenceEqual(new[] { "04 merge commit\n", "03\n", "02\n", "01\n", "First test commmit.\n" }));
        }
    }
}

