using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace git.net
{
    public class Commit : GitObject
    {
        public Author Author { get; }

        public Commit(Hash id, Author author, IEnumerable<Hash> parents) : base(id)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author));
            if (parents == null)
                throw new ArgumentNullException(nameof(parents));

            Author = author;
            Parents = parents.ToList();
        }

        public IReadOnlyCollection<Hash> Parents { get; }

        public static Commit Parse(Hash id, string rawCommit)
        {
            var gitObject = GitParse.ParseObject(rawCommit);
            if (gitObject.Type != ObjectType.Commit)
                throw new ArgumentException($"Not {id} is not a Commit object, but a {gitObject.Type}.");

            return new Commit(id, gitObject.GetAuthor(), new []{gitObject.GetParent()});
        }
    }

    internal static class GitParse 
    {
        public static RawGitObject ParseObject(string rawContent)
        {
            string[] parts = rawContent.Split('\0');
            if (parts.Length != 2)
                throw new ArgumentException($"Not a valid Git Object: '{rawContent}'", nameof(rawContent));
            string[] typeAndLength = parts[0].Split(' ');
            string typeSpecifier = typeAndLength[0];
            ObjectType type;
            if (!Enum.TryParse(typeSpecifier, ignoreCase:true, result: out type))
                throw new ArgumentException($"Not a valid object type: '{typeSpecifier}'", nameof(rawContent));
            int? length = ParseInt.ToNullable(typeAndLength[1]);
            if (length == null)
                throw new ArgumentException($"Not a valid object length: '{typeAndLength[1]}'", nameof(rawContent));

            StringReader reader = new StringReader(rawContent);
            string prop;
            Dictionary<string,string> properties = new Dictionary<string, string>();
            while ((prop = reader.ReadLine()) != null)
            {
                var kv = prop.Split(new[] {' '}, 2);
                if (kv.Length != 2)
                    break;
                properties.Add(kv[0], kv[1]);
            }

            string body = reader.ReadToEnd();
            return new RawGitObject(type,properties, body);
        }

        public static Author GetAuthor(this RawGitObject gitObject)
        {
            if(gitObject == null)
                throw new ArgumentNullException(nameof(gitObject));
            string authorValue = gitObject.Properties?.ValueOrDefault("author");
            string [] authorParts = authorValue.Split('<','>').Select(x => x.Trim()).ToArray();
            if (authorParts.Length != 3)
                throw new ArgumentException($"Cannot parse author: {authorValue}");
            return new Author(new EmailAddress(authorParts[1]), authorParts[0]);                                                
        }

        public static Hash GetParent(this RawGitObject gitObject)
        {
            if(gitObject == null)
                throw new ArgumentNullException(nameof(gitObject));
            
            return Hash.FromString(gitObject.Properties.ValueOrDefault("parent"));
        }

        
    }

    internal class RawGitObject
    {
        public string Body {get;}
        public Dictionary<string,string> Properties { get; }
        public ObjectType Type {get;}

        public RawGitObject(ObjectType type, Dictionary<string, string> properties, string body)
        {
            Body = body;
            Properties = properties;
            Type = type;
        }
    }

    internal static class ParseInt
    {
        public static int? ToNullable(string value)
        {
            int x;
            if (!Int32.TryParse(value, out x))
                return null;
            return x;
        }
    }

    internal static class LookupExtensions
    {
        public static TValue ValueOrDefault<TKey,TValue>(this Dictionary<TKey,TValue> dic, TKey key)
        {
            TValue value;
            if (dic.TryGetValue(key, out value))
                return value;
            return default(TValue);
        }
    }
}