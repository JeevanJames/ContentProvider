﻿#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020-2024 Damian Kulik, Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Diagnostics;
using System.Reflection;

namespace ContentProvider.EmbeddedResources;

[DebuggerDisplay("Embedded resources content source ({_resources.Count} items)")]
public sealed class EmbeddedResourceContentSource : ContentSource<EmbeddedResourceContentSourceOptions>
{
    private readonly Dictionary<string, ResourceDetail> _resources = new Dictionary<string, ResourceDetail>();

    public EmbeddedResourceContentSource(IEnumerable<Assembly> assemblies,
        EmbeddedResourceContentSourceOptions options)
        : base(options)
    {
        if (assemblies is null)
            throw new ArgumentNullException(nameof(assemblies));

        DiscoverResources(assemblies);
    }

    private void DiscoverResources(IEnumerable<Assembly> assemblies)
    {
        var contentNameGetter = string.IsNullOrWhiteSpace(Options.RootNamespace)
            ? (Func<string, string>)(res => res)
            : res => res.Substring(Options.RootNamespace!.Length + 1);

        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<string> resourceNames = assembly.GetManifestResourceNames();
            if (Options.NameMatcher != null)
                resourceNames = resourceNames.Where(res => Options.NameMatcher.IsMatch(res));
            if (!string.IsNullOrWhiteSpace(Options.FileExtension))
            {
                resourceNames = resourceNames
                    .Where(res => res.EndsWith($".{Options.FileExtension}", StringComparison.OrdinalIgnoreCase));
            }

            foreach (string resourceName in resourceNames.ToList())
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
    }

    public override async Task<(bool success, string? content)> TryLoadAsStringAsync(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return (false, null);

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);
#pragma warning disable S3966 // Objects should not be disposed more than once
        using var reader = new StreamReader(resourceStream);
#pragma warning restore S3966 // Objects should not be disposed more than once

        string content = await reader.ReadToEndAsync().ConfigureAwait(false);
        return (true, content);
    }

    public override async Task<(bool success, byte[]? content)> TryLoadAsBinaryAsync(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return (false, null);

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);

        var buffer = new byte[2048];

        using var ms = new MemoryStream();
        int read;
        while ((read = await resourceStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            await ms.WriteAsync(buffer, 0, read).ConfigureAwait(false);
        byte[] content = ms.ToArray();

        return (true, content);
    }

    public override (bool success, string? content) TryLoadAsString(string name)
    {
        if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
            return (false, null);

        using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);
#pragma warning disable S3966 // Objects should not be disposed more than once
        using var reader = new StreamReader(resourceStream);
#pragma warning restore S3966 // Objects should not be disposed more than once

        string content = reader.ReadToEnd();
        return (true, content);
    }

    public override (bool success, byte[]? content) TryLoadAsBinary(string name)
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

        return (true, content);
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
