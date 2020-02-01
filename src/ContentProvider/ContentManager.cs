#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright 2020 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

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
            var contentSet = new ContentSet(name);
            foreach (ContentSource source in sources)
                contentSet.Sources.Add(source);
            _contents.Add(name, contentSet);
        }

        public static ContentSet Get(string name)
        {
            if (!_contents.TryGetValue(name, out ContentSet contentSet))
                throw new ArgumentException($"Could not find content named {name}.", nameof(name));

            return contentSet;
        }

        public static async Task<string> Get(string name, string entryName)
        {
            ContentSet contentSet = Get(name);
            return await contentSet.Get(entryName).ConfigureAwait(false);
        }
    }
}
