// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider;

public class ContentException : Exception
{
    public ContentException()
    {
    }

    public ContentException(string message) : base(message)
    {
    }

    public ContentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
