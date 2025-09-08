using Microsoft.Extensions.DependencyInjection;

namespace ContentProvider.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static IContentSet? GetContent(this IServiceProvider services, string name)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return services.GetKeyedService<IContentSet>(name);
    }

    public static IContentSet GetRequiredContent(this IServiceProvider services, string name)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return services.GetRequiredKeyedService<IContentSet>(name);
    }
}
