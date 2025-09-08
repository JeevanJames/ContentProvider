// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Text;

namespace ContentProvider;

public abstract class ContentSource
{
    /// <summary>
    ///     Attempts to load an item from the content source as a string, given its <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the content item to retrieve.</param>
    /// <returns>
    ///     The content of the source as a string; otherwise <c>null</c> if the item is not found.
    /// </returns>
    public virtual async Task<string?> LoadAsStringAsync(string name)
    {
        byte[]? content = await LoadAsBinaryAsync(name).ConfigureAwait(false);
        return content is not null ? Encoding.UTF8.GetString(content) : null;
    }

    /// <summary>
    ///     Attempts to load an item from the content source as a byte array, given its <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the content item to retrieve.</param>
    /// <returns>
    ///     The content of the source as a byte array; otherwise <c>null</c> if the item is not found.
    /// </returns>
    public virtual Task<byte[]?> LoadAsBinaryAsync(string name) =>
        Task.FromResult(LoadAsBinary(name));

    public virtual string? LoadAsString(string name)
    {
        byte[]? content = LoadAsBinary(name);
        return content is not null ? Encoding.UTF8.GetString(content) : default;
    }

    public abstract byte[]? LoadAsBinary(string name);
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
