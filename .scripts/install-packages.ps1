[CmdletBinding()]
Param(
    [string]$Version,
    [string]$NuGetSource
)

if(-Not $NuGetSource) {
    Write-Host -ForegroundColor Red "Please specify publishing target ('-NuGetSource')"
    exit -1
}

if (-Not $Version) {
    Write-Host -ForegroundColor Red "Please specify the version to publish"
    Write-Host -ForegroundColor Cyan -NoNewLine "USAGE: "
    Write-Host "install-packages.ps1 -version <version>"
    Write-Host -ForegroundColor Yellow "Existing packages listed below:"
    nuget list -source $NuGetSource -prerelease
    exit -1
}

dotnet build -c Release

Function Publish-Package {
    Param ([string]$Name)
    Write-Host -ForegroundColor Magenta "Packing and publishing $Name package"
    dotnet pack ./src/$Name/$Name.csproj -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -c Release /p:Version=$Version
    dotnet nuget push ./src/$Name/bin/Release/$Name.$Version.nupkg -s $NuGetSource
}

Publish-Package "ContentProvider"
Publish-Package "ContentProvider.Extensions"
Publish-Package "ContentProvider.Formats.Json"
