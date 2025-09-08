// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace ContentProvider.Formats.Json;

public static class JsonOptions
{
    private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions();

    public static JsonSerializerOptions SerializerOptions
    {
        get => _serializerOptions ??= new JsonSerializerOptions();
        set => _serializerOptions = value;
    }
}
