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
using System.Threading.Tasks;

namespace ContentProvider
{
    public abstract class ContentSetBase : IContentSet
    {
        private readonly ContentSet _contentSet;

        protected ContentSetBase()
        {
            ContentSetAttribute attribute = GetType().GetCustomAttribute<ContentSetAttribute>(false);
            if (attribute is null)
                throw new InvalidOperationException($"Decorate the ${GetType().FullName} class with a {typeof(ContentSetAttribute).FullName} attribute to indicate the name of the content to load.");
            _contentSet = ContentManager.Get(attribute.Name);
        }

        protected ContentSetBase(string name)
        {
            _contentSet = ContentManager.Get(name);
        }

        public Task<string> GetAsString(string name)
        {
            return _contentSet.GetAsString(name);
        }
    }
}
