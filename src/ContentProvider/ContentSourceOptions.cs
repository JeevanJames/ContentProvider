// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider;

/// <summary>
///     Base class for all content source options.
/// </summary>
public abstract class ContentSourceOptions
{
    /// <summary>
    ///     Gets or sets an optional delegate that can be used to transform the content entry names
    ///     generated from the content source.
    /// </summary>
    public Func<string, string>? NameTransformer { get; set; }
}
