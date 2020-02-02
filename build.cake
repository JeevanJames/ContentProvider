#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "CICD");
var configuration = Argument("configuration", "Release");

string version;
bool isPrerelease = true;

Task("CI")
    .IsDependentOn("Test");

Task("CICD")
    .IsDependentOn("CI")
    .IsDependentOn("Publish");

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

Task("Publish")
    .IsDependentOn("Version")
    .IsDependentOn("Test")
    .Does(() =>
    {
        if (!BuildSystem.IsRunningOnAppVeyor)
        {
            Error("Need to be running on AppVeyor to publish.");
            return;
        }

        var outputDir = new DirectoryPath("./nuget");
        var projects = new string[] { "ContentProvider", "ContentProvider.Extensions" };

        CleanDirectory(outputDir);

        var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            IncludeSource = true,
            IncludeSymbols = true,
            OutputDirectory = outputDir.FullPath,
            ArgumentCustomization = args => args.Append($"/p:Version={version}")
        };

        foreach (string project in projects)
        {
            DotNetCorePack($"./src/{project}/{project}.csproj", settings);

            if (isPrerelease)
                Information($"Publishing to MyGet - {EnvironmentVariable("MYGET_SOURCE")}");
            else
                Information($"Publishing to NuGet - {EnvironmentVariable("NUGET_SOURCE")}");
        }

        var packageFiles = GetFiles("./nuget/*.nupkg");
        foreach (var packageFile in packageFiles)
            AppVeyor.UploadArtifact(packageFile);
    });

RunTarget(target);
