// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ContentProvider.EmbeddedResources;

[DebuggerDisplay("Embedded resources content source ({_resources.Count} items)")]
public sealed class ResourceContentSource : ContentSource<ResourceContentSourceOptions>
{
    private readonly Assembly _assembly;
    private readonly Dictionary<string, string> _resources = [];

    public ResourceContentSource(Assembly assembly, ResourceContentSourceOptions options)
        : base(options)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

        DiscoverAndCacheResources(assembly);
    }

    private void DiscoverAndCacheResources(Assembly assembly)
    {
        IEnumerable<string> resourceNames = assembly.GetManifestResourceNames();

        StringBuilder sb = new();
        foreach (string resourceName in resourceNames)
        {
            sb.Clear().Append(resourceName);

            if (!string.IsNullOrWhiteSpace(Options.ResourceNamespace) &&
                !sb.TrimIfStartsWith(Options.ResourceNamespace))
            {
                continue;
            }

            bool specifiesFileExtension = !string.IsNullOrWhiteSpace(Options.FileExtension);
            if (specifiesFileExtension && !sb.EndsWith(Options.FileExtension))
                continue;

            if (!Options.KeepExtension)
            {
                if (specifiesFileExtension)
                    sb.TrimFileExtension(Options.FileExtension);
                else
                    sb.TrimFileExtension();
            }

            string contentName = sb.ToString();

            if (Options.NameFilter?.Invoke(contentName) == false)
                continue;

            if (Options.NameTransformer is not null)
                contentName = Options.NameTransformer(contentName);

            _resources.Add(contentName, resourceName);
        }
    }

    public override async Task<string?> LoadAsStringAsync(string name)
    {
        if (!_resources.TryGetValue(name, out string? resourceName))
            return null;

        using Stream resourceStream = _assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(resourceStream);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public override async Task<byte[]?> LoadAsBinaryAsync(string name)
    {
        if (!_resources.TryGetValue(name, out string? resourceName))
            return null;

        using Stream resourceStream = _assembly.GetManifestResourceStream(resourceName);

        var buffer = new byte[2048];

        using var ms = new MemoryStream();
        int read;
        while ((read = await resourceStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            await ms.WriteAsync(buffer, 0, read).ConfigureAwait(false);
        return ms.ToArray();
    }

    public override string? LoadAsString(string name)
    {
        if (!_resources.TryGetValue(name, out string? resourceName))
            return null;

        using Stream resourceStream = _assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }

    public override byte[]? LoadAsBinary(string name)
    {
        if (!_resources.TryGetValue(name, out string? resourceName))
            return default;

        using Stream resourceStream = _assembly.GetManifestResourceStream(resourceName);

        var buffer = new byte[2048];

        using var ms = new MemoryStream();
        int read;
        while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
            ms.WriteAsync(buffer, 0, read);
        byte[] content = ms.ToArray();

        return content;
    }
}

internal static class StringBuilderExtensions
{
    internal static bool TrimIfStartsWith(this StringBuilder sb, string ns)
    {
        // Include the dot after the resource namespace
        int prefixLength = ns.Length + 1;

        if (sb.Length < prefixLength) return false;

        for (int i = 0; i < ns.Length; i++)
        {
            if (char.ToLowerInvariant(sb[i]) != char.ToLowerInvariant(ns[i]))
                return false;
        }

        // Match the dot as well
        if (char.ToLowerInvariant(sb[ns.Length]) != '.')
            return false;

        sb.Remove(0, prefixLength);
        return true;
    }

    internal static bool EndsWith(this StringBuilder sb, string suffix)
    {
        if (sb.Length < suffix.Length) return false;

        int sbIndex = sb.Length - 1;
        int suffixIndex = suffix.Length - 1;

        while (suffixIndex >= 0)
        {
            if (char.ToLowerInvariant(sb[sbIndex]) != char.ToLowerInvariant(suffix[suffixIndex]))
                return false;

            sbIndex--;
            suffixIndex--;
        }

        return true;
    }

    internal static void TrimFileExtension(this StringBuilder sb, string fileExtension)
    {
        sb.Remove(sb.Length - fileExtension.Length - 1, fileExtension.Length + 1);
    }

    internal static void TrimFileExtension(this StringBuilder sb)
    {
        int dotPos = sb.Length - 1;
        while (dotPos >= 0 && sb[dotPos] != '.')
            dotPos--;
        if (dotPos >= 0)
            sb.Remove(dotPos, sb.Length - dotPos - 1);
    }
}
