#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020-2024 Damian Kulik, Jeevan James

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

using System.Runtime.Serialization;

namespace ContentProvider;

[Serializable]
public class ContentException : Exception
{
    public ContentException()
    {
    }

    public ContentException(string message)
        : base(message)
    {
    }

    public ContentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected ContentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
