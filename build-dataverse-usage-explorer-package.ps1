param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$project = ".\LucasVerissimo.XrmToolBox.DataverseUsageExplorer\LucasVerissimo.XrmToolBox.DataverseUsageExplorer.csproj"
$specification = ".\LucasVerissimo.XrmToolBox.DataverseUsageExplorer\LucasVerissimo.XrmToolBox.DataverseUsageExplorer.nuspec"
$outputDirectory = ".\bin\$Configuration"

dotnet build $project -c $Configuration
if ($LASTEXITCODE -ne 0) {
    throw "A compilacao do Dataverse Usage Explorer falhou. O pacote nao sera criado."
}

$nugetCommand = Get-Command nuget -ErrorAction SilentlyContinue
if ($nugetCommand) {
    & $nugetCommand.Source pack $specification -OutputDirectory $outputDirectory
    exit $LASTEXITCODE
}

$localNuget = Join-Path $PSScriptRoot "obj\nuget\nuget.exe"
if (-not (Test-Path $localNuget)) {
    New-Item -ItemType Directory -Force -Path (Split-Path $localNuget) | Out-Null
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $localNuget
}

& $localNuget pack $specification -OutputDirectory $outputDirectory
exit $LASTEXITCODE
