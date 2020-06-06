#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020 Damian Kulik, Jeevan James

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

            // Register the IContentManager interface. This can be done multiple times.
            services.AddSingleton<IContentManager>(ContentManager.Global);

            // Create the content builder and register it.
            var builder = new ContentBuilder();
            sourceBuilder(builder);
            ContentManager.Global.Register(name, builder.Build());

            return services;
        }

        /// <summary>
        ///     Registers content from one or more sources that can be injected into the application.
        ///     <para/>
        ///     This method associates the content with a specific <see cref="ContentSetBase"/> type, so
        ///     to access it later, you can simply inject the <typeparamref name="TContentSet"/> type
        ///     into your code.
        /// </summary>
        /// <typeparam name="TContentSet">
        ///     The content set class to register with the DI container.
        /// </typeparam>
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
                IContentSet internalContentSet = sp.GetRequiredService<IContentManager>().GetContentSet(name);
                return new TContentSet
                {
                    ContentSet = internalContentSet,
                };
            });

            return services;
        }

        /// <summary>
        ///     Shortcut method to register content from files on the file system.
        /// </summary>
        /// <typeparam name="TContentSet">
        ///     The content set class to register with the DI container.
        /// </typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="fileExtension">
        ///     The file extension of the files to register. If multiple file extensions are needed, use
        ///     the advanced <see cref="AddContent(IServiceCollection, string, Action{ContentBuilder})"/>
        ///     or <see cref="AddContent{TContentSet}(IServiceCollection, string, Action{ContentBuilder})"/>
        ///     methods to have more control on the content that is added.
        /// </param>
        /// <param name="baseDirectory">
        ///     The base directory under which to find the file content.
        ///     <para/>
        ///     If not specified, the current directory will be used.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
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

                // Check file extension does not start with '.'
                if (fileExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                    fileExtension = fileExtension.Substring(1);

                // Add the content from the predefined sources.
                services.AddContent(contentSetType.AssemblyQualifiedName, builder => builder
                    .From.FilesIn(baseDirectoryCopy, FileOptions(fileExtension)));
            }, baseDirectory);
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

                // Check file extension does not start with '.'
                if (fileExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                    fileExtension = fileExtension.Substring(1);

                // Add the content from the predefined sources.
                services.AddContent(contentSetType.AssemblyQualifiedName, builder => builder
                    .From.ResourcesIn(contentSetType.Assembly, ResourceOptions(fileExtension, rootNamespaceCopy)));
            }, rootNamespace);
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
        public static IServiceCollection AddFileContentWithFallbackToResources<TContentSet>(
            this IServiceCollection services,
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

                // Check file extension does not start with '.'
                if (fileExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                    fileExtension = fileExtension.Substring(1);

                // Add the content from the predefined sources.
                services.AddContent(contentSetType.AssemblyQualifiedName, builder => builder
                    .From.FilesIn(baseDirectoryCopy, FileOptions(fileExtension))
                    .From.ResourcesIn(typeof(TContentSet).Assembly, ResourceOptions(fileExtension, rootNamespaceCopy)));
            }, rootNamespace, baseDirectory);
        }

        // Common method used by AddFileContent, AddResourceContent and
        // AddAddFileContentWithFallbackToResources methods.
        private static IServiceCollection AddPredefinedContent<TContentSet>(this IServiceCollection services,
            string fileExtension,
            Action<IServiceCollection, Type, IList<string>> registrationAction, // Delegate contains all needed params to avoid closures
            params string[] args)
            where TContentSet : ContentSetBase, new()
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentException(Errors.InvalidFileExtension, nameof(fileExtension));

            registrationAction(services, typeof(TContentSet), args);

            // Register the content set type (TContentSet) with the container.
            services.AddSingleton(sp =>
            {
                IContentSet internalContentSet = sp.GetRequiredService<IContentManager>().GetContentSet(typeof(TContentSet).AssemblyQualifiedName);
                return new TContentSet
                {
                    ContentSet = internalContentSet,
                };
            });

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
