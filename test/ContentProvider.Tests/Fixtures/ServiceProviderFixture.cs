// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Extensions.DependencyInjection;
using ContentProvider.Tests.Content;
using ContentProvider.Tests.SqlContent;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace ContentProvider.Tests.Fixtures;

public sealed class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddContent<TextContentSet>(b => b
            .From.ResourcesInExecutingAssembly(o => o.WithResourceNamespace<TextContentSet>().WithFileExtension("txt"))
            .ThenFrom.Memory().Add("Dummy", "Value"));
        services.AddContent("Json", b => b
            .From.ResourcesInExecutingAssembly(o => o.WithResourceNamespace<JsonContentSet>().WithFileExtension("json")));

        services.AddContent("Sqls", builder => builder
            .From.FilesAndResources("./SqlContent", sourceBuilderAction: o => o
                .WithFileExtension("sql")
                .WithResourceNamespace("ContentProvider.Tests.SqlContent")
                .WithNameTransformer(name =>
                {
                    int underscoreIndex = name.IndexOf('_', StringComparison.Ordinal);
                    return name[(underscoreIndex + 1)..];
                })));

        services.AddContent<SqlContentSet>(builder => builder
            .From.FilesAndResources("./SqlContent", sourceBuilderAction: o => o
                .WithFileExtension("sql")
                .WithResourceNamespace("ContentProvider.Tests.SqlContent")
                .WithNameTransformer(name =>
                {
                    int underscoreIndex = name.IndexOf('_', StringComparison.Ordinal);
                    return name[(underscoreIndex + 1)..];
                })));

        ServiceProvider = services.BuildServiceProvider();
    }

    public ServiceProvider ServiceProvider { get; }
}

[CollectionDefinition(nameof(ServiceProviderFixture))]
public sealed class ServiceProviderCollectionFixture : ICollectionFixture<ServiceProviderFixture>;
