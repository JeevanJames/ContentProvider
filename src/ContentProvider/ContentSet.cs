// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider;

/// <inheritdoc/>
public class ContentSet : IContentSet
{
    /// <summary>
    ///     Gets the list of content sources registered with this content set.
    /// </summary>
    internal List<ContentSource> Sources { get; } = [];

    /// <inheritdoc />
    /// <exception cref="ContentException">
    ///     Thrown if content with the specified <paramref name="name"/> is not found in any of the
    ///     registered sources.
    /// </exception>
    public async Task<string> GetAsStringAsync(string name)
    {
        foreach (ContentSource source in Sources)
        {
            string? content = await source.LoadAsStringAsync(name).ConfigureAwait(false);
            if (content is not null)
                return content;
        }

        throw new ContentException($"Could not find content entry named '{name}'.");
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
            byte[]? content = await source.LoadAsBinaryAsync(name).ConfigureAwait(false);
            if (content is not null)
                return content;
        }

        throw new ContentException($"Could not find content entry named '{name}'.");
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
            string? content = source.LoadAsString(name);
            if (content is not null)
                return content!;
        }

        throw new ContentException($"Could not find content entry named '{name}'.");
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
            byte[]? content = source.LoadAsBinary(name);
            if (content is not null)
                return content!;
        }

        throw new ContentException($"Could not find content entry named '{name}'.");
    }
}
