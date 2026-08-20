param([switch]$SkipBuild)

$ErrorActionPreference = "Stop"
$repositoryRoot = $PSScriptRoot
$project = Join-Path $repositoryRoot "LucasVerissimo.XrmToolBox.DataverseUsageExplorer\LucasVerissimo.XrmToolBox.DataverseUsageExplorer.csproj"
$debugDirectory = Join-Path $repositoryRoot "bin\Debug"
$executable = Join-Path $debugDirectory "XrmToolBox.exe"
$plugin = Join-Path $debugDirectory "Plugins\LucasVerissimo.XrmToolBox.DataverseUsageExplorer.dll"
$testProfile = Join-Path $env:TEMP "XrmToolBox-DataverseUsageExplorer-Test"

if (-not $SkipBuild) {
    dotnet build $project -c Debug
    if ($LASTEXITCODE -ne 0) {
        throw "The Dataverse Usage Explorer Debug build failed."
    }
}

if (-not (Test-Path -LiteralPath $executable)) {
    throw "XrmToolBox.exe was not found at $executable"
}

if (-not (Test-Path -LiteralPath $plugin)) {
    throw "The plugin was not found at $plugin"
}

New-Item -Path $testProfile -ItemType Directory -Force | Out-Null

$originalAppData = $env:APPDATA
try {
    $env:APPDATA = $testProfile
    Start-Process -FilePath $executable -WorkingDirectory $debugDirectory
}
finally {
    $env:APPDATA = $originalAppData
}

Write-Host "XrmToolBox started with isolated profile: $testProfile" -ForegroundColor Cyan
