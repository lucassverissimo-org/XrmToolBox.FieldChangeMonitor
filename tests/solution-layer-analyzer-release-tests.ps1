param(
    [string]$Configuration = "Release",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path $PSScriptRoot -Parent
$packageId = "LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer"
$toolDirectory = Join-Path $repositoryRoot $packageId
$assemblyPath = Join-Path $repositoryRoot "bin\$Configuration\$packageId.dll"
$nuspecPath = Join-Path $toolDirectory "$packageId.nuspec"
$buildScriptPath = Join-Path $repositoryRoot "build-solution-layer-analyzer-package.ps1"
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
    $assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($assemblyPath).Version
    Assert-True ($packageVersion -eq (Get-NormalizedVersion $assemblyVersion)) "Package and plugin assembly versions match"

    $packagePath = Join-Path $repositoryRoot "bin\$Configuration\$packageId.$packageVersion.nupkg"
    Assert-True (Test-Path $packagePath) "Expected NuGet package exists"

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        $entries = @($archive.Entries | ForEach-Object FullName)
        $pluginAssemblies = @($entries | Where-Object { $_ -match "^lib/net48/Plugins/[^/]+\.dll$" })
        Assert-True ($entries -contains "lib/net48/Plugins/$packageId.dll") "Plugin assembly is under lib/net48/Plugins"
        Assert-True ($pluginAssemblies.Count -eq 1) "Package contains exactly one plugin assembly"
        Assert-True (-not ($entries -contains "lib/net48/Plugins/LucasVerissimo.XrmToolBox.Shared.dll")) "Shared assembly is not packaged separately"
        Assert-True ($entries -contains "solution-layer-analyzer-128.png") "128 x 128 package icon is included"
        Assert-True ($entries -contains "README.md") "Package README is included"
        Assert-True (-not ($entries -match "FieldChangeMonitor\.dll$")) "Field Change Monitor assembly is not packaged"
        Assert-True (-not ($entries -match "DataverseUsageExplorer\.dll$")) "Dataverse Usage Explorer assembly is not packaged"
        Assert-True (-not ($entries -match "\.pdb$")) "Debug symbols are not included in the release package"
    }
    finally {
        $archive.Dispose()
    }

    $pluginAssembly = [Reflection.Assembly]::LoadFrom($assemblyPath)
    $controlType = $pluginAssembly.GetType("LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.SolutionLayerAnalyzerControl", $false)
    $pluginType = $pluginAssembly.GetType("LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.MyPlugin", $false)
    $solutionPickerType = $pluginAssembly.GetType("LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Controls.SolutionPickerControl", $false)
    $solutionInfoType = $pluginAssembly.GetType("LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models.SolutionInfo", $false)
    Assert-True ($null -ne $controlType) "Plugin control is compiled"
    Assert-True ($null -ne $pluginType) "MEF plugin entry point is compiled"
    Assert-True ($null -ne $solutionPickerType) "Searchable solution grid selector is compiled"
    Assert-True ($controlType.BaseType.FullName -eq "XrmToolBox.Extensibility.MultipleConnectionsPluginControlBase") "Control uses the XrmToolBox multiple-connection base class"
    Assert-True ($null -ne $pluginAssembly.GetType("LucasVerissimo.XrmToolBox.Shared.BusinessLogic.DataverseQueryService", $false)) "Plugin contains Shared Project query infrastructure"
    Assert-True (-not ((Get-AssemblyReferenceNames $assemblyPath) -contains "LucasVerissimo.XrmToolBox.Shared")) "Plugin has no external Shared reference"
    $plugin = [Activator]::CreateInstance($pluginType)
    $pluginControl = $plugin.GetControl()
    try {
        Assert-True ($pluginControl.GetType() -eq $controlType -and $pluginControl.Controls.Count -gt 0) "Plugin creates and initializes its WinForms control"
    }
    finally {
        $pluginControl.Dispose()
    }

    $solutionPicker = [Activator]::CreateInstance($solutionPickerType)
    try {
        $solution = [Activator]::CreateInstance($solutionInfoType)
        $solution.SolutionId = [Guid]::NewGuid()
        $solution.FriendlyName = "Display Name"
        $solution.UniqueName = "logical_unique_name"
        $solution.Version = "1.0.0.0"
        $solutionListType = [Collections.Generic.List``1].MakeGenericType($solutionInfoType)
        $solutionList = [Activator]::CreateInstance($solutionListType)
        $solutionList.Add($solution)
        $solutionPicker.SetSolutions($solutionList)

        $popupField = $solutionPickerType.GetField("popup", [Reflection.BindingFlags]"Instance, NonPublic")
        $popup = $popupField.GetValue($solutionPicker)
        $popup.SelectSolution($solution)
        $acceptMethod = $solutionPickerType.GetMethod("PopupSolutionAccepted", [Reflection.BindingFlags]"Instance, NonPublic")
        $acceptMethod.Invoke($solutionPicker, @($popup, [EventArgs]::Empty)) | Out-Null
        Assert-True ($solutionPicker.SelectedSolution.SolutionId -eq $solution.SolutionId) "Solution grid selector returns the selected typed solution"
    }
    finally {
        $solutionPicker.Dispose()
    }

    if ($failures.Count -gt 0) {
        throw "$($failures.Count) release validation(s) failed."
    }

    Write-Host "Release validation passed: $passed checks." -ForegroundColor Cyan
}
finally {
    Pop-Location
}
