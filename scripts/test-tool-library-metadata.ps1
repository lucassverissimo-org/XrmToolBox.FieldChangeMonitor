[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$NuspecPath,
    [string]$PackagePath,
    [switch]$SkipNetwork
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Read-ToolLibraryMetadata {
    param([xml]$Specification)

    $metadata = $Specification.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']")
    if ($null -eq $metadata) {
        throw 'Package metadata is missing.'
    }
    if ($null -ne $metadata.SelectSingleNode("*[local-name()='icon']")) {
        throw 'Tool Library compatibility: do not declare embedded icon metadata. Use only a public PNG iconUrl (see AGENTS.md).'
    }
    $values = @{}
    foreach ($name in @('id', 'version', 'iconUrl', 'projectUrl')) {
        $node = $metadata.SelectSingleNode("*[local-name()='$name']")
        if ($null -eq $node -or [string]::IsNullOrWhiteSpace($node.InnerText)) {
            throw "Required Tool Library metadata is missing: $name."
        }
        $values[$name] = $node.InnerText
    }
    foreach ($name in @('iconUrl', 'projectUrl')) {
        $uri = $null
        if (-not [Uri]::TryCreate($values[$name], [UriKind]::Absolute, [ref]$uri) -or $uri.Scheme -ne 'https' -or $uri.UserInfo.Length -gt 0) {
            throw "$name must be an anonymous absolute HTTPS URL."
        }
        if ($name -eq 'iconUrl' -and $uri.AbsolutePath -notmatch '\.png$') {
            throw 'iconUrl must point directly to a public PNG, not a page or NuGet icon endpoint.'
        }
    }
    return $values
}

[xml]$source = [IO.File]::ReadAllText((Resolve-Path -LiteralPath $NuspecPath).Path)
$expected = Read-ToolLibraryMetadata $source

if (-not [string]::IsNullOrWhiteSpace($PackagePath)) {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead((Resolve-Path -LiteralPath $PackagePath).Path)
    try {
        $specifications = @($archive.Entries | Where-Object { $_.FullName -match '^[^/]+\.nuspec$' })
        if ($specifications.Count -ne 1) {
            throw 'The package must contain exactly one root nuspec.'
        }
        $reader = [IO.StreamReader]::new($specifications[0].Open())
        try {
            [xml]$packagedSpecification = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }
        $actual = Read-ToolLibraryMetadata $packagedSpecification
        foreach ($name in @('id', 'version', 'iconUrl', 'projectUrl')) {
            if ($actual[$name] -ne $expected[$name]) {
                throw "Packaged $name differs from the source nuspec."
            }
        }
    }
    finally {
        $archive.Dispose()
    }
}

if (-not $SkipNetwork) {
    # Use GET, not just HEAD: HTTP success alone does not prove this is an image.
    $client = [Net.WebClient]::new()
    try {
        $bytes = $client.DownloadData($expected.iconUrl)
        $contentType = $client.ResponseHeaders['Content-Type']
        if (($contentType -split ';')[0].Trim() -ne 'image/png') {
            throw "The public logo must return image/png; received '$contentType'."
        }
        $pngSignature = @(137, 80, 78, 71, 13, 10, 26, 10)
        if ($bytes.Length -lt 8) {
            throw 'The public logo response is not a PNG.'
        }
        for ($index = 0; $index -lt $pngSignature.Length; $index++) {
            if ($bytes[$index] -ne $pngSignature[$index]) {
                throw 'The public logo response is not a PNG.'
            }
        }
        Add-Type -AssemblyName System.Drawing
        $stream = [IO.MemoryStream]::new($bytes, $false)
        try {
            $image = [Drawing.Image]::FromStream($stream)
            try {
                if ($image.Width -ne 128 -or $image.Height -ne 128) {
                    throw 'The public logo must be 128 x 128 pixels.'
                }
            }
            finally {
                $image.Dispose()
            }
        }
        finally {
            $stream.Dispose()
        }
        $documentation = Invoke-WebRequest -Uri $expected.projectUrl -UseBasicParsing
        if ($documentation.StatusCode -ne 200) {
            throw 'The public project documentation is not accessible.'
        }
    }
    finally {
        $client.Dispose()
    }
}

Write-Host "Tool Library metadata passed: $($expected.id) $($expected.version)." -ForegroundColor Green
