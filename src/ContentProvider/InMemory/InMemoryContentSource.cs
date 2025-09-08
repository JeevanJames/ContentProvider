// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Text;

namespace ContentProvider.InMemory;

public sealed class InMemoryContentSource : ContentSource
{
    private readonly Dictionary<string, string> _data = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryContentSource()
    {
    }

    public InMemoryContentSource(IDictionary<string, string> data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        foreach (KeyValuePair<string, string> kvp in data)
            _data.Add(kvp.Key, kvp.Value);
    }

    public InMemoryContentSource Add(string key, string value)
    {
        _data.Add(key, value);
        return this;
    }

    public override string? LoadAsString(string name)
    {
        return _data.TryGetValue(name, out string? value) ? value : null;
    }

    public override byte[]? LoadAsBinary(string name)
    {
        return _data.TryGetValue(name, out string? value) ? Encoding.UTF8.GetBytes(value) : null;
    }
}
