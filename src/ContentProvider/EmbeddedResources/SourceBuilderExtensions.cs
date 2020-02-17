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
using System.Reflection;

namespace ContentProvider.EmbeddedResources
{
    public static class SourceBuilderExtensions
    {
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


    }
}
