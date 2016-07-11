using System;
using System.Collections.Generic;
using System.Linq;

namespace git.net
{
    public class Commit : GitObject
    {
        public Commit(Hash id, IEnumerable<Hash> parents) : base(id)
        {
            if (parents == null)
                throw new ArgumentNullException(nameof(parents));

            Parents = parents.ToList();
        }

        public IReadOnlyCollection<Hash> Parents { get; }
    }
}