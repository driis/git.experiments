namespace git.net
{
    public class EmailAddress
    {
        public string Value { get; }

        public EmailAddress(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        protected bool Equals(EmailAddress other)
        {
            return string.Equals(Value, other.Value);
        }

        public bool Equals(string email)
        {
            return Value.Equals(email);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((EmailAddress) obj);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public static implicit operator string(EmailAddress rhs)
        {
            return rhs.Value;
        }
    }
}