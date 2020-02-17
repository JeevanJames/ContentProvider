#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020 Damian Kulik, Jeevan James

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
    public sealed partial class ContentManager
    {
        private readonly Dictionary<string, ContentSet> _contentSets = new Dictionary<string, ContentSet>(StringComparer.OrdinalIgnoreCase);

        public ContentManager Register(string name, params ContentSource[] sources)
        {
            var contentSet = new ContentSet(name);
            foreach (ContentSource source in sources)
                contentSet.Sources.Add(source);
            _contentSets.Add(name, contentSet);
            return this;
        }

        public ContentManager Register(string name, Action<ContentBuilder> builderSetup)
        {
            if (builderSetup is null)
                throw new ArgumentNullException(nameof(builderSetup));

            var builder = new ContentBuilder();
            builderSetup(builder);
            ContentSource[] sources = builder.Build();

            return Register(name, sources);
        }

        public ContentSet Get(string name)
        {
            return _contentSets.TryGetValue(name, out ContentSet contentSet)
                ? contentSet
                : throw new ArgumentException($"Could not find a content set named {name}.", nameof(name));
        }

        public async Task<string> Get(string name, string entryName)
        {
            ContentSet contentSet = Get(name);
            return await contentSet.GetAsString(entryName).ConfigureAwait(false);
        }
    }

    public sealed partial class ContentManager
    {
        public static ContentManager Global { get; } = new ContentManager();
    }
}
