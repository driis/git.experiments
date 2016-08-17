using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace git.net
{
    public class Commit : GitObject
    {
        public Author Author { get; }

        public string Message { get; }
        public Commit(Hash id, Author author, IEnumerable<Hash> parents, string message) : base(id)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author));
            if (parents == null)
                throw new ArgumentNullException(nameof(parents));

            Author = author;
            Parents = parents.ToList();
            Message = message;
        }

        public IReadOnlyCollection<Hash> Parents { get; }

        public static Commit Parse(Hash id, string rawCommit)
        {
            var gitObject = GitParse.ParseObject(rawCommit);
            if (gitObject.Type != ObjectType.Commit)
                throw new ArgumentException($"Not {id} is not a Commit object, but a {gitObject.Type}.");

            return new Commit(id, gitObject.GetAuthor(), gitObject.GetParents(), gitObject.Body);
        }


    }
}