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

            // TODO: Is it okay to repeat this call
            services.AddSingleton<IContentManager, ContentManagerImpl>();

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
            services.AddContent(name, sourceBuilder);

            services.AddSingleton(sp =>
            {
                ContentSet internalContentSet = sp.GetRequiredService<IContentManager>().GetContentSet(name);
                var contentSet = new TContentSet
                {
                    ContentSet = internalContentSet,
                };
                return contentSet;
            });

            return services;
        }

        public static IServiceCollection AddFileContent(this IServiceCollection services,
            string fileExtension,
            string rootNamespace,
            string baseDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentException(Errors.InvalidFileExtension, nameof(fileExtension));
            if (string.IsNullOrWhiteSpace(rootNamespace))
                throw new ArgumentException(Errors.InvalidRootNamespace, nameof(rootNamespace));

            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = Directory.GetCurrentDirectory();

            return services.AddContent(fileExtension, builder =>
                builder
                    .From.FilesIn(baseDirectory, $"*.{fileExtension}", SearchOption.AllDirectories)
                    .From.ResourcesInExecutingAssembly(resourceFileExtension: fileExtension, rootNamespace: rootNamespace));
        }
    }
}
