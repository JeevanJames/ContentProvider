#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "CI");
var configuration = Argument("configuration", "Release");

string version;
bool isPrerelease = true;

Task("CI")
    .IsDependentOn("Version")
    .IsDependentOn("Test")
    .Does(() =>
{
});

Task("Version")
    .Does(() =>
    {
        var gitVersion = GitVersion();
        Information($"Branch: {gitVersion.BranchName}");

        isPrerelease = !string.IsNullOrWhiteSpace(gitVersion.PreReleaseTag);

        if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
        {
            version = $"{gitVersion.SemVer}+{AppVeyor.Environment.Build.Number}";
            AppVeyor.UpdateBuildVersion(version);
        }
        else
            version = gitVersion.FullSemVer;

        Information($"Calculated version: {version}");
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
        DotNetCoreTest("./ContentProvider.sln", new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
        });
    });

RunTarget(target);