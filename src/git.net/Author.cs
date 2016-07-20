using System;

namespace git.net
{
    public class Author
    {
        public Author(EmailAddress email, string name, DateTimeOffset time)
        {
            Email = email;
            Name = name;
            Time = time;
        }

        public EmailAddress Email { get; }
        public string Name { get; }
        public DateTimeOffset Time { get; }
       
        public override string ToString()
        {
            return $"Email: {Email}, Name: {Name}";
        }
    }
}