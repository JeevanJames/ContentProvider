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
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContentProvider
{
    /// <summary>
    ///     Represents a named set of contents, consisting of a primary content source and zero or more
    ///     fallback sources.
    /// </summary>
    [DebuggerDisplay("Content Set {Name}")]
    public sealed class ContentSet : IContentSet
    {
        public ContentSet(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specify a valid name for the content set.", nameof(name));
            Name = name;
        }

        public string Name { get; }

        public async Task<string> Get(string name)
        {
            foreach (ContentSource source in Sources)
            {
                (bool success, string content) = await source.TryLoadAsString(name).ConfigureAwait(false);
                if (success)
                    return content;
            }

            throw new ContentException($"Could not find content entry {name} under the {Name} content.");
        }

        internal List<ContentSource> Sources { get; } = new List<ContentSource>();
    }
}
