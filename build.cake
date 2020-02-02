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
        // Only run on AppVeyor
        if (!BuildSystem.IsRunningOnAppVeyor)
        {
            Error("Need to be running on AppVeyor to publish.");
            return;
        }

        // Clean nuget output path
        var outputDir = new DirectoryPath("./nuget");
        CleanDirectory(outputDir);

        var packSettings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            IncludeSource = true,
            IncludeSymbols = true,
            OutputDirectory = outputDir.FullPath,
            ArgumentCustomization = args => args.Append($"/p:Version={version}")
        };

        // Package the src projects
        var projects = new string[] { "ContentProvider", "ContentProvider.Extensions" };
        foreach (string project in projects)
            DotNetCorePack($"./src/{project}/{project}.csproj", packSettings);

        //Figure out the feed sources and API keys.
        string source, apiKey, symbolSource, symbolApiKey;
        if (isPrerelease)
        {
            // Pre-release version, so publish to MyGet
            source = EnvironmentVariable("MYGET_SOURCE");
            apiKey = EnvironmentVariable("MYGET_APIKEY");
            symbolSource = EnvironmentVariable("MYGET_SYMBOL_SOURCE");
            symbolApiKey = EnvironmentVariable("MYGET_SYMBOL_APIKEY");
            Information($"Publishing to MyGet - {source}");
        }
        else
        {
            // Release version, so publish to NuGet
            source = EnvironmentVariable("NUGET_SOURCE");
            apiKey = EnvironmentVariable("NUGET_APIKEY");
            symbolSource = EnvironmentVariable("NUGET_SYMBOL_SOURCE");
            symbolApiKey = EnvironmentVariable("NUGET_SYMBOL_APIKEY");
            Information($"Publishing to NuGet - {source}");
        }

        // Find all created .nupkg files (not symbols)
        var packageFiles = GetFiles("./nuget/*.nupkg", new GlobberSettings
        {
            FilePredicate = file => !file.Path.FullPath.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase),
        });

        foreach (var packageFile in packageFiles)
        {
            // Upload the package as an AppVeyor artifact
            AppVeyor.UploadArtifact(packageFile);
        
            // Publish the package to the feed
            var pushSettings = new DotNetCoreNuGetPushSettings
            {
                ApiKey = apiKey,
                Source = source,
            };
            if (isPrerelease)
            {
                pushSettings.SymbolSource = symbolSource;
                pushSettings.SymbolApiKey = symbolApiKey;
            }

            DotNetCoreNuGetPush(packageFile.FullPath, pushSettings);
        }
    });

RunTarget(target);
