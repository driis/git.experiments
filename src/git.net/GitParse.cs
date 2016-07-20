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

            string[] timeParts = authorParts[2].Split(' ');
            var timePart = timeParts[0];
            var time = Parse.String.ToTimestamp(timePart);
            if (time == null)
                throw  new ArgumentException($"Not a valid authored timestamp: {timePart}");
            return new Author(new EmailAddress(authorParts[1]), authorParts[0], time.Value);                                                
        }

        public static Hash GetParent(this RawGitObject gitObject)
        {
            if(gitObject == null)
                throw new ArgumentNullException(nameof(gitObject));

            var parent = gitObject.Properties.ValueOrDefault("parent");
            if (parent == null)
                return null;
            return Hash.FromString(parent);
        }

        
    }
}