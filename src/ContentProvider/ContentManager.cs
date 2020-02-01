using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentProvider
{
    public static class ContentManager
    {
        private static readonly Dictionary<string, Content> _contents = new Dictionary<string, Content>(StringComparer.OrdinalIgnoreCase);

        public static void Register(string name, params ContentSource[] sources)
        {
            var content = new Content(name);
            foreach (ContentSource source in sources)
                content.Sources.Add(source);
            _contents.Add(name, content);
        }

        public static Content Get(string name)
        {
            if (!_contents.TryGetValue(name, out Content bucket))
                throw new ArgumentException($"Could not find content named {name}.", nameof(name));

            return bucket;
        }

        public static async Task<string> Get(string name, string entryName)
        {
            Content content = Get(name);
            return await content.Get(entryName);
        }
    }
}
