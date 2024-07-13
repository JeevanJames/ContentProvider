#region --- License & Copyright Notice ---
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

using System;
using System.IO;

using ContentProvider.EmbeddedResources;
using ContentProvider.Files;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContentProvider
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers content from one or more sources that can be injected into the application.
        ///     <para/>
        ///     This method does not associate the content with a specific type, so to access it later,
        ///     you must inject a <see cref="IContentManager"/> instance and use it's
        ///     <see cref="IContentManager.GetContentSet(string)"/> to get this content set by name.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="name">
        ///     A name that can be used to reference this content when accessing through an injected
        ///     <see cref="IContentManager"/>.
        /// </param>
        /// <param name="sourceBuilder">
        ///     A function used to set up the source for the content, along with any fallback sources.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="services"/> or <paramref name="sourceBuilder"/> parameters
        ///     are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the <paramref name="name"/> parameter is <c>null</c>, empty or whitespaces.
        /// </exception>
        public static IServiceCollection AddContent(this IServiceCollection services,
            string name,
            Action<ContentBuilder> sourceBuilder)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Errors.InvalidContentSetName, nameof(name));
            if (sourceBuilder is null)
                throw new ArgumentNullException(nameof(sourceBuilder));

            // Register the IContentManager interface.
            services.TryAddSingleton<IContentManager>(ContentManager.Global);

            // Create the content builder and register it.
            ContentManager.Global.Register(name, sourceBuilder);

            return services;
        }

        /// <summary>
        ///     Registers content from one or more sources that can be injected into the application.
        ///     <para/>
        ///     This method associates the content with a specific <see cref="ContentSet"/> type, so
        ///     to access it later, you can simply inject the <typeparamref name="TContentSet"/> type
        ///     into your code.
        /// </summary>
        /// <typeparam name="TContentSet">
        ///     The content set class to register with the DI container.
        /// </typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="sourceBuilder">
        ///     A function used to set up the source for the content, along with any fallback sources.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="services"/> or <paramref name="sourceBuilder"/> parameters
        ///     are <c>null</c>.
        /// </exception>
        public static IServiceCollection AddContent<TContentSet>(this IServiceCollection services,
            Action<ContentBuilder> sourceBuilder)
            where TContentSet : ContentSet, new()
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            // Register the IContentManager interface.
            services.TryAddSingleton<IContentManager>(ContentManager.Global);

            // Create the content builder and register it.
            ContentManager.Global.Register<TContentSet>(sourceBuilder);

            // Register the content set type (TContentSet) with the container.
            services.AddSingleton(sp => sp.GetRequiredService<IContentManager>().GetContentSet<TContentSet>());

            return services;
        }

        /// <summary>
        ///     Shortcut method to register content from files on the file system.
        /// </summary>
        /// <typeparam name="TContentSet">
        ///     The content set class to register with the DI container.
        /// </typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="fileExtension">The file extension of the files to register.</param>
        /// <param name="baseDirectory">
        ///     The base directory under which to find the file content.
        ///     <para/>
        ///     If not specified, the current directory will be used.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFileContent<TContentSet>(this IServiceCollection services,
            string? fileExtension,
            string? baseDirectory = null)
            where TContentSet : ContentSet, new()
        {
            // Assign defaults to parameters
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = Directory.GetCurrentDirectory();

            // Check file extension does not start with '.'
            if (fileExtension?.StartsWith(".", StringComparison.OrdinalIgnoreCase) == true)
                fileExtension = fileExtension.Substring(1);

            // Add the content from the predefined sources.
            services.AddContent<TContentSet>(builder => builder
                .From.FilesIn(baseDirectory, FileOptions(fileExtension)));

            return services;
        }

        /// <summary>
        ///     Shortcut method to register content from embedded resources in the assembly where the
        ///     type argument <typeparamref name="TContentSet"/> is contained.
        /// </summary>
        /// <typeparam name="TContentSet">
        ///     The content set class to register with the DI container.
        /// </typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="fileExtension">
        ///     The file extension of the resources to register.
        /// </param>
        /// <param name="rootNamespace">
        ///     The root namespace of the embedded resources. This part will be stripped from the
        ///     content name.
        ///     <para/>
        ///     If not specified, the namespace of the content set type
        ///     (<typeparamref name="TContentSet"/>) will be used.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddResourceContent<TContentSet>(this IServiceCollection services,
            string? fileExtension,
            string? rootNamespace = null)
            where TContentSet : ContentSet, new()
        {
            // Assign defaults to parameters
            if (string.IsNullOrWhiteSpace(rootNamespace))
                rootNamespace = typeof(TContentSet).Namespace;

            // Check file extension does not start with '.'
            if (fileExtension?.StartsWith(".", StringComparison.OrdinalIgnoreCase) == true)
                fileExtension = fileExtension.Substring(1);

            // Add the content from the predefined sources.
            services.AddContent<TContentSet>(builder => builder
                .From.ResourcesIn(typeof(TContentSet).Assembly, ResourceOptions(fileExtension, rootNamespace)));

            return services;
        }

        /// <summary>
        ///     Shortcut method to register content from files on the file system, with a fallback to
        ///     embedded resources if the file does not exist.
        ///     <para/>
        ///     This method will take care to normalize the content names for the files and resources,
        ///     so that they match each other, thereby allowing for a clean fallback.
        /// </summary>
        /// <typeparam name="TContentSet">
        ///     The content set class to register with the DI container.
        /// </typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="fileExtension">
        ///     The file extension of the files and resources to register.
        /// </param>
        /// <param name="baseDirectory">
        ///     The base directory under which to find the file content. This should follow the same
        ///     hierarchy as the embedded resources to work correctly.
        ///     <para/>
        ///     If not specified, the current directory will be used.
        /// </param>
        /// <param name="rootNamespace">
        ///     The root namespace of the embedded resources. This part will be stripped from the
        ///     content name.
        ///     <para/>
        ///     If not specified, the namespace of the content set type
        ///     (<typeparamref name="TContentSet"/>) will be used.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFileContentWithResourcesFallback<TContentSet>(
            this IServiceCollection services,
            string? fileExtension,
            string? baseDirectory = null,
            string? rootNamespace = null)
            where TContentSet : ContentSet, new()
        {
            // Assign defaults to parameters
            if (string.IsNullOrWhiteSpace(rootNamespace))
                rootNamespace = typeof(TContentSet).Namespace;
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = Directory.GetCurrentDirectory();

            // Check file extension does not start with '.'
            if (fileExtension?.StartsWith(".", StringComparison.OrdinalIgnoreCase) == true)
                fileExtension = fileExtension.Substring(1);

            // Add the content from the predefined sources.
            services.AddContent<TContentSet>(builder => builder
                .From.FilesIn(baseDirectory, FileOptions(fileExtension))
                .From.ResourcesIn(typeof(TContentSet).Assembly, ResourceOptions(fileExtension, rootNamespace)));

            return services;
        }

        private static FileContentSourceOptions FileOptions(string fileExtension)
        {
            return new FileContentSourceOptions
            {
                SearchPattern = $"*.{fileExtension}",
                SearchOption = SearchOption.AllDirectories,

                // Replace all path separators with dots
                NameTransformer = name => name
                    .Replace('/', '.')
                    .Replace('\\', '.'),
            };
        }

        private static EmbeddedResourceContentSourceOptions ResourceOptions(string fileExtension, string rootNamespace)
        {
            return new EmbeddedResourceContentSourceOptions
            {
                FileExtension = fileExtension,
                RootNamespace = rootNamespace,
            };
        }
    }
}
