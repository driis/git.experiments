using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using git.net;
using System.Linq;
using System.Threading;
using FluentAssertions;

namespace git.net.test.Integration
{
    public class GitRepositoryStructureTests : IDisposable
    {
        private readonly NativeGit _nativeGit;
        private readonly GitRepository _gitRepository;
        public GitRepositoryStructureTests()
        {
            string name = $"native-git-test-{System.Guid.NewGuid():N}";
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
            head.Should().NotBeNull();
        }

        [Fact]
        public async Task CanParseCommitAuthor()
        {
            Commit head = await _gitRepository.Head();
            
            head.Author.Email.Value.Should().Be("john@doe.dk");
            head.Author.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task CanParseCommitParent()
        {
            _nativeGit.WriteFileAndCommit("test.txt", "bogus", "a commit message");
            Commit head = await _gitRepository.Head();
            
            head.Parents.Should().NotBeEmpty();    
        }

        [Fact]
        public async Task ParentsCanBeEmpty()
        {
            Commit head = await _gitRepository.Head();

            head.Parents.Should().BeEmpty();            
        }

        [Fact]
        public async Task CanParseAuthoredTime()
        {
            Commit head = await _gitRepository.Head();

            TimeSpan difference = DateTime.Now - head.Author.Time;

            head.Author.Time.Should().BeCloseTo(DateTime.Now, precision: 5000);
        }

        [Fact]
        public async Task CanParseCommitMessage() 
        {
            Commit head = await _gitRepository.Head();

            head.Message.Should().Be("First test commit.\n");            
        }

        [Fact]
        public void CanWalkCommitHistory()
        {
            _nativeGit.WriteFileAndCommit("test1.txt", "blah", "commit1");
            _nativeGit.WriteFileAndCommit("test2.txt", "more blah blah", "commit2");

            var history = _gitRepository.History();

            var messages = history.Select(x => x.Message).ToArray();
            messages.Should().Equal(new [] {"commit2\n", "commit1\n", "First test commit.\n"});
        }

        [Fact]
        public void CanWalkCommitHistoryWithMultipleParents()
        {
            _nativeGit.NewBranch("test");
            _nativeGit.WriteFileAndCommit("test1.txt", "from test branch", "01");
            _nativeGit.Checkout("master");
            Thread.Sleep(1000);
            _nativeGit.WriteFileAndCommit("test2.txt", "from master branch", "02");
            Thread.Sleep(1000);
            _nativeGit.WriteFileAndCommit("test1.txt", "this should make a conflict", "03");
            _nativeGit.Merge("test", throwOnConflicts:false);

            _nativeGit.WriteFileAndCommit("test1.txt", "resolved", "04 merge commit");

            var history = _gitRepository.History();
            var messages = history.Select(x => x.Message).ToArray();
            messages.Should().Equal(new[] { "04 merge commit\n", "03\n", "02\n", "01\n", "First test commit.\n" });
        }
    }
}

