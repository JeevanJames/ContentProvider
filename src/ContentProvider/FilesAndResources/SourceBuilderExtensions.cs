// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Reflection;

using ContentProvider.EmbeddedResources;
using ContentProvider.FilesAndResources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ContentProvider;

public static partial class SourceBuilderExtensions
{
    public static ContentSourceBuilder<ResourceContentSource> FilesAndResources(this ContentSourceBuilder builder,
        string? baseDirectory = null, Assembly? assembly = null,
        Action<FileAndResourceContentSourceOptionsBuilder>? sourceBuilderAction = null)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        FileAndResourceContentSourceOptions options;
        if (sourceBuilderAction is not null)
        {
            FileAndResourceContentSourceOptionsBuilder optionsBuilder = new();
            sourceBuilderAction(optionsBuilder);
            options = optionsBuilder.Build();
        }
        else
            options = FileAndResourceContentSourceOptions.Default;

        return builder
            .FilesIn(baseDirectory ?? Directory.GetCurrentDirectory(), b =>
            {
                if (options.NameTransformer is not null)
                    b.WithNameTransformer(options.NameTransformer);
                if (options.NameMatcher is not null)
                    b.FilterNamesBy(options.NameMatcher);
                if (!string.IsNullOrWhiteSpace(options.FileExtension))
                    b.WithFileExtension(options.FileExtension);
                if (options.KeepExtension)
                    b.KeepExtensions();
            })
            .ThenFrom.ResourcesIn(assembly ?? Assembly.GetCallingAssembly(), b =>
            {
                if (options.NameTransformer is not null)
                    b.WithNameTransformer(options.NameTransformer);
                if (options.NameMatcher is not null)
                    b.FilterNamesBy(options.NameMatcher);
                if (!string.IsNullOrWhiteSpace(options.FileExtension))
                    b.WithFileExtension(options.FileExtension);
                if (options.KeepExtension)
                    b.KeepExtensions();
                if (!string.IsNullOrWhiteSpace(options.ResourceNamespace))
                    b.WithResourceNamespace(options.ResourceNamespace);
            });
    }
}
