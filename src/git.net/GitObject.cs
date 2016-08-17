namespace git.net
{
    public class GitObject
    {
        protected GitObject(Hash id)
        {
            Id = id;
        }

        public Hash Id { get; }

        public override string ToString() => Id.ToString();        

        protected bool Equals(GitObject other)
        {
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((GitObject) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}