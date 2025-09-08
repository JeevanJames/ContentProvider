// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ContentProvider;

public sealed class ContentSourceBuilder
{
    private readonly ContentBuilder _builder;
    private ContentSource? _source;

    internal ContentSourceBuilder(ContentBuilder builder)
    {
        _builder = builder;
    }

    public ContentSourceBuilder Source(ContentSource source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (_source is not null)
            throw new InvalidOperationException("Cannot assign source again; it has already been assigned.");
        _builder.Add(source);
        _source = source;
        return this;
    }

    public ContentSourceBuilder<TSource> Source<TSource>(TSource source)
        where TSource : ContentSource
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        _builder.Add(source);
        return new ContentSourceBuilder<TSource>(_builder, source);
    }

    public ContentSourceBuilder<TSource> Source<TSource>()
        where TSource : ContentSource, new()
    {
        TSource source = new();
        _builder.Add(source);
        return new ContentSourceBuilder<TSource>(_builder, source);
    }

    public ContentSourceBuilder ThenFrom => new(_builder);
}
