using System.Collections.Generic;

namespace git.net
{
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
}