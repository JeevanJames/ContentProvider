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
using System.Reflection;

namespace ContentProvider.EmbeddedResources
{
    public static class SourceBuilderExtensions
    {
        /// <summary>
        ///     Registers a <see cref="ContentSource"/> for content from embedded resources in the
        ///     executing assembly.
        /// </summary>
        /// <param name="builder">The <see cref="ContentSourceBuilder"/> instance.</param>
        /// <param name="options">Options for registering the content source.</param>
        /// <returns>An instance of <see cref="ContentBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="builder"/> or <paramref name="options"/> parameters are
        ///     <c>null</c>.
        /// </exception>
        public static ContentBuilder ResourcesInExecutingAssembly(this ContentSourceBuilder builder,
            EmbeddedResourceContentSourceOptions options)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            return builder.Source(new EmbeddedResourceContentSource(
                new[] { Assembly.GetCallingAssembly() }, options));
        }

        /// <summary>
        ///     Registers a <see cref="ContentSource"/> for content from embedded resources in the
        ///     specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContentSourceBuilder"/> instance.</param>
        /// <param name="assembly">The assembly to load resources from.</param>
        /// <param name="options">Options for registering the content source.</param>
        /// <returns>An instance of <see cref="ContentBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="builder"/>, <paramref name="assembly"/> or
        ///     <paramref name="options"/> parameters are <c>null</c>.
        /// </exception>
        public static ContentBuilder ResourcesIn(this ContentSourceBuilder builder, Assembly assembly,
            EmbeddedResourceContentSourceOptions options)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            return builder.Source(new EmbeddedResourceContentSource(new[] { assembly }, options));
        }

        /// <summary>
        ///     Registers a <see cref="ContentSource"/> for content from embedded resources in the
        ///     specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContentSourceBuilder"/> instance.</param>
        /// <param name="assemblies">The assembly to load resources from.</param>
        /// <param name="options">Options for registering the content source.</param>
        /// <returns>An instance of <see cref="ContentBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="builder"/>, <paramref name="assemblies"/>
        ///     or <paramref name="options"/> parameters are <c>null</c>.
        /// </exception>
        public static ContentBuilder ResourcesIn(this ContentSourceBuilder builder, IEnumerable<Assembly> assemblies,
            EmbeddedResourceContentSourceOptions options)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            if (assemblies is null)
                throw new ArgumentNullException(nameof(assemblies));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            return builder.Source(new EmbeddedResourceContentSource(assemblies, options));
        }
    }
}
