// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Formats.Json;
using ContentProvider.Formats.Json.Structures;

namespace ContentProvider.Tests;

[Collection(nameof(ContentManagerFixture))]
public sealed class JsonTests
{
    private readonly IContentSet _contentSet;

    public JsonTests(ContentManagerFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _contentSet = fixture.ContentManager.GetContentSet("Json");
    }

    [Fact]
    public async Task Able_to_load_json_resources()
    {
        var value = await _contentSet.GetAsJsonAsync<JsonStruct>("Content");

        value.ShouldNotBeNull();
    }

    [Fact]
    public async Task Able_to_load_custom_list_entry()
    {
        var value = await _contentSet.GetJsonAsCustomListEntry<CustomListEntry>("CustomListEntry", "Flash", 2);

        value.ShouldNotBeNull();
        value.Name.ShouldBe("Barry Allen");
    }
}

public sealed class JsonStruct
{
    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;
}

public sealed class CustomListEntry
{
    public string Name { get; set; } = null!;
}
