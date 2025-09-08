// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.InMemory;

namespace ContentProvider;

public static partial class SourceBuilderExtensions
{
    public static ContentSourceBuilder<InMemoryContentSource> Memory(this ContentSourceBuilder builder)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Source(new InMemoryContentSource());
    }

    public static ContentSourceBuilder<InMemoryContentSource> Memory(this ContentSourceBuilder builder,
        IDictionary<string, string> data)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        return builder.Source(new InMemoryContentSource(data));
    }

    public static ContentSourceBuilder<InMemoryContentSource> Add(
        this ContentSourceBuilder<InMemoryContentSource> builder,
        string key, string value)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Configure(source => source.Add(key, value));
        return builder;
    }
}
