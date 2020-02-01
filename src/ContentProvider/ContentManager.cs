using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentProvider
{
    public static class ContentManager
    {
        private static readonly Dictionary<string, ContentSet> _contents = new Dictionary<string, ContentSet>(StringComparer.OrdinalIgnoreCase);

        public static void Register(string name, params ContentSource[] sources)
        {
            var content = new ContentSet(name);
            foreach (ContentSource source in sources)
                content.Sources.Add(source);
            _contents.Add(name, content);
        }

        public static ContentSet Get(string name)
        {
            if (!_contents.TryGetValue(name, out ContentSet content))
                throw new ArgumentException($"Could not find content named {name}.", nameof(name));

            return content;
        }

        public static async Task<string> Get(string name, string entryName)
        {
            ContentSet content = Get(name);
            return await content.Get(entryName).ConfigureAwait(false);
        }
    }
}
