// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Reflection;

using ContentProvider.EmbeddedResources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ContentProvider;

#pragma warning disable S4136 // Method overloads should be grouped together
public static partial class SourceBuilderExtensions
{
    /// <summary>
    ///     Registers a <see cref="ContentSource"/> for content from embedded resources in the specified
    ///     assemblies.
    /// </summary>
    /// <param name="builder">The <see cref="ContentSourceBuilder"/> instance.</param>
    /// <param name="assembly">The assembly that contains the embedded resources.</param>
    /// <param name="sourceBuilderAction">Optional delegate to customize the content source.</param>
    /// <returns>An instance of <see cref="ContentBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="builder"/> or <paramref name="assembly"/> parameters are
    ///     <c>null</c>.
    /// </exception>
    public static ContentSourceBuilder<ResourceContentSource> ResourcesIn(this ContentSourceBuilder builder,
        Assembly assembly, Action<ResourceContentSourceOptionsBuilder>? sourceBuilderAction = null)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));
        if (assembly is null)
            throw new ArgumentNullException(nameof(assembly));

        ResourceContentSourceOptionsBuilder sourceBuilder = new(assembly);
        sourceBuilderAction?.Invoke(sourceBuilder);
        return builder.Source(sourceBuilder.Build());
    }

    public static ContentSourceBuilder<ResourceContentSource> ResourcesInExecutingAssembly(
        this ContentSourceBuilder builder, Action<ResourceContentSourceOptionsBuilder>? sourceBuilderAction = null) =>
        builder.ResourcesIn(Assembly.GetCallingAssembly(), sourceBuilderAction);
}
