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
using System.Diagnostics;
using System.Globalization;
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
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentSet"/> class with the specified
        ///     <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content set.</param>
        public ContentSet(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Errors.InvalidContentSetName, nameof(name));
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the content set.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets a content entry as a string value, given its <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content entry.</param>
        /// <returns>The string value of the content entry.</returns>
        /// <exception cref="ContentException">
        ///     Thrown if the content entry is not found in this content set.
        /// </exception>
        public async Task<string> GetAsStringAsync(string name)
        {
            foreach (ContentSource source in Sources)
            {
                (bool success, string? content) = await source.TryLoadAsStringAsync(name)
                    .ConfigureAwait(false);
                if (success)
                    return content!;
            }

            throw new ContentException(string.Format(CultureInfo.CurrentCulture,
                Errors.ContentEntryNotFound, name, Name));
        }

        /// <summary>
        ///     Gets a content entry as a binary value (byte[]), given its <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content entry.</param>
        /// <returns>The byte[] value of the content entry.</returns>
        /// <exception cref="ContentException">
        ///     Thrown if the content entry is not found in this content set.
        /// </exception>
        public async Task<byte[]> GetAsBinaryAsync(string name)
        {
            foreach (ContentSource source in Sources)
            {
                (bool success, byte[]? content) = await source.TryLoadAsBinaryAsync(name)
                    .ConfigureAwait(false);
                if (success)
                    return content!;
            }

            throw new ContentException(string.Format(CultureInfo.CurrentCulture,
                Errors.ContentEntryNotFound, name, Name));
        }

        public string GetAsString(string name)
        {
            foreach (ContentSource source in Sources)
            {
                (bool success, string? content) = source.TryLoadAsString(name);
                if (success)
                    return content!;
            }

            throw new ContentException(string.Format(CultureInfo.CurrentCulture,
                Errors.ContentEntryNotFound, name, Name));
        }

        public byte[] GetAsBinary(string name)
        {
            foreach (ContentSource source in Sources)
            {
                (bool success, byte[]? content) = source.TryLoadAsBinary(name);
                if (success)
                    return content!;
            }

            throw new ContentException(string.Format(CultureInfo.CurrentCulture,
                Errors.ContentEntryNotFound, name, Name));
        }

        /// <summary>
        ///     Gets the list of content sources registered with this content set.
        /// </summary>
        internal List<ContentSource> Sources { get; } = new List<ContentSource>();
    }
}
