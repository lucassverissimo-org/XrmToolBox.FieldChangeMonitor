param(
    [string]$Configuration = "Release",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path $PSScriptRoot -Parent
$packageId = "LucasVerissimo.XrmToolBox.DataverseUsageExplorer"
$toolDirectory = Join-Path $repositoryRoot $packageId
$assemblyPath = Join-Path $repositoryRoot "bin\$Configuration\$packageId.dll"
$sharedAssemblyPath = Join-Path $repositoryRoot "bin\$Configuration\LucasVerissimo.XrmToolBox.Shared.dll"
$nuspecPath = Join-Path $toolDirectory "$packageId.nuspec"
$buildScriptPath = Join-Path $repositoryRoot "build-dataverse-usage-explorer-package.ps1"
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
    Assert-True ($projectUrl -eq "https://github.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor/tree/main/LucasVerissimo.XrmToolBox.DataverseUsageExplorer") "Project URL points to the tool documentation"
    Assert-True ($iconUrl -eq "https://raw.githubusercontent.com/lucassverissimo-org/XrmToolBox.FieldChangeMonitor/main/LucasVerissimo.XrmToolBox.DataverseUsageExplorer/Assets/dataverse-usage-explorer-128.png") "Icon URL points to the public PNG asset"
    Assert-True (Test-Path $sharedAssemblyPath) "Compatible Shared assembly exists"

    $packagePath = Join-Path $repositoryRoot "bin\$Configuration\$packageId.$packageVersion.nupkg"
    Assert-True (Test-Path $packagePath) "Expected NuGet package exists"

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entries = @($archive.Entries | ForEach-Object FullName)

        Assert-True ($entries -contains "lib/net48/Plugins/$packageId.dll") "Plugin assembly is under lib/net48/Plugins"
        Assert-True ($entries -contains "lib/net48/Plugins/LucasVerissimo.XrmToolBox.Shared.dll") "Shared assembly is packaged"
        Assert-True ($entries -contains "dataverse-usage-explorer-128.png") "128 x 128 package icon is included"
        Assert-True ($entries -contains "README.md") "Package README is included"
        Assert-True (-not ($entries -match "FieldChangeMonitor\.dll$")) "Field Change Monitor assembly is not packaged"
        Assert-True (-not ($entries -match "\.pdb$")) "Debug symbols are not included in the release package"
        Assert-True (-not ($entries -match "System\.Memory\.dll$")) "Host framework dependencies are not bundled"

        $iconEntry = $archive.GetEntry("dataverse-usage-explorer-128.png")
        Add-Type -AssemblyName System.Drawing
        $iconStream = $iconEntry.Open()
        try {
            $iconImage = [Drawing.Image]::FromStream($iconStream)
            try {
                Assert-True ($iconImage.Width -eq 128 -and $iconImage.Height -eq 128) "Package icon dimensions are 128 x 128"
            }
            finally {
                $iconImage.Dispose()
            }
        }
        finally {
            $iconStream.Dispose()
        }
    }
    finally {
        $archive.Dispose()
    }

    $pluginReferences = [Reflection.Assembly]::LoadFrom($assemblyPath).GetReferencedAssemblies()
    $sharedReference = $pluginReferences | Where-Object Name -eq "LucasVerissimo.XrmToolBox.Shared"
    Assert-True ($null -ne $sharedReference) "Plugin explicitly references Shared"
    Assert-True ($sharedReference.Version -eq [Version]"1.0.0.0") "Plugin targets the backward-compatible Shared identity"

    if ($failures.Count -gt 0) {
        throw "$($failures.Count) release validation(s) failed."
    }

    Write-Host "Release validation passed: $passed checks." -ForegroundColor Cyan
}
finally {
    Pop-Location
}
