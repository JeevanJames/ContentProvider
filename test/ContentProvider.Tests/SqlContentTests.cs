// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Tests.SqlContent;

using Microsoft.Extensions.DependencyInjection;

namespace ContentProvider.Tests;

[Collection(nameof(ServiceProviderFixture))]
public sealed class SqlContentTests(ServiceProviderFixture fixture, ITestOutputHelper output)
{
    private readonly ServiceProviderFixture _fixture = fixture;

    public ITestOutputHelper Output { get; } = output;

    [Theory]
    [InlineData("GetOrders", true)]
    [InlineData("GetCustomers", false)]
    public async Task Can_read_sql_from_named_contentset(string name, bool sourceShouldBeFile)
    {
        IContentSet contentSet = _fixture.ServiceProvider.GetRequiredKeyedService<IContentSet>("Sqls");
        contentSet.ShouldNotBeNull();

        string sql = await contentSet.GetAsStringAsync(name);
        sql.ShouldNotBeNull();
        Output.WriteLine(sql);
        if (sourceShouldBeFile)
            sql.ShouldContain("Source: File");
        else
            sql.ShouldContain("Source: Resource");
    }

    [Theory]
    [InlineData("GetOrders", true)]
    [InlineData("GetCustomers", false)]
    public async Task Can_read_sql_from_typed_contentset(string name, bool sourceShouldBeFile)
    {
        SqlContentSet contentSet = _fixture.ServiceProvider.GetRequiredService<SqlContentSet>();
        contentSet.ShouldNotBeNull();

        string sql = await contentSet.GetAsStringAsync(name);
        sql.ShouldNotBeNull();
        Output.WriteLine(sql);
        if (sourceShouldBeFile)
            sql.ShouldContain("Source: File");
        else
            sql.ShouldContain("Source: Resource");
    }

    [Fact]
    public async Task Can_read_sql_from_contentset_methods()
    {
        SqlContentSet contentSet = _fixture.ServiceProvider.GetRequiredService<SqlContentSet>();
        contentSet.ShouldNotBeNull();

        string getOrdersSql = await contentSet.GetOrders();
        Output.WriteLine(getOrdersSql);
        getOrdersSql.ShouldContain("Source: File");

        string getCustomersSql = await contentSet.GetCustomers();
        Output.WriteLine(getCustomersSql);
        getCustomersSql.ShouldContain("Source: Resource");
    }
}
