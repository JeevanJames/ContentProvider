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
using System.Threading.Tasks;

namespace ContentProvider
{
    public abstract class ContentSetBase : IContentSet
    {
        private IContentSet _contentSet;

        public Task<string> GetAsStringAsync(string name)
        {
            return _contentSet.GetAsStringAsync(name);
        }

        public Task<byte[]> GetAsBinaryAsync(string name)
        {
            return _contentSet.GetAsBinaryAsync(name);
        }

        internal IContentSet ContentSet
        {
            get => _contentSet;
            set
            {
                if (_contentSet != null)
                    throw new InvalidOperationException(Errors.CannotAssignInternalContentSet);
                _contentSet = value;
            }
        }
    }
}
