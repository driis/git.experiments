using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static git.net.AsyncBridge;

namespace git.net
{
    public class GitRepository
    {
        private const string RefKey = "ref: ";
        public string RootPath {get;} 
        public string GitPath { get; }
        private static Encoding Encoding { get; } = Encoding.UTF8;
        public GitRepository(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                throw new ArgumentException($"'{rootPath}' does not exist, is not a directory, or you have insufficient permissions");
            var gitPath = Path.Combine(rootPath, ".git");
            if (!Directory.Exists(gitPath))
                throw new ArgumentException($"Not a valid git repository, '{gitPath}' does not exist.");
            RootPath = rootPath;
            GitPath = gitPath;

        }

        public async Task<Commit> Head()
        {
            string headFile = await ReadGitFile("HEAD");
            if (headFile.StartsWith(RefKey))
            {
                string refFile = headFile.Substring(RefKey.Length);
                if (!Exists(refFile))
                    return null;
                headFile = await ReadGitFile(refFile);
            }
            Hash head = Hash.FromString(headFile);
            return await GetCommit(head);
        }

        public async Task<Commit> GetCommit(Hash id) 
        {
            if (id == null)
                return null;
            string rawCommit = await ReadGitObject(id);
            return Commit.Parse(id, rawCommit);
        }

        private async Task<string> ReadGitObject(Hash obj)
        {
            string fullPath = Path.Combine(GitPath, "objects", obj.StringValue.Substring(0, 2), obj.StringValue.Substring(2));
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                FileOptions.Asynchronous))
            using (var zlib = new Ionic.Zlib.ZlibStream(stream, Ionic.Zlib.CompressionMode.Decompress))
            using (var reader = new StreamReader(zlib))
            {
                return await reader.ReadToEndAsync();
            }
            
        }

        private async Task<string> ReadGitFile(string relative)
        {
            string fullPath = Path.Combine(GitPath, relative);
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                    FileOptions.Asynchronous))
            using (var reader = new StreamReader(stream, Encoding))
            {
                return (await reader.ReadToEndAsync()).Trim();
            }
        }

        /// <summary>
        /// Return history of commits in reverse chronological order (as user would expect them), 
        /// starting from HEAD.
        /// </summary>
        public IEnumerable<Commit> History()
        {
            Commit head = RunSync(Head());
            return History(head);
        }

        /// <summary>
        /// Return history of commits in reverse chronological order (as user would expect them), 
        /// starting from the specified commit.
        /// </summary>
        public IEnumerable<Commit> History(Commit commit)
        {
            return History(commit, commit.Parents.Select(p => RunSync(GetCommit(p))));
        }

        private IEnumerable<Commit> History(Commit current, IEnumerable<Commit> branches)
        {
            yield return current;
            var parents = current.Parents.Select(p => RunSync(GetCommit(p)));
            var candidates = parents.Concat(branches).ToList();
            var next = candidates.OrderByDescending(x => x.Author.Time).FirstOrDefault();
            if (next != null)
            {
                candidates.Remove(next);
                foreach (var commit in History(next, candidates))
                    yield return commit;
            }

        }

        private bool Exists(string relative)
        {
            string fullPath = Path.Combine(GitPath, relative);
            return File.Exists(fullPath);
        }
    }
}