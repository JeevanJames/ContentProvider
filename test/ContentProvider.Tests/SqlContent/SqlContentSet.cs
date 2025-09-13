namespace ContentProvider.Tests.SqlContent;

public sealed class SqlContentSet : ContentSet
{
    public async Task<string> GetOrders() => await LoadAsStringAsync();

    public async Task<string> GetCustomers() => await LoadAsStringAsync();
}
