[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ManifestPath,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+\.\d+$')]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [AllowEmptyString()]
    [string]$ReleaseNotes,

    [switch]$Publish
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path $PSScriptRoot -Parent
$resolvedManifestPath = (Resolve-Path -LiteralPath $ManifestPath).Path
$toolDirectory = Split-Path $resolvedManifestPath -Parent
$manifest = Get-Content -LiteralPath $resolvedManifestPath -Raw | ConvertFrom-Json
$projectPath = [IO.Path]::GetFullPath((Join-Path $toolDirectory $manifest.projectPath))
$nuspecPath = [IO.Path]::GetFullPath((Join-Path $toolDirectory $manifest.nuspecPath))
$assemblyInfoPath = [IO.Path]::GetFullPath((Join-Path $toolDirectory $manifest.assemblyInfoPath))
$packageScriptPath = [IO.Path]::GetFullPath((Join-Path $toolDirectory $manifest.packageScriptPath))
$releaseDirectory = Join-Path $repositoryRoot "bin\Release"
$nugetSource = "https://api.nuget.org/v3/index.json"
$nugetVersionsUrl = "https://api.nuget.org/v3-flatcontainer/$($manifest.packageId.ToLowerInvariant())/index.json"

function Set-RegexValue {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Pattern,
        [Parameter(Mandatory = $true)][string]$Value,
        [Text.RegularExpressions.RegexOptions]$Options = [Text.RegularExpressions.RegexOptions]::None
    )

    $content = [IO.File]::ReadAllText($Path)
    $regex = [Text.RegularExpressions.Regex]::new($Pattern, $Options)
    if (-not $regex.IsMatch($content)) {
        throw "Não foi possível localizar o metadado esperado em '$Path'."
    }

    $updated = $regex.Replace(
        $content,
        [Text.RegularExpressions.MatchEvaluator]{
            param($match)
            return $match.Groups[1].Value + $Value + $match.Groups[2].Value
        },
        1)
    [IO.File]::WriteAllText($Path, $updated, [Text.UTF8Encoding]::new($false))
}

function Invoke-CheckedCommand {
    param(
        [Parameter(Mandatory = $true)][string]$Description,
        [Parameter(Mandatory = $true)][scriptblock]$Command
    )

    Write-Host $Description
    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "$Description falhou com o código $LASTEXITCODE."
    }
}

function Test-NuGetVersionExists {
    param([Parameter(Mandatory = $true)][string]$RequestedVersion)

    try {
        $response = Invoke-RestMethod -Uri $nugetVersionsUrl -Method Get
        return @($response.versions) -contains $RequestedVersion.ToLowerInvariant()
    }
    catch {
        Write-Warning "Não foi possível consultar as versões existentes no NuGet: $($_.Exception.Message)"
        return $null
    }
}

foreach ($requiredPath in @($projectPath, $nuspecPath, $assemblyInfoPath, $packageScriptPath)) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Arquivo obrigatório não encontrado: $requiredPath"
    }
}

if ([string]::IsNullOrWhiteSpace($ReleaseNotes)) {
    throw "As notas da versão são obrigatórias."
}

$versionParts = $Version.Split('.')
$packageVersion = if ($versionParts[3] -eq '0') {
    $versionParts[0..2] -join '.'
}
else {
    $Version
}

if ((Test-NuGetVersionExists -RequestedVersion $packageVersion) -eq $true) {
    throw "A versão $packageVersion já existe no NuGet e não pode ser substituída."
}

Write-Host "Preparando $($manifest.packageId) $Version"
Set-RegexValue -Path $assemblyInfoPath -Pattern '(AssemblyVersion\(")[^"]+("\)\])' -Value $Version
Set-RegexValue -Path $assemblyInfoPath -Pattern '(AssemblyFileVersion\(")[^"]+("\)\])' -Value $Version
Set-RegexValue -Path $nuspecPath -Pattern '(<version>)[^<]+(</version>)' -Value $packageVersion
$escapedReleaseNotes = [Security.SecurityElement]::Escape($ReleaseNotes.Trim())
Set-RegexValue `
    -Path $nuspecPath `
    -Pattern '(<releaseNotes>).*?(</releaseNotes>)' `
    -Value $escapedReleaseNotes `
    -Options ([Text.RegularExpressions.RegexOptions]::Singleline)

Push-Location $repositoryRoot
try {
    Invoke-CheckedCommand "Restaurando ferramentas locais" { dotnet tool restore }
    Invoke-CheckedCommand "Verificando a formatação C#" { dotnet csharpier check . }
    Invoke-CheckedCommand "Compilando Debug" { dotnet build $projectPath -c Debug }
    Invoke-CheckedCommand "Compilando Release e gerando o pacote" { & $packageScriptPath -Configuration Release }

    foreach ($validationScript in @($manifest.validationScripts)) {
        $resolvedValidationScript = [IO.Path]::GetFullPath((Join-Path $toolDirectory $validationScript))
        if (-not (Test-Path -LiteralPath $resolvedValidationScript)) {
            throw "Script de validação não encontrado: $resolvedValidationScript"
        }

        Invoke-CheckedCommand "Executando $([IO.Path]::GetFileName($resolvedValidationScript))" {
            & $resolvedValidationScript -Configuration Release
        }
    }
}
finally {
    Pop-Location
}

$packagePath = Join-Path $releaseDirectory "$($manifest.packageId).$packageVersion.nupkg"
if (-not (Test-Path -LiteralPath $packagePath)) {
    throw "O pacote esperado não foi criado: $packagePath"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    $pluginEntries = @(
        $archive.Entries |
            Where-Object { $_.FullName -match '^lib/net48/Plugins/[^/]+\.dll$' }
    )
    $expectedAssembly = "lib/net48/Plugins/$($manifest.packageId).dll"
    if ($pluginEntries.Count -ne 1 -or $null -eq $archive.GetEntry($expectedAssembly)) {
        throw "O pacote deve conter somente a DLL esperada em '$expectedAssembly'."
    }

    if ($null -ne $archive.GetEntry("lib/net48/Plugins/LucasVerissimo.XrmToolBox.Shared.dll")) {
        throw "O pacote não pode conter uma DLL Shared separada."
    }
}
finally {
    $archive.Dispose()
}

$packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash
Write-Host "Pacote validado: $packagePath"
Write-Host "SHA256: $packageHash"

if (-not $Publish) {
    Write-Host "Preparação concluída. O pacote ainda não foi enviado."
    exit 0
}

if ([string]::IsNullOrWhiteSpace($env:NUGET_API_KEY)) {
    throw "A API key do NuGet não foi informada."
}

dotnet nuget push $packagePath --api-key $env:NUGET_API_KEY --source $nugetSource
if ($LASTEXITCODE -ne 0) {
    throw "O envio ao NuGet falhou. Verifique a API key, seu vencimento e o escopo para este pacote."
}

Write-Host "Versão $packageVersion enviada ao NuGet com sucesso."
