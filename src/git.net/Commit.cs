using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace git.net
{
    public class Commit : GitObject
    {
        public Author Author { get; }
        public Author Committer { get; }
        public string Message { get; }

        public Commit(Hash id, Author author, Author committer, IEnumerable<Hash> parents, string message) : base(id)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Parents = parents?.ToList() ?? throw new ArgumentNullException(nameof(parents));
            Committer = committer ?? throw new ArgumentNullException(nameof(committer));
            Message = message;
        }

        public IReadOnlyCollection<Hash> Parents { get; }

        public static Commit Parse(Hash id, string rawCommit)
        {
            var gitObject = GitParse.ParseObject(rawCommit);
            if (gitObject.Type != ObjectType.Commit)
                throw new ArgumentException($"Not {id} is not a Commit object, but a {gitObject.Type}.");
            
            return new Commit(id, gitObject.GetAuthor(), gitObject.GetCommitter(), gitObject.GetParents(), gitObject.Body);
        }


    }
}