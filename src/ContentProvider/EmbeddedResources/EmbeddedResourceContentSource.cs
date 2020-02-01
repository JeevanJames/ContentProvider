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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContentProvider.EmbeddedResources
{
    public sealed class EmbeddedResourceContentSource : ContentSource
    {
        private readonly Dictionary<string, ResourceDetail> _resources = new Dictionary<string, ResourceDetail>();

        public EmbeddedResourceContentSource(IEnumerable<Assembly> assemblies,
            Regex resourceNameMatcher = null,
            string resourceFileExtension = null,
            string rootNamespace = null)
        {
            DiscoverResources(assemblies, resourceNameMatcher, resourceFileExtension, rootNamespace);
        }

        private void DiscoverResources(IEnumerable<Assembly> assemblies,
            Regex resourceNameMatcher, string resourceFileExtension, string rootNamespace)
        {
            var contentNameGetter = string.IsNullOrWhiteSpace(rootNamespace)
                ? (Func<string, string>)(res => res)
                : res => res.Substring(rootNamespace.Length + 1);

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<string> resourceNames = assembly.GetManifestResourceNames();
                if (resourceNameMatcher != null)
                    resourceNames = resourceNames.Where(res => resourceNameMatcher.IsMatch(res));
                if (!string.IsNullOrWhiteSpace(resourceFileExtension))
                    resourceNames = resourceNames.Where(res => res.EndsWith($".{resourceFileExtension}", System.StringComparison.OrdinalIgnoreCase));

                foreach (string resourceName in resourceNames.ToList())
                {
                    string contentName = contentNameGetter(resourceName);
                    _resources.Add(contentName, new ResourceDetail(assembly, resourceName));
                }
            }
        }

        public async override Task<(bool success, string content)> TryLoadAsString(string name)
        {
            if (!_resources.TryGetValue(name, out ResourceDetail resourceDetail))
                return (false, null);

            using Stream resourceStream = resourceDetail.Assembly.GetManifestResourceStream(resourceDetail.ResourceName);
            var reader = new StreamReader(resourceStream);

#pragma warning disable S1854 // Unused assignments should be removed
            string content = await reader.ReadToEndAsync().ConfigureAwait(false);
#pragma warning restore S1854 // Unused assignments should be removed
            return (true, content);
        }
    }

    internal readonly struct ResourceDetail
    {
        internal ResourceDetail(Assembly assembly, string resourceName)
        {
            Assembly = assembly;
            ResourceName = resourceName;
        }

        internal Assembly Assembly { get; }

        internal string ResourceName { get; }
    }
}
