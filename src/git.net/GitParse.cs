using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace git.net
{
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
            int? length = Parse.String.ToIntNullable(typeAndLength[1]);
            if (length == null)
                throw new ArgumentException($"Not a valid object length: '{typeAndLength[1]}'", nameof(rawContent));

            StringReader reader = new StringReader(rawContent);
            var lines = LinesFromReader(reader);
            var keyValues = lines.Select(x => x.Split(new [] {' '},2))
                .Where(x => x.Length == 2)
                .Select(kv => new { key = kv[0], val = kv[1]})
                .GroupBy(x => x.key)
                .Select(g => new { key = g.Key, val = String.Join("|",g.Select(x => x.val)) })
                .ToDictionary(kv => kv.key, kv => kv.val);

            string body = reader.ReadToEnd();
            return new RawGitObject(type,keyValues, body);
        }

        private static IEnumerable<string> LinesFromReader(TextReader reader)
        {
            string line  = reader.ReadLine();
            while(!String.IsNullOrEmpty(line))
            {
                yield return line;
                line = reader.ReadLine();
            }     
        }

        public static Author GetAuthor(this RawGitObject gitObject, string propName = "author")
        {
            if(gitObject == null)
                throw new ArgumentNullException(nameof(gitObject));
            string authorValue = gitObject.Properties?.ValueOrDefault(propName);            
            string [] authorParts = authorValue.Split('<','>').Select(x => x.Trim()).ToArray();
            if (authorParts.Length != 3)
                throw new ArgumentException($"Cannot parse author: {authorValue}");

            string[] timeParts = authorParts[2].Split(' ');
            var timePart = timeParts[0];
            var time = Parse.String.ToTimestamp(timePart);
            if (time == null)
                throw  new ArgumentException($"Not a valid authored timestamp: {timePart}");
            return new Author(new EmailAddress(authorParts[1]), authorParts[0], time.Value);                                                
        }

        public static Author GetCommitter(this RawGitObject gitObject)
        {
            return GetAuthor(gitObject, "committer");
        }

        public static IEnumerable<Hash> GetParents(this RawGitObject gitObject)
        {
            if(gitObject == null)
                throw new ArgumentNullException(nameof(gitObject));

            var parents = gitObject.Properties.ValueOrDefault("parent")?.Split('|');
            return parents?.Select(Hash.FromString) ?? Enumerable.Empty<Hash>();
        }
    }
}