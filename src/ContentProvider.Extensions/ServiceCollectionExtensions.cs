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
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace ContentProvider
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContentProvider(this IServiceCollection services,
            string name,
            Action<ContentBuilder> sourceBuilder)
        {
            // TODO: Is it okay to repeat this call
            services.AddSingleton<IContentManager, ContentManagerImpl>();

            var builder = new ContentBuilder();
            sourceBuilder(builder);
            ContentManager.Register(name, builder.Build());

            return services;
        }

        public static IServiceCollection AddContent<TContent>(this IServiceCollection services, string name)
            where TContent : ContentBase
        {
            services.AddSingleton(_ =>
            {
                var constructor = Array.Find(typeof(TContent).GetConstructors(), ctor =>
                    {
                        ParameterInfo[] parameters = ctor.GetParameters();
                        return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
                    });
                if (constructor is null)
                    throw new ArgumentException($"Specific content type should contain a constructor with a single string parameter.");

                object contentObject = constructor.Invoke(new[] { name });
                return (TContent)contentObject;
            });
            return services;
        }

        public static IServiceCollection AddContent<TContent>(this IServiceCollection services)
            where TContent : ContentBase, new()
        {
            services.AddSingleton<TContent>(_ => new TContent());
            return services;
        }

        public static IServiceCollection AddFileContent(this IServiceCollection services)
        {
            //TODO:
            return services;
        }
    }
}
