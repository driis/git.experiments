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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as GitObject;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(GitObject left, GitObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GitObject left, GitObject right)
        {
            return !Equals(left, right);
        }
    }
}