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
    public interface IContentManager
    {
        /// <summary>
        ///     Returns the <see cref="IContentSet"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The content set name.</param>
        /// <returns>The <see cref="IContentSet"/> instance, if found; otherwise <c>null</c>.</returns>
        IContentSet GetContentSet(string name);

        /// <summary>
        ///     Gets the value of the content entry named <paramref name="entryName"/> from the content
        ///     set named <paramref name="name"/>, as a string.
        /// </summary>
        /// <param name="name">The content set name.</param>
        /// <param name="entryName">The content entry name.</param>
        /// <returns>The content value as a string, if found; otherwise <c>null</c>.</returns>
        Task<string> GetAsString(string name, string entryName);

        /// <summary>
        ///     Gets the value of the content entry named <paramref name="entryName"/> from the content
        ///     set named <paramref name="name"/>, as a byte array.
        /// </summary>
        /// <param name="name">The content set name.</param>
        /// <param name="entryName">The content entry name.</param>
        /// <returns>The content value as a byte array, if found; otherwise <c>null</c>.</returns>
        Task<byte[]> GetAsBinary(string name, string entryName);
    }

    public static class ContentManagerExtensions
    {
        /// <summary>
        ///     Gets the value of the content entry named <paramref name="entryName"/> from the content
        ///     set named <paramref name="name"/>, as a byte collection.
        /// </summary>
        /// <param name="contentManager">The <see cref="IContentManager"/> instance to get the content from.</param>
        /// <param name="name">The content set name.</param>
        /// <param name="entryName">The content entry name.</param>
        /// <returns>The content value as a byte collection, if found; otherwise <c>null</c>.</returns>
        public static Task<IList<byte>> GetAsBinary(this IContentManager contentManager, string name, string entryName)
        {
            if (contentManager is null)
                throw new ArgumentNullException(nameof(contentManager));
            return contentManager.GetAsBinaryInternal(name, entryName);
        }

        private static async Task<IList<byte>> GetAsBinaryInternal(this IContentManager contentManager, string name, string entryName)
        {
            return await contentManager.GetAsBinary(name, entryName).ConfigureAwait(false);
        }
    }
}
