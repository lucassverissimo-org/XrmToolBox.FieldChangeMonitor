param(
    [string]$Configuration = "Release",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path $PSScriptRoot -Parent
$packageId = "LucasVerissimo.XrmToolBox.DataverseMappingManager"
$toolDirectory = Join-Path $repositoryRoot $packageId
$assemblyPath = Join-Path $repositoryRoot "bin\$Configuration\$packageId.dll"
$nuspecPath = Join-Path $toolDirectory "$packageId.nuspec"
$buildScriptPath = Join-Path $repositoryRoot "build-dataverse-mapping-manager-package.ps1"
$failures = [System.Collections.Generic.List[string]]::new()
$passed = 0

function Assert-True {
    param(
        [bool]$Condition,
        [string]$Message
    )

    if ($Condition) {
        $script:passed++
        Write-Host "OK: $Message" -ForegroundColor Green
        return
    }

    $script:failures.Add($Message)
    Write-Host "FAILED: $Message" -ForegroundColor Red
}

function Get-NormalizedVersion {
    param([Version]$Version)

    $parts = @($Version.Major, $Version.Minor, $Version.Build, $Version.Revision)
    while ($parts.Count -gt 3 -and $parts[-1] -eq 0) {
        $parts = $parts[0..($parts.Count - 2)]
    }

    return $parts -join "."
}

function Get-AssemblyReferenceNames {
    param([string]$AssemblyPath)

    $stream = [IO.File]::OpenRead($AssemblyPath)
    try {
        $reader = [Reflection.PortableExecutable.PEReader]::new($stream)
        try {
            $metadata = [Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($reader)
            return @(
                foreach ($handle in $metadata.AssemblyReferences) {
                    $reference = $metadata.GetAssemblyReference($handle)
                    $metadata.GetString($reference.Name)
                }
            )
        }
        finally {
            $reader.Dispose()
        }
    }
    finally {
        $stream.Dispose()
    }
}

Push-Location $repositoryRoot
try {
    if (-not $SkipBuild) {
        & $buildScriptPath -Configuration $Configuration
        Assert-True ($LASTEXITCODE -eq 0) "Release package build succeeds"
    }

    [xml]$specification = Get-Content $nuspecPath -Raw
    $namespace = [System.Xml.XmlNamespaceManager]::new($specification.NameTable)
    $namespace.AddNamespace("n", $specification.DocumentElement.NamespaceURI)

    $packageVersion = $specification.SelectSingleNode("/n:package/n:metadata/n:version", $namespace).InnerText
    $projectUrl = $specification.SelectSingleNode("/n:package/n:metadata/n:projectUrl", $namespace).InnerText
    $iconUrl = $specification.SelectSingleNode("/n:package/n:metadata/n:iconUrl", $namespace).InnerText
    $assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($assemblyPath).Version
    $normalizedAssemblyVersion = Get-NormalizedVersion $assemblyVersion

    Assert-True ($packageVersion -eq $normalizedAssemblyVersion) "Package and plugin assembly versions match"
    Assert-True ($projectUrl -eq "https://github.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor/tree/main/LucasVerissimo.XrmToolBox.DataverseMappingManager") "Project URL points to the tool documentation"
    Assert-True ($iconUrl -eq "https://raw.githubusercontent.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor/main/LucasVerissimo.XrmToolBox.DataverseMappingManager/Assets/dataverse-mapping-manager-128.png") "Icon URL points to the public PNG asset"
    Assert-True ($null -eq $specification.SelectSingleNode("/n:package/n:metadata/n:icon", $namespace)) "Embedded icon metadata is absent for Tool Library compatibility"

    $packagePath = Join-Path $repositoryRoot "bin\$Configuration\$packageId.$packageVersion.nupkg"
    Assert-True (Test-Path $packagePath) "Expected NuGet package exists"
    & (Join-Path $repositoryRoot 'scripts/test-tool-library-metadata.ps1') -NuspecPath $nuspecPath -PackagePath $packagePath

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entries = @($archive.Entries | ForEach-Object FullName)

        $pluginAssemblies = @($entries | Where-Object { $_ -match "^lib/net48/Plugins/[^/]+\.dll$" })
        Assert-True ($entries -contains "lib/net48/Plugins/$packageId.dll") "Plugin assembly is under lib/net48/Plugins"
        Assert-True ($pluginAssemblies.Count -eq 1) "Package contains exactly one plugin assembly"
        Assert-True (-not ($entries -contains "lib/net48/Plugins/LucasVerissimo.XrmToolBox.Shared.dll")) "Shared assembly is not packaged separately"
        Assert-True (-not ($entries -contains "dataverse-mapping-manager-128.png")) "Logo is served by iconUrl rather than embedded in the package"
        Assert-True ($entries -contains "README.md") "Package README is included"
        Assert-True (-not ($entries -match "FieldChangeMonitor\.dll$")) "Field Change Monitor assembly is not packaged"
        Assert-True (-not ($entries -match "\.pdb$")) "Debug symbols are not included in the release package"
        Assert-True (-not ($entries -match "System\.Memory\.dll$")) "Host framework dependencies are not bundled"

    }
    finally {
        $archive.Dispose()
    }

    $pluginAssembly = [Reflection.Assembly]::LoadFrom($assemblyPath)
    $sharedReference = Get-AssemblyReferenceNames $assemblyPath | Where-Object { $_ -eq "LucasVerissimo.XrmToolBox.Shared" }
    Assert-True ($null -eq $sharedReference) "Plugin has no external Shared reference"
    Assert-True ($null -ne $pluginAssembly.GetType("LucasVerissimo.XrmToolBox.Shared.BusinessLogic.DataverseMetadataService", $false)) "Plugin contains the Shared Project implementation"

    if ($failures.Count -gt 0) {
        throw "$($failures.Count) release validation(s) failed."
    }

    Write-Host "Release validation passed: $passed checks." -ForegroundColor Cyan
}
finally {
    Pop-Location
}
