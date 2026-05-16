param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

dotnet build .\XrmTool-bravo.csproj -c $Configuration

$nuget = Get-Command nuget -ErrorAction SilentlyContinue
if ($nuget) {
    & $nuget.Source pack .\LucasVerissimo.XrmToolBox.FieldChangeMonitor.nuspec -Properties Configuration=$Configuration -OutputDirectory .\bin\$Configuration
    exit $LASTEXITCODE
}

$localNuget = Join-Path $PSScriptRoot "obj\nuget\nuget.exe"
if (-not (Test-Path $localNuget)) {
    New-Item -ItemType Directory -Force -Path (Split-Path $localNuget) | Out-Null
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $localNuget
}

& $localNuget pack .\LucasVerissimo.XrmToolBox.FieldChangeMonitor.nuspec -Properties Configuration=$Configuration -OutputDirectory .\bin\$Configuration
