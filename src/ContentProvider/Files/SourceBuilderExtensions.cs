// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Files;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ContentProvider;

public static partial class SourceBuilderExtensions
{
    /// <summary>
    ///     Registers a <see cref="ContentSource"/> for content from files on the file system.
    /// </summary>
    /// <param name="builder">The <see cref="ContentSourceBuilder"/> instance.</param>
    /// <param name="baseDirectory">The base directory to retrieve the files from.</param>
    /// <param name="sourceBuilderAction">Delegate to setup options for registering the content source.</param>
    /// <returns>An instance of <see cref="ContentSourceBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="builder"/> parameter is <c>null</c>.
    /// </exception>
    public static ContentSourceBuilder<FileContentSource> FilesIn(this ContentSourceBuilder builder,
        string baseDirectory, Action<FileContentSourceOptionsBuilder>? sourceBuilderAction = null)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        FileContentSourceOptionsBuilder sourceBuilder = new(baseDirectory);
        sourceBuilderAction?.Invoke(sourceBuilder);
        return builder.Source(sourceBuilder.Build());
    }

    public static ContentSourceBuilder<FileContentSource> FilesInCurrentDirectory(this ContentSourceBuilder builder,
        Action<FileContentSourceOptionsBuilder>? sourceBuilderAction = null) =>
        builder.FilesIn(Directory.GetCurrentDirectory(), sourceBuilderAction);
}
