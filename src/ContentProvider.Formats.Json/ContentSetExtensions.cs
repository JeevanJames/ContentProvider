// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Text;
using System.Text.Json;

namespace ContentProvider.Formats.Json;

public static class ContentSetExtensions
{
    public static async Task<T?> GetAsJsonAsync<T>(this IContentSet contentSet,
        string name, JsonSerializerOptions? serializerOptions = null)
    {
        if (contentSet is null)
            throw new ArgumentNullException(nameof(contentSet));

        string json = await contentSet.GetAsStringAsync(name).ConfigureAwait(false);

        using var ms = new MemoryStream();
#pragma warning disable CA2000 // Dispose objects before losing scope
        var writer = new StreamWriter(ms, Encoding.UTF8);
#pragma warning restore CA2000 // Dispose objects before losing scope
        await writer.WriteAsync(json).ConfigureAwait(false);
        await writer.FlushAsync().ConfigureAwait(false);
        ms.Position = 0;

        return await JsonSerializer.DeserializeAsync<T>(ms, serializerOptions ?? JsonOptions.SerializerOptions)
            .ConfigureAwait(false);
    }

    public static T? GetAsJson<T>(this IContentSet contentSet, string name,
        JsonSerializerOptions? serializerOptions = null)
    {
        if (contentSet is null)
            throw new ArgumentNullException(nameof(contentSet));

        string json = contentSet.GetAsString(name);
        return JsonSerializer.Deserialize<T>(json, serializerOptions ?? JsonOptions.SerializerOptions);
    }
}
