// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Tests.Content;

namespace ContentProvider.Tests.Fixtures;

public sealed class ContentManagerFixture
{
    public ContentManagerFixture()
    {
        ContentManager = new ContentManager()
            .Register("Text", builder => builder
                .From.ResourcesInExecutingAssembly(options => options
                    .WithFileExtension("txt")
                    .WithResourceNamespace<TextContentSet>())
                .ThenFrom.Memory()
                    .Add("InMemoryKey", "InMemoryValue"))
            .Register("Json", builder => builder
                .From.ResourcesInExecutingAssembly(options => options
                    .WithFileExtension("json")
                    .WithResourceNamespace<JsonContentSet>()));
    }

    public ContentManager ContentManager { get; }
}

[CollectionDefinition(nameof(ContentManagerFixture))]
public sealed class ContentManagerCollectionFixture : ICollectionFixture<ContentManagerFixture>;
