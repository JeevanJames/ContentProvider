// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider;

public interface IContentManager
{
    /// <summary>
    ///     Returns the <see cref="IContentSet"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The content set name.</param>
    /// <returns>The <see cref="IContentSet"/> instance, if found; otherwise <c>null</c>.</returns>
    IContentSet GetContentSet(string name);

    TContentSet GetContentSet<TContentSet>()
        where TContentSet : IContentSet, new();

    TContentSet GetContentSet<TContentSet>(string name)
        where TContentSet : IContentSet, new();
}
