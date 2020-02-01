var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Default")
    .Does(() =>
{
    Information("Hello Cake!");
});

Task("Build")
    .Does(() =>
{
    DotNetCoreBuild("./ContentProvider.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DotNetCoreTest("./ContentProvider.sln");
    });

RunTarget(target);