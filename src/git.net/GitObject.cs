namespace git.net
{
    public class GitObject
    {
        protected GitObject(Hash id)
        {
            Id = id;
        }

        public Hash Id { get; }
    }
}