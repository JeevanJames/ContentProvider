// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests;

public sealed class FallbackTests
{
    private readonly IServiceProvider _serviceProvider;

    public FallbackTests()
    {
        IServiceCollection services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void DummyTest()
    {
        _serviceProvider.ShouldNotBeNull();
    }
}
