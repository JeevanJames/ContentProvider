// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;

namespace ContentProvider.EmbeddedResources;

[DebuggerDisplay("Embedded resources content source ({_resources.Count} items)")]
public sealed class ResourceContentSource : ContentSource<ResourceContentSourceOptions>
{
    private readonly Dictionary<string, ResourceDetail> _resources = [];

    public ResourceContentSource(Assembly assembly, ResourceContentSourceOptions options)
        : base(options)
    {
        if (assembly is null)
            throw new ArgumentNullException(nameof(assembly));

        DiscoverResources(assembly);
    }

    private void DiscoverResources(Assembly assembly)
    {
        Func<string, string> contentNameGetter = string.IsNullOrWhiteSpace(Options.RootNamespace)
            ? res => res
            : res => res.Substring(Options.RootNamespace.Length + 1);

        IEnumerable<string> resourceNames = assembly.GetManifestResourceNames();

        if (Options.NameMatcher is not null)
            resourceNames = resourceNames.Where(res => Options.NameMatcher.IsMatch(res));

        if (!string.IsNullOrWhiteSpace(Options.FileExtension))
        {
            resourceNames = resourceNames
                .Where(res => res.EndsWith($".{Options.FileExtension}", StringComparison.OrdinalIgnoreCase));
        }

        foreach (string resourceName in resourceNames)
        {
            string contentName = Options.NameTransformer is null
                ? contentNameGetter(resourceName)
                : Options.NameTransformer(contentNameGetter(resourceName));
            if (!Options.KeepExtension)
            {
                int dotIndex = contentName.LastIndexOf('.');
                if (dotIndex > 0)
                    contentName = contentName.Substring(0, dotIndex);
            }

            _resources.Add(contentName, new ResourceDetail(assembly, resourceName));
        }
    }

    public override async Task<string?> LoadAsStringAsync(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return null;

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);
        using var reader = new StreamReader(resourceStream);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public override async Task<byte[]?> LoadAsBinaryAsync(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return null;

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);

        var buffer = new byte[2048];

        using var ms = new MemoryStream();
        int read;
        while ((read = await resourceStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            await ms.WriteAsync(buffer, 0, read).ConfigureAwait(false);
        return ms.ToArray();
    }

    public override string? LoadAsString(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return null;

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);
        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }

    public override byte[]? LoadAsBinary(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return default;

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);

        var buffer = new byte[2048];

        using var ms = new MemoryStream();
        int read;
        while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
            ms.WriteAsync(buffer, 0, read);
        byte[] content = ms.ToArray();

        return content;
    }
}

internal readonly struct ResourceDetail
{
    internal ResourceDetail(Assembly assembly, string resourceName)
    {
        Assembly = assembly;
        ResourceName = resourceName;
    }

    internal Assembly Assembly { get; }

    internal string ResourceName { get; }
}
