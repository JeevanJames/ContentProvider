// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ContentProvider;

public sealed class ContentSourceBuilder<TSource>
    where TSource : ContentSource
{
    private readonly ContentBuilder _builder;
    private readonly TSource _source;

    internal ContentSourceBuilder(ContentBuilder builder, TSource source)
    {
        _builder = builder;
        _source = source;
    }

    public ContentSourceBuilder<TSource> Configure(Action<TSource> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        configurer(_source);
        return this;
    }

    public ContentSourceBuilder ThenFrom => new(_builder);
}
