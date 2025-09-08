// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider;

/// <summary>
///     Represents a named set of contents, consisting of a primary content source and zero or more
///     fallback sources.
/// </summary>
public interface IContentSet
{
    /// <summary>
    ///     Gets the value of the content entry named <paramref name="name"/> as a string.
    /// </summary>
    /// <param name="name">The content entry name.</param>
    /// <returns>The content value as a string, if found; otherwise <c>null</c>.</returns>
    Task<string> GetAsStringAsync(string name);

    /// <summary>
    ///     Gets the value of the content entry named <paramref name="name"/> as a byte array.
    /// </summary>
    /// <param name="name">The content entry name.</param>
    /// <returns>The content value as a byte array, if found; otherwise <c>null</c>.</returns>
    Task<byte[]> GetAsBinaryAsync(string name);

    /// <summary>
    ///     Gets the value of the content entry named <paramref name="name"/> as a string.
    /// </summary>
    /// <param name="name">The content entry name.</param>
    /// <returns>The content value as a string, if found; otherwise <c>null</c>.</returns>
    string GetAsString(string name);

    /// <summary>
    ///     Gets the value of the content entry named <paramref name="name"/> as a byte array.
    /// </summary>
    /// <param name="name">The content entry name.</param>
    /// <returns>The content value as a byte array, if found; otherwise <c>null</c>.</returns>
    byte[] GetAsBinary(string name);
}
