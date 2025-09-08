namespace ContentProvider;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public abstract class ContentSourceOptionsBuilder<TSelf, TContentSource, TContentSourceOptions>
    where TSelf : ContentSourceOptionsBuilder<TSelf, TContentSource, TContentSourceOptions>
    where TContentSource : ContentSource
    where TContentSourceOptions : ContentSourceOptions, new()
{
    public abstract TContentSource Build();

    protected TContentSourceOptions Options { get; } = new();

    public TSelf WithNameTransformer(Func<string, string> nameTransformer)
    {
        Options.NameTransformer = nameTransformer;
        return (TSelf)this;
    }
}
