// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.FilesAndResources;

namespace ContentProvider.EmbeddedResources;

public sealed class ResourceContentSourceOptions : BaseFileAndResourceContentSourceOptions
{
    /// <summary>
    ///     Gets or sets the namespace to filter embedded resources. If not specified, all embedded
    ///     resources are considered.
    /// </summary>
    public string? ResourceNamespace { get; set; }
}
