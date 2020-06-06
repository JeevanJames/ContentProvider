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
using System.Text;
using System.Threading.Tasks;

namespace ContentProvider
{
    public abstract class ContentSource
    {
        /// <summary>
        ///     Attempts to load an item from the content source as a string, given its
        ///     <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content item to retrieve.</param>
        /// <returns>
        ///     A tuple indicating whether the content item could be loaded, and if so, the string
        ///     content itself.
        /// </returns>
        public virtual async Task<(bool success, string? content)> TryLoadAsString(string name)
        {
            (bool success, byte[]? content) = await TryLoadAsBinary(name).ConfigureAwait(false);
            if (!success)
                return (false, null);

            string contentString = Encoding.UTF8.GetString(content);
            return (true, contentString);
        }

        /// <summary>
        ///     Attempts to load an item from the content source as a byte array, given its
        ///     <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content item to retrieve.</param>
        /// <returns>
        ///     A tuple indicating whether the content item could be loaded, and if so, the byte
        ///     array content itself.
        /// </returns>
        public abstract Task<(bool success, byte[]? content)> TryLoadAsBinary(string name);
    }

    /// <summary>
    ///     Represents a source from which to retrieve content.
    ///     <para/>
    ///     Examples include assembly embedded resources, files, web sites, etc.
    /// </summary>
    public abstract class ContentSource<TOptions> : ContentSource
        where TOptions : ContentSourceOptions
    {
        protected ContentSource(TOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));
            Options = options;
        }

        protected TOptions Options { get; set; }
    }
}
