using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContentProvider.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContent(this IServiceCollection services,
        string name, Action<ContentBuilder> builderAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(builderAction);

        ContentBuilder builder = [];
        builderAction(builder);

        ContentSet contentSet = new();
        contentSet.Sources.AddRange(builder.Build());

        services.TryAddKeyedSingleton<IContentSet>(name, contentSet);

        return services;
    }

    public static IServiceCollection AddContent<TContentSet>(
        this IServiceCollection services, Action<ContentBuilder> builderAction)
        where TContentSet : ContentSet, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(builderAction);

        ContentBuilder builder = [];
        builderAction(builder);

        TContentSet contentSet = new();
        contentSet.Sources.AddRange(builder.Build());

        services.TryAddSingleton(contentSet);

        return services;
    }

    public static IServiceCollection AddContent<TContentSetContract, TContentSet>(
        this IServiceCollection services, Action<ContentBuilder> builderAction)
        where TContentSetContract : class, IContentSet
        where TContentSet : ContentSet, TContentSetContract, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(builderAction);

        ContentBuilder builder = [];
        builderAction(builder);

        TContentSet contentSet = new();
        contentSet.Sources.AddRange(builder.Build());

        services.TryAddSingleton<TContentSetContract>(contentSet);

        return services;
    }
}
