$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = Split-Path $PSScriptRoot -Parent
$validator = Join-Path $root 'scripts/test-tool-library-metadata.ps1'
$fixtureDirectory = Join-Path $root ('obj/metadata-tests-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixtureDirectory | Out-Null
$fixture = Join-Path $fixtureDirectory 'tool.nuspec'
$baseline = '<package><metadata><id>Test.Tool</id><version>1.0.0</version><iconUrl>https://example.org/tool.png</iconUrl><projectUrl>https://example.org/docs</projectUrl></metadata></package>'
$passed = 0

function Assert-Rejected {
    param([string]$Xml, [string]$Message)
    [IO.File]::WriteAllText($fixture, $Xml)
    $rejected = $false
    try {
        & $validator -NuspecPath $fixture -SkipNetwork
    }
    catch {
        $rejected = $true
    }
    if (-not $rejected) {
        throw $Message
    }
    $script:passed++
}

[IO.File]::WriteAllText($fixture, $baseline)
& $validator -NuspecPath $fixture -SkipNetwork
$passed++
Assert-Rejected ($baseline.Replace('</metadata>', '<icon>tool.png</icon></metadata>')) 'Embedded icon plus iconUrl must be rejected.'
Assert-Rejected ($baseline.Replace('<iconUrl>https://example.org/tool.png</iconUrl>', '<icon>tool.png</icon>')) 'Embedded icon alone must be rejected.'
Assert-Rejected ($baseline.Replace('https://example.org/tool.png', 'https://api.nuget.org/v3-flatcontainer/test.tool/1.0.0/icon')) 'NuGet icon endpoints must be rejected.'
Assert-Rejected ($baseline.Replace('https://example.org/tool.png', 'http://example.org/tool.png')) 'Non-HTTPS icons must be rejected.'
Assert-Rejected ($baseline.Replace('<projectUrl>https://example.org/docs</projectUrl>', '')) 'Missing documentation must be rejected.'

# The validator must inspect the shipped nuspec, not only the source file.
Add-Type -AssemblyName System.IO.Compression.FileSystem
$packagePath = Join-Path $fixtureDirectory 'tool.nupkg'
$archive = [IO.Compression.ZipFile]::Open($packagePath, [IO.Compression.ZipArchiveMode]::Create)
try {
    $entry = $archive.CreateEntry('Test.Tool.nuspec')
    $writer = [IO.StreamWriter]::new($entry.Open())
    try {
        $writer.Write($baseline.Replace('</metadata>', '<icon>tool.png</icon></metadata>'))
    }
    finally {
        $writer.Dispose()
    }
}
finally {
    $archive.Dispose()
}
[IO.File]::WriteAllText($fixture, $baseline)
$rejected = $false
try {
    & $validator -NuspecPath $fixture -PackagePath $packagePath -SkipNetwork
}
catch {
    $rejected = $true
}
if (-not $rejected) {
    throw 'A package that reintroduces icon metadata must be rejected even if its source is valid.'
}
$passed++
Write-Host "Passed $passed Tool Library metadata regression checks."
