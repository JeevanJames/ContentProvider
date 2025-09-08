// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider.Files;

public sealed class FileContentSourceOptions : ContentSourceOptions
{
    /// <summary>
    ///     Gets or sets the search string to match against the names of the files. This parameter can
    ///     contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't
    ///     support regular expressions.
    ///     <para/>
    ///     The default is <c>*</c>.
    /// </summary>
    public string SearchPattern { get; set; } = "*";

    /// <summary>
    ///     Gets or sets an enumeration value that specified whether the search operation should include
    ///     all subdirectories or only the current directory.
    /// </summary>
    public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

    /// <summary>
    ///     Gets or sets a value indicating whether to keep the file extension in the content name.
    /// </summary>
    public bool KeepExtension { get; set; }
}
