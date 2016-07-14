namespace git.net
{
    public class Author
    {
        public Author(EmailAddress email, string name)
        {
            Email = email;
            Name = name;
        }

        public EmailAddress Email { get; }
        public string Name { get; }

        protected bool Equals(Author other)
        {
            return Equals(Email, other.Email) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Author) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Email?.GetHashCode() ?? 0)*397) ^ (Name?.GetHashCode() ?? 0);
            }
        }

        public override string ToString()
        {
            return $"Email: {Email}, Name: {Name}";
        }
    }
}