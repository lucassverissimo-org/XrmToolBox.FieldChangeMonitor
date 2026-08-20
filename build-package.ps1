param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$packageId = "LucasVerissimo.XrmToolBox.FieldChangeMonitor"
$projectPath = Join-Path $PSScriptRoot "$packageId\$packageId.csproj"
$specificationPath = Join-Path $PSScriptRoot "$packageId\$packageId.nuspec"
$outputDirectory = Join-Path $PSScriptRoot "bin\$Configuration"
$nugetPath = Join-Path $PSScriptRoot "obj\tools\nuget.exe"

dotnet build $projectPath -c $Configuration
if ($LASTEXITCODE -ne 0) {
    throw "The Field Change Monitor build failed. The package will not be created."
}

if (-not (Test-Path $nugetPath)) {
    New-Item -ItemType Directory -Force -Path (Split-Path $nugetPath -Parent) | Out-Null
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nugetPath
}

& $nugetPath pack $specificationPath `
    -Properties "Configuration=$Configuration" `
    -OutputDirectory $outputDirectory

if ($LASTEXITCODE -ne 0) {
    throw "NuGet failed to create the Field Change Monitor package."
}
