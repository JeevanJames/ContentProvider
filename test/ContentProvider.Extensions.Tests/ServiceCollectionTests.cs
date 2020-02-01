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

using System.Threading.Tasks;
using ContentProvider.EmbeddedResources;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests
{
    public sealed class ServiceCollectionTests
    {
        private readonly ServiceProvider _serviceProvider;

        public ServiceCollectionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddContentProvider("Text", b => b
                .From.ResourcesInExecutingAssembly(rootNamespace: typeof(ServiceCollectionTests).Namespace));
            services.AddContentProvider("Json", b => b
                .From.ResourcesInExecutingAssembly(rootNamespace: typeof(ServiceCollectionTests).Namespace, resourceFileExtension: "json"));
            services.AddContentSet<TextContentSet>();
            services.AddContentSet<JsonContentSet>("Json");
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Can_inject_service_manager_and_retrieve_content()
        {
            var manager = _serviceProvider.GetService<IContentManager>();
            manager.ShouldNotBeNull();

            ContentSet contentSet = manager.Get("Text");
            contentSet.ShouldNotBeNull();

            string value = await contentSet.Get("Content.txt");
            value.ShouldBe("This is the content.");
        }

        [Fact]
        public async Task Can_inject_content_set_with_attribute()
        {
            var textContentSet = _serviceProvider.GetService<TextContentSet>();
            textContentSet.ShouldNotBeNull();

            string value = await textContentSet.Get("Content.txt");
            value.ShouldBe("This is the content.");
        }

        [Fact]
        public async Task Can_inject_content_set_with_name()
        {
            var jsonContentSet = _serviceProvider.GetService<JsonContentSet>();
            jsonContentSet.ShouldNotBeNull();

            string value = await jsonContentSet.Get("Content.json");
            value.ShouldNotBeNull();

            //value.ShouldBe("This is the content.");
        }
    }

    [ContentSet("Text")]
    public sealed class TextContentSet : ContentSetBase
    {
    }

    public sealed class JsonContentSet : ContentSetBase
    {
        public JsonContentSet(string name)
            : base(name)
        {
        }
    }
}
