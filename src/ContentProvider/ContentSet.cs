#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020-2024 Damian Kulik, Jeevan James

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

using System.Diagnostics;
using System.Globalization;

namespace ContentProvider;

/// <inheritdoc/>
[DebuggerDisplay("Content Set {Name}")]
public class ContentSet : IContentSet
{
    private string _name = null!;

    /// <inheritdoc />
    public string Name
    {
        get => _name;
        internal set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (value.Trim().Length == 0)
                throw new ArgumentException(Errors.InvalidContentSetName, nameof(value));
            _name = value;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ContentException">
    ///     Thrown if content with the specified <paramref name="name"/> is not found in any of the
    ///     registered sources.
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

    /// <inheritdoc />
    /// <exception cref="ContentException">
    ///     Thrown if content with the specified <paramref name="name"/> is not found in any of the
    ///     registered sources.
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

    /// <inheritdoc />
    /// <exception cref="ContentException">
    ///     Thrown if content with the specified <paramref name="name"/> is not found in any of the
    ///     registered sources.
    /// </exception>
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

    /// <inheritdoc />
    /// <exception cref="ContentException">
    ///     Thrown if content with the specified <paramref name="name"/> is not found in any of the
    ///     registered sources.
    /// </exception>
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
