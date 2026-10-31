param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [switch]$SkipBuild,
    [switch]$PrepareOnly
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
$packageId = "LucasVerissimo.XrmToolBox.DataverseMappingManager"
$project = Join-Path $PSScriptRoot "$packageId\$packageId.csproj"
$runtimeDirectory = Join-Path $PSScriptRoot "bin\$Configuration"
$executable = Join-Path $runtimeDirectory "XrmToolBox.exe"
$assembly = Join-Path $runtimeDirectory "$packageId.dll"
$pluginsDirectory = Join-Path $runtimeDirectory "Plugins"
$launcher = Join-Path $runtimeDirectory "Start-DataverseMappingManager.cmd"

if (-not $SkipBuild) {
    dotnet build $project -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        throw "The Dataverse Mapping Manager $Configuration build failed."
    }
}

if (-not (Test-Path -LiteralPath $executable)) {
    throw "XrmToolBox.exe was not found at $executable"
}

if (-not (Test-Path -LiteralPath $assembly)) {
    throw "The plugin was not found at $assembly"
}

New-Item -Path $pluginsDirectory -ItemType Directory -Force | Out-Null
Copy-Item -LiteralPath $assembly -Destination $pluginsDirectory -Force
$symbols = Join-Path $runtimeDirectory "$packageId.pdb"
if (Test-Path -LiteralPath $symbols) {
    Copy-Item -LiteralPath $symbols -Destination $pluginsDirectory -Force
}

# Invalidate the plugin discovery cache after copying the assembly.
$manifest = Join-Path $pluginsDirectory "manifest.json"
if (Test-Path -LiteralPath $manifest) {
    Remove-Item -LiteralPath $manifest -Force
}

$launcherLines = @(
    '@echo off',
    ('powershell.exe -NoProfile -File "%~dp0..\..\run-dataverse-mapping-manager.ps1" -Configuration {0}' -f $Configuration)
)
$launcherLines | Set-Content -LiteralPath $launcher -Encoding Ascii

if (-not $PrepareOnly) {
    # This is the interactive application requested by the user.
    Start-Process -FilePath $executable `
        -WorkingDirectory $runtimeDirectory `
        -ArgumentList ('/overridepath:"{0}" /plugin:"Dataverse Mapping Manager"' -f $runtimeDirectory)
}

Write-Host "Plugin prepared in: $pluginsDirectory" -ForegroundColor Cyan
Write-Host "Open using: $launcher" -ForegroundColor Cyan
Write-Host "This profile stores tools, connections and settings in: $runtimeDirectory"
