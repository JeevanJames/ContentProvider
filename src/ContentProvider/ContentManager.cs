// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ContentProvider;

[DebuggerDisplay("{_contentSets.Count} registered content sets")]
public sealed partial class ContentManager : IContentManager
{
    private readonly Dictionary<string, ContentSet> _contentSets = new(StringComparer.OrdinalIgnoreCase);

    public ContentManager Register(string name, params ContentSource[] sources) =>
        Register<ContentSet>(name, sources);

    public ContentManager Register(string name, Action<ContentBuilder> builderSetup) =>
        Register<ContentSet>(name, builderSetup);

    public ContentManager Register<TContentSet>(params ContentSource[] sources)
        where TContentSet : ContentSet, new() =>
        Register<TContentSet>(typeof(TContentSet).AssemblyQualifiedName, sources);

    public ContentManager Register<TContentSet>(Action<ContentBuilder> builderSetup)
        where TContentSet : ContentSet, new() =>
        Register<TContentSet>(typeof(TContentSet).AssemblyQualifiedName, builderSetup);

    public ContentManager Register<TContentSet>(string name, params ContentSource[] sources)
        where TContentSet : ContentSet, new()
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));
        if (sources is null)
            throw new ArgumentNullException(nameof(sources));

        TContentSet contentSet = new();
        contentSet.Sources.AddRange(sources);
        _contentSets.Add(name, contentSet);

        return this;
    }

    public ContentManager Register<TContentSet>(string name, Action<ContentBuilder> builderSetup)
        where TContentSet : ContentSet, new()
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));
        if (builderSetup is null)
            throw new ArgumentNullException(nameof(builderSetup));

        var builder = new ContentBuilder();
        builderSetup(builder);
        ContentSource[] sources = builder.Build();

        return Register<TContentSet>(name, sources);
    }

    /// <inheritdoc/>
    public IContentSet GetContentSet(string name)
    {
        return _contentSets.TryGetValue(name, out ContentSet contentSet)
            ? contentSet
            : throw new ArgumentException($"Could not find a content set named {name}.", nameof(name));
    }

    /// <inheritdoc/>
    public TContentSet GetContentSet<TContentSet>()
        where TContentSet : IContentSet, new() =>
        GetContentSet<TContentSet>(typeof(TContentSet).AssemblyQualifiedName);

    /// <inheritdoc/>
    public TContentSet GetContentSet<TContentSet>(string name)
        where TContentSet : IContentSet, new()
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));
        if (!_contentSets.TryGetValue(name, out ContentSet contentSet))
            throw new ArgumentException($"Could not find a content set named {name}.", nameof(name));
        if (contentSet is TContentSet typedContentSet)
            return typedContentSet;
        throw new ContentException($"The content set named {name} is a different type {contentSet.GetType().FullName}.");
    }
}

public sealed partial class ContentManager
{
    /// <summary>
    ///     Gets a pre-instatiated global instance of <see cref="ContentManager"/>.
    /// </summary>
    public static ContentManager Global { get; } = new ContentManager();
}
