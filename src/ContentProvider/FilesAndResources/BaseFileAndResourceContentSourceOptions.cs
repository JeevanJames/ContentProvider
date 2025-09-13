// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider.FilesAndResources;

public abstract class BaseFileAndResourceContentSourceOptions : ContentSourceOptions
{
    /// <summary>
    ///     Optional delegate to filter found content names.
    /// </summary>
    public Func<string, bool>? NameFilter { get; set; }

    /// <summary>
    ///     File extension to filter files and embedded resources. If not specified, all files and embedded
    ///     resources under the specific scope (base directory and resource namespace respectively)
    ///     are considered.
    /// </summary>
    public string? FileExtension { get; set; }

    /// <summary>
    ///     Indicates whether to keep the file extension in the content name.
    /// </summary>
    public bool KeepExtension { get; set; }
}
