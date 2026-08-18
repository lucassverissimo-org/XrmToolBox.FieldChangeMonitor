[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+\.\d+$')]
    [string]$Version,

    [string]$ReleaseNotes,

    [string]$ReleaseNotesFile,

    [switch]$Publish,

    [switch]$Yes,

    [switch]$SkipAvailabilityCheck
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$projectRoot = $PSScriptRoot
$packageId = "LucasVerissimo.XrmToolBox.FieldChangeMonitor"
$toolProjectDirectory = Join-Path $projectRoot $packageId
$assemblyInfoPath = Join-Path $toolProjectDirectory "Properties\AssemblyInfo.cs"
$nuspecPath = Join-Path $toolProjectDirectory "$packageId.nuspec"
$buildScriptPath = Join-Path $projectRoot "build-package.ps1"
$releaseDirectory = Join-Path $projectRoot "bin\Release"
$nugetSource = "https://api.nuget.org/v3/index.json"
$nugetVersionsUrl = "https://api.nuget.org/v3-flatcontainer/$($packageId.ToLowerInvariant())/index.json"

function Set-RegexValue {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Pattern,
        [Parameter(Mandatory = $true)][string]$Value,
        [System.Text.RegularExpressions.RegexOptions]$Options = [System.Text.RegularExpressions.RegexOptions]::None
    )

    $content = [System.IO.File]::ReadAllText($Path)
    $regex = [System.Text.RegularExpressions.Regex]::new($Pattern, $Options)
    if (-not $regex.IsMatch($content)) {
        throw "Nao foi possivel localizar o metadado esperado em '$Path'."
    }

    $updated = $regex.Replace(
        $content,
        [System.Text.RegularExpressions.MatchEvaluator]{
            param($match)
            return $match.Groups[1].Value + $Value + $match.Groups[2].Value
        },
        1)
    [System.IO.File]::WriteAllText($Path, $updated, [System.Text.UTF8Encoding]::new($false))
}

function Get-CurrentAssemblyVersion {
    $content = [System.IO.File]::ReadAllText($assemblyInfoPath)
    $match = [regex]::Match($content, 'AssemblyVersion\("([^"]+)"\)')
    if (-not $match.Success) {
        throw "AssemblyVersion nao encontrado em '$assemblyInfoPath'."
    }

    return $match.Groups[1].Value
}

function Test-NuGetVersionExists {
    param([Parameter(Mandatory = $true)][string]$RequestedVersion)

    try {
        $response = Invoke-RestMethod -Uri $nugetVersionsUrl -Method Get
        return @($response.versions) -contains $RequestedVersion.ToLowerInvariant()
    }
    catch {
        Write-Warning "Nao foi possivel consultar as versoes existentes no NuGet: $($_.Exception.Message)"
        return $null
    }
}

if ($ReleaseNotes -and $ReleaseNotesFile) {
    throw "Use somente -ReleaseNotes ou -ReleaseNotesFile."
}

if ($ReleaseNotesFile) {
    $resolvedNotesFile = (Resolve-Path -LiteralPath $ReleaseNotesFile).Path
    $ReleaseNotes = [System.IO.File]::ReadAllText($resolvedNotesFile).Trim()
}

if (-not $Version) {
    $Version = Get-CurrentAssemblyVersion
}

Write-Host "Preparando $packageId $Version" -ForegroundColor Cyan

# NuGet normaliza o quarto segmento quando ele e zero (por exemplo, 1.2.0.0 vira 1.2.0).
# O assembly continua usando os quatro segmentos, enquanto o pacote usa a forma normalizada.
$versionParts = $Version.Split('.')
$packageVersion = if ($versionParts.Length -eq 4 -and $versionParts[3] -eq '0') {
    ($versionParts[0..2] -join '.')
}
else {
    $Version
}

Set-RegexValue -Path $assemblyInfoPath -Pattern '(AssemblyVersion\(")[^"]+("\)\])' -Value $Version
Set-RegexValue -Path $assemblyInfoPath -Pattern '(AssemblyFileVersion\(")[^"]+("\)\])' -Value $Version
Set-RegexValue -Path $nuspecPath -Pattern '(<version>)[^<]+(</version>)' -Value $Version

if (-not [string]::IsNullOrWhiteSpace($ReleaseNotes)) {
    Set-RegexValue `
        -Path $nuspecPath `
        -Pattern '(<releaseNotes>).*?(</releaseNotes>)' `
        -Value $ReleaseNotes.Trim() `
        -Options ([System.Text.RegularExpressions.RegexOptions]::Singleline)
}

$packagePath = Join-Path $releaseDirectory "$packageId.$packageVersion.nupkg"
$releaseDllPath = Join-Path $releaseDirectory "$packageId.dll"
if (Test-Path -LiteralPath $releaseDllPath) {
    try {
        $releaseDllLockTest = [System.IO.File]::Open(
            $releaseDllPath,
            [System.IO.FileMode]::Open,
            [System.IO.FileAccess]::ReadWrite,
            [System.IO.FileShare]::None)
        $releaseDllLockTest.Dispose()
    }
    catch {
        throw "A DLL de Release esta em uso. Feche o XrmToolBox e execute o script novamente. Arquivo: $releaseDllPath"
    }
}

if (Test-Path -LiteralPath $packagePath) {
    $resolvedReleaseDirectory = [System.IO.Path]::GetFullPath($releaseDirectory).TrimEnd('\') + '\'
    $resolvedPackagePath = [System.IO.Path]::GetFullPath($packagePath)
    if (-not $resolvedPackagePath.StartsWith($resolvedReleaseDirectory, [StringComparison]::OrdinalIgnoreCase)) {
        throw "O pacote calculado ficou fora da pasta de Release."
    }

    Remove-Item -LiteralPath $resolvedPackagePath -Force
}

& $buildScriptPath -Configuration Release
if ($LASTEXITCODE -ne 0) {
    throw "A compilacao ou o empacotamento falhou."
}

if (-not (Test-Path -LiteralPath $packagePath)) {
    throw "O pacote esperado nao foi criado: $packagePath"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    $nuspecEntry = $archive.GetEntry("$packageId.nuspec")
    $dllEntry = $archive.GetEntry("lib/net48/Plugins/$packageId.dll")
    $sharedDllEntry = $archive.GetEntry("lib/net48/Plugins/LucasVerissimo.XrmToolBox.Shared.dll")
    if ($null -eq $nuspecEntry -or $null -eq $dllEntry -or $null -eq $sharedDllEntry) {
        throw "O pacote nao contem o nuspec, a DLL do plugin ou a DLL Shared na pasta lib/net48/Plugins."
    }

    $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())
    try {
        [xml]$packageXml = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }

    $namespace = [System.Xml.XmlNamespaceManager]::new($packageXml.NameTable)
    $namespace.AddNamespace("n", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd")
    $metadata = $packageXml.SelectSingleNode("/n:package/n:metadata", $namespace)
    if ($metadata.id -ne $packageId -or $metadata.version -ne $packageVersion) {
        throw "ID ou versao do nuspec interno nao corresponde ao release solicitado."
    }

    $iconName = [string]$metadata.icon
    if ([string]::IsNullOrWhiteSpace($iconName) -or $null -eq $archive.GetEntry($iconName)) {
        throw "O icone declarado nao esta incorporado ao pacote."
    }

    $deprecatedIconUrl = $packageXml.SelectSingleNode("/n:package/n:metadata/n:iconUrl", $namespace)
    if ($null -ne $deprecatedIconUrl) {
        throw "O pacote ainda contem o elemento obsoleto iconUrl."
    }

    $localDllPath = Join-Path $releaseDirectory "$packageId.dll"
    $localAssemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($localDllPath).Version.ToString()
    if ($localAssemblyVersion -ne $Version) {
        throw "AssemblyVersion '$localAssemblyVersion' difere da versao do pacote '$Version'."
    }

    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try {
        $packagedDllStream = $dllEntry.Open()
        try {
            $packagedDllHash = ([BitConverter]::ToString($sha256.ComputeHash($packagedDllStream))).Replace("-", "")
        }
        finally {
            $packagedDllStream.Dispose()
        }

        $localDllStream = [System.IO.File]::OpenRead($localDllPath)
        try {
            $localDllHash = ([BitConverter]::ToString($sha256.ComputeHash($localDllStream))).Replace("-", "")
        }
        finally {
            $localDllStream.Dispose()
        }
    }
    finally {
        $sha256.Dispose()
    }

    if ($packagedDllHash -ne $localDllHash) {
        throw "A DLL incorporada ao pacote difere da DLL compilada."
    }
}
finally {
    $archive.Dispose()
}

$packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash
Write-Host "Pacote validado: $packagePath" -ForegroundColor Green
Write-Host "SHA256: $packageHash"

$versionExists = $null
if (-not $SkipAvailabilityCheck) {
    $versionExists = Test-NuGetVersionExists -RequestedVersion $packageVersion
    if ($versionExists -eq $true) {
        Write-Warning "A versao NuGet $packageVersion ja existe e nao pode ser substituida."
    }
    elseif ($versionExists -eq $false) {
        Write-Host "A versao NuGet $packageVersion esta disponivel." -ForegroundColor Green
    }
}

if (-not $Publish) {
    Write-Host "Preparacao concluida. Use -Publish para enviar este pacote ao NuGet." -ForegroundColor Yellow
    exit 0
}

if ($versionExists -eq $true) {
    throw "Publicacao cancelada porque a versao $Version ja existe."
}

if ([string]::IsNullOrWhiteSpace($env:NUGET_API_KEY)) {
    throw "Defina a variavel de ambiente NUGET_API_KEY antes de publicar."
}

if (-not $Yes) {
    $confirmation = Read-Host "Digite exatamente '$Version' para confirmar a publicacao irreversivel no NuGet"
    if ($confirmation -ne $Version) {
        throw "Publicacao cancelada pelo usuario."
    }
}

dotnet nuget push $packagePath --api-key $env:NUGET_API_KEY --source $nugetSource
if ($LASTEXITCODE -ne 0) {
    throw "O envio ao NuGet falhou."
}

Write-Host "Versao $Version enviada ao NuGet com sucesso." -ForegroundColor Green
