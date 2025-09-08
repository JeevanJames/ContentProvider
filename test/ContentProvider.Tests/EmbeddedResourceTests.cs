// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests;

[Collection(nameof(ContentManagerFixture))]
public sealed class EmbeddedResourceTests
{
    private readonly ContentManager _contentManager;

    public EmbeddedResourceTests(ContentManagerFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _contentManager = fixture.ContentManager;
    }

    [Fact]
    public async Task Able_to_load_embedded_resources()
    {
        IContentSet content = _contentManager.GetContentSet("Text");
        string value = await content.GetAsStringAsync("Content");

        content.ShouldNotBeNull();
        value.ShouldBe("This is the content.");
    }
}
