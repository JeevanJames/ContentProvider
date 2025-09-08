// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Extensions.DependencyInjection;
using ContentProvider.Formats.Json;
using ContentProvider.Tests.Content;
using ContentProvider.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests;

[Collection(nameof(ServiceProviderFixture))]
public sealed class ServiceCollectionTests(ServiceProviderFixture fixture)
{
    private readonly ServiceProviderFixture _fixture = fixture;

    [Fact]
    public async Task Can_inject_typed_content_set()
    {
        TextContentSet contentSet = _fixture.ServiceProvider.GetService<TextContentSet>();
        contentSet.ShouldNotBeNull();

        string value = await contentSet.GetAsStringAsync("Content");
        value.ShouldBe("This is the content.");
    }

    [Fact]
    public async Task Can_inject_named_content_set()
    {
        IContentSet contentSet = _fixture.ServiceProvider.GetKeyedService<IContentSet>("Json");
        contentSet.ShouldNotBeNull();

        string value = await contentSet.GetAsStringAsync("Content");
        value.ShouldNotBeNullOrWhiteSpace();
    }
}
