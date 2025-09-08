// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace ContentProvider.EmbeddedResources;

public sealed class ResourceContentSourceOptions : ContentSourceOptions
{
    /// <summary>
    ///     Gets or sets the file extension to filter embedded resources. If not specified, all
    ///     embedded resources are considered.
    /// </summary>
    public string? FileExtension { get; set; }

    public Regex? NameMatcher { get; set; }

    /// <summary>
    ///     Gets or sets the namespace to filter embedded resources. If not specified, all embedded
    ///     resources are considered.
    /// </summary>
    public string? RootNamespace { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to keep the file extension in the content name.
    /// </summary>
    public bool KeepExtension { get; set; }
}
