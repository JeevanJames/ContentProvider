// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider.FilesAndResources;

public sealed class FileAndResourceContentSourceOptions : ContentSourceOptions
{
    public Func<string, bool>? NameMatcher { get; set; }

    public string? FileExtension { get; set; }

    public bool KeepExtension { get; set; }

    public string? ResourceNamespace { get; set; }

    internal static readonly FileAndResourceContentSourceOptions Default = new();
}
