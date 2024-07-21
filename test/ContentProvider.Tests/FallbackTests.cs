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
