#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright 2020 Jeevan James

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
using System.Collections.Generic;
using System.IO;

using ContentProvider.EmbeddedResources;
using ContentProvider.Files;

using Microsoft.Extensions.DependencyInjection;

namespace ContentProvider
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContent(this IServiceCollection services,
            string name,
            Action<ContentBuilder> sourceBuilder)
        {
            if (sourceBuilder is null)
                throw new ArgumentNullException(nameof(sourceBuilder));

            // Register the IContentManager interface. This can be done multiple times.
            services.AddSingleton<IContentManager, ContentManagerImpl>();

            // Create the content builder and register it.
            var builder = new ContentBuilder();
            sourceBuilder(builder);
            ContentManager.Register(name, builder.Build());

            return services;
        }

        public static IServiceCollection AddContent<TContentSet>(this IServiceCollection services,
            string name,
            Action<ContentBuilder> sourceBuilder)
            where TContentSet : ContentSetBase, new()
        {
            // Adds the content as normal
            services.AddContent(name, sourceBuilder);

            // Register the content set type (TContentSet) with the container.
            services.AddSingleton(sp =>
            {
                ContentSet internalContentSet = sp.GetRequiredService<IContentManager>().GetContentSet(name);
                return new TContentSet
                {
                    ContentSet = internalContentSet,
                };
            });

            return services;
        }

        public static IServiceCollection AddFileContent<TContentSet>(this IServiceCollection services,
            string fileExtension,
            string baseDirectory = null)
            where TContentSet : ContentSetBase, new()
        {
            return services.AddPredefinedContent<TContentSet>(fileExtension, (services, contentSetType, args) =>
            {
                string baseDirectoryCopy = args[0];

                // Assign defaults to parameters
                if (string.IsNullOrWhiteSpace(baseDirectoryCopy))
                    baseDirectoryCopy = Directory.GetCurrentDirectory();

                // Add the content from the predefined sources.
                services.AddContent(contentSetType.AssemblyQualifiedName, builder => builder
                    .From.FilesIn(baseDirectoryCopy, FileOptions(fileExtension)));
            }, baseDirectory);
        }

        public static IServiceCollection AddResourceContent<TContentSet>(this IServiceCollection services,
            string fileExtension,
            string rootNamespace = null)
            where TContentSet : ContentSetBase, new()
        {
            return services.AddPredefinedContent<TContentSet>(fileExtension, (services, contentSetType, args) =>
            {
                string rootNamespaceCopy = args[0];

                // Assign defaults to parameters
                if (string.IsNullOrWhiteSpace(rootNamespaceCopy))
                    rootNamespaceCopy = contentSetType.Namespace;

                // Add the content from the predefined sources.
                services.AddContent(contentSetType.AssemblyQualifiedName, builder => builder
                    .From.ResourcesIn(contentSetType.Assembly, ResourceOptions(fileExtension, rootNamespaceCopy)));
            }, rootNamespace);
        }

        /// <summary>
        ///     Shortcut method to register content from the typical sources, namely files on the file
        ///     system, with a fallback to embedded resources.
        ///     <para/>
        ///     This method will take care to normalize the content names for the files and resources,
        ///     so that they match each other, thereby allowing for a clean fallback.
        /// </summary>
        /// <typeparam name="TContentSet">The content set class to register with the DI container.</typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="fileExtension">
        ///     The file extension (without the dot) of the files and resources to register.
        /// </param>
        /// <param name="rootNamespace">
        ///     The root namespace of the embedded resources. This part will be stripped from the
        ///     content name.
        ///     <para/>
        ///     If not specified, the namespace of the content set type
        ///     (<typeparamref name="TContentSet"/>) will be used.
        /// </param>
        /// <param name="baseDirectory">
        ///     The base directory under which to find the file content. This should follow the same
        ///     hierarchy as the embedded resources to work correctly.
        ///     <para/>
        ///     If not specified, the current directory will be used.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFileContentWithFallbackToResources<TContentSet>(this IServiceCollection services,
            string fileExtension,
            string rootNamespace = null,
            string baseDirectory = null)
            where TContentSet : ContentSetBase, new()
        {
            return services.AddPredefinedContent<TContentSet>(fileExtension, (services, contentSetType, args) =>
            {
                string rootNamespaceCopy = args[0];
                string baseDirectoryCopy = args[1];

                // Assign defaults to parameters
                if (string.IsNullOrWhiteSpace(rootNamespaceCopy))
                    rootNamespaceCopy = typeof(TContentSet).Namespace;
                if (string.IsNullOrWhiteSpace(baseDirectoryCopy))
                    baseDirectoryCopy = Directory.GetCurrentDirectory();

                // Add the content from the predefined sources.
                services.AddContent(contentSetType.AssemblyQualifiedName, builder => builder
                    .From.FilesIn(baseDirectoryCopy, FileOptions(fileExtension))
                    .From.ResourcesIn(typeof(TContentSet).Assembly, ResourceOptions(fileExtension, rootNamespaceCopy)));
            }, rootNamespace, baseDirectory);
        }

        private static IServiceCollection AddPredefinedContent<TContentSet>(this IServiceCollection services,
            string fileExtension,
            Action<IServiceCollection, Type, IList<string>> registrationAction,
            params string[] args)
            where TContentSet : ContentSetBase, new()
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentException(Errors.InvalidFileExtension, nameof(fileExtension));

            // Check file extension does not start with '.'
            if (fileExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                fileExtension = fileExtension.Substring(1);

            registrationAction(services, typeof(TContentSet), args);

            // Register the content set type (TContentSet) with the container.
            services.AddSingleton(sp =>
            {
                ContentSet internalContentSet = sp.GetRequiredService<IContentManager>().GetContentSet(fileExtension);
                return new TContentSet
                {
                    ContentSet = internalContentSet,
                };
            });

            return services;
        }

        private static FileContentSourceOptions FileOptions(string fileExtension) =>
            new FileContentSourceOptions
            {
                SearchPattern = $"*.{fileExtension}",
                SearchOption = SearchOption.AllDirectories,

                // Replace all path separators with dots and remove the file extension
                NameTransformer = name => name
                    .Substring(0, name.Length - fileExtension.Length - 1)
                    .Replace('/', '.')
                    .Replace('\\', '.'),
            };

        private static EmbeddedResourceContentSourceOptions ResourceOptions(string fileExtension, string rootNamespace) =>
            new EmbeddedResourceContentSourceOptions
            {
                FileExtension = fileExtension,
                RootNamespace = rootNamespace,
                NameTransformer = name => name.Substring(0, name.Length - fileExtension.Length - 1),
            };
    }
}
