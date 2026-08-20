param([string]$Configuration = "Release")

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path $PSScriptRoot -Parent
$toolDirectory = Join-Path $repositoryRoot "LucasVerissimo.XrmToolBox.FieldChangeMonitor"
$toolProject = Join-Path $toolDirectory "LucasVerissimo.XrmToolBox.FieldChangeMonitor.csproj"
$toolOutput = Join-Path $repositoryRoot "bin\$Configuration"
$packageId = "LucasVerissimo.XrmToolBox.FieldChangeMonitor"
$failures = [System.Collections.Generic.List[string]]::new()
$passed = 0

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) {
        $script:failures.Add($Message)
        Write-Host "FALHOU: $Message" -ForegroundColor Red
        return
    }

    $script:passed++
    Write-Host "OK: $Message" -ForegroundColor Green
}

function Invoke-PrivateStatic {
    param([Type]$Type, [string]$Name, [object[]]$Arguments)
    $method = $Type.GetMethod($Name, [Reflection.BindingFlags]"NonPublic,Static")
    if ($null -eq $method) {
        throw "Metodo privado nao encontrado: $Name"
    }
    return $method.Invoke($null, $Arguments)
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
    dotnet build $toolProject -c $Configuration | Out-Host
    Assert-True ($LASTEXITCODE -eq 0) "Solution de producao compila"

    $toolAssemblyPath = Join-Path $toolOutput "$packageId.dll"
    Assert-True (Test-Path $toolAssemblyPath) "DLL principal gerada"

    $toolAssembly = [Reflection.Assembly]::LoadFrom($toolAssemblyPath)
    Assert-True ($null -ne $toolAssembly.GetType("LucasVerissimo.XrmToolBox.Shared.WinForms.LookupValuePickerForm")) "Picker de lookup compilado no plugin"
    Assert-True ($null -ne $toolAssembly.GetType("LucasVerissimo.XrmToolBox.Shared.WinForms.OptionSetValuePickerForm")) "Picker de opcoes compilado no plugin"

    $pluginType = $toolAssembly.GetType("LucasVerissimo.XrmToolBox.FieldChangeMonitor.MyPlugin", $false)
    $controlType = $toolAssembly.GetType("LucasVerissimo.XrmToolBox.FieldChangeMonitor.MyPluginControl", $false)
    $settingsType = $toolAssembly.GetType("LucasVerissimo.XrmToolBox.FieldChangeMonitor.Settings", $false)
    Assert-True ($null -ne $pluginType) "Plugin usa o namespace definitivo"
    Assert-True ($null -ne $controlType) "Controle usa o namespace definitivo"
    Assert-True ($null -ne $settingsType) "Configuracoes usam o namespace definitivo"
    $legacyPluginType = $toolAssembly.GetType("XrmTool_bravo.MyPlugin", $false)
    Assert-True ($null -ne $legacyPluginType -and $pluginType.IsAssignableFrom($legacyPluginType)) "Manifestos antigos resolvem a ponte de compatibilidade"
    $legacyExports = $legacyPluginType.GetCustomAttributes($false) | Where-Object { $_.GetType().FullName -eq "System.ComponentModel.Composition.ExportAttribute" }
    Assert-True (@($legacyExports).Count -eq 0) "Ponte antiga nao cria uma segunda exportacao MEF"

    $plugin = [Activator]::CreateInstance($pluginType)
    $pluginControl = $plugin.GetControl()
    try {
        Assert-True ($pluginControl.GetType() -eq $controlType -and $pluginControl.Controls.Count -gt 0) "Plugin cria e inicializa o controle WinForms"
    }
    finally {
        $pluginControl.Dispose()
    }

    $optionSet = [Microsoft.Xrm.Sdk.Metadata.OptionSetMetadata]::new()
    $optionSet.Options.Add([Microsoft.Xrm.Sdk.Metadata.OptionMetadata]::new([Microsoft.Xrm.Sdk.Label]::new("Ativo", 1046), 1))
    $optionSet.Options.Add([Microsoft.Xrm.Sdk.Metadata.OptionMetadata]::new([Microsoft.Xrm.Sdk.Label]::new("Inativo", 1046), 2))
    $optionAttribute = [Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata]::new()
    $optionAttribute.OptionSet = $optionSet
    $optionPickerType = $toolAssembly.GetType("LucasVerissimo.XrmToolBox.Shared.WinForms.OptionSetValuePickerForm", $true)
    $optionPicker = [Activator]::CreateInstance($optionPickerType, @($optionAttribute, $true))
    try {
        $optionsList = $optionPickerType.GetField("lvOptions", [Reflection.BindingFlags]"NonPublic,Instance").GetValue($optionPicker)
        Assert-True ($optionsList.Items.Count -eq 2 -and $optionsList.Items[0].Text -eq "Ativo") "Picker de opcoes popula metadados do Dataverse"
    }
    finally {
        $optionPicker.Dispose()
    }

    $lookupAttribute = [Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata]::new()
    $lookupAttribute.Targets = @("account", "contact")
    $lookupPickerType = $toolAssembly.GetType("LucasVerissimo.XrmToolBox.Shared.WinForms.LookupValuePickerForm", $true)
    $lookupPicker = [Activator]::CreateInstance($lookupPickerType, @($null, $lookupAttribute))
    try {
        $targetCombo = $lookupPickerType.GetField("cboTarget", [Reflection.BindingFlags]"NonPublic,Instance").GetValue($lookupPicker)
        Assert-True ($targetCombo.Items.Count -eq 2 -and $targetCombo.SelectedItem -eq "account") "Picker de lookup popula tabelas-alvo"
    }
    finally {
        $lookupPicker.Dispose()
    }

    $columns = [System.Collections.Generic.List[string]]::new()
    $columns.Add("name")
    $columns.Add("revenue")
    $fetch = Invoke-PrivateStatic $controlType "BuildFetchXml" @("account", "accountid", "name", $columns, '<filter type="and"><condition attribute="statecode" operator="eq" value="0" /></filter>')
    [xml]$fetchXml = $fetch
    Assert-True ($fetchXml.fetch.entity.name -eq "account") "Geracao de FetchXML mantem a tabela"
    Assert-True (@($fetchXml.fetch.entity.attribute | ForEach-Object name) -contains "revenue") "Geracao de FetchXML inclui colunas monitoradas"
    Assert-True ($fetchXml.fetch.entity.filter.condition.attribute -eq "statecode") "Geracao de FetchXML inclui o filtro"

    $paged = Invoke-PrivateStatic $controlType "ApplyPaging" @($fetch, 3, "cookie<&>")
    [xml]$pagedXml = $paged
    Assert-True ($pagedXml.fetch.page -eq "3") "Paginacao aplica o numero da pagina"
    Assert-True ($pagedXml.fetch.'paging-cookie' -eq "cookie<&>") "Paginacao preserva o cookie"

    $normalizeArguments = [object[]]@('<condition attribute="name" operator="like" value="A%" />', "", $null)
    $normalized = Invoke-PrivateStatic $controlType "TryNormalizeFilterXml" $normalizeArguments
    Assert-True ($normalized -and $normalizeArguments[1].StartsWith('<filter type="and">')) "Filtro condition e normalizado"
    $invalidArguments = [object[]]@('<condition', "", $null)
    $invalid = Invoke-PrivateStatic $controlType "TryNormalizeFilterXml" $invalidArguments
    Assert-True ((-not $invalid) -and -not [string]::IsNullOrWhiteSpace([string]$invalidArguments[2])) "Filtro XML invalido e rejeitado"

    $settings = [Activator]::CreateInstance($settingsType)
    $settings.MaximumRecentChanges = 42
    $serializer = [System.Xml.Serialization.XmlSerializer]::new($settingsType)
    $writer = [IO.StringWriter]::new()
    $serializer.Serialize($writer, $settings)
    $reader = [IO.StringReader]::new($writer.ToString())
    $roundTripSettings = $serializer.Deserialize($reader)
    Assert-True ($roundTripSettings.MaximumRecentChanges -eq 42) "Configuracoes fazem round-trip XML"

    $existingSettingsPath = Join-Path $env:APPDATA "MscrmTools\XrmToolBox\Settings\$packageId.xml"
    if (Test-Path -LiteralPath $existingSettingsPath) {
        $existingSettingsReader = [IO.StreamReader]::new($existingSettingsPath)
        try {
            $existingSettings = $serializer.Deserialize($existingSettingsReader)
            Assert-True ($null -ne $existingSettings) "Namespace novo le configuracoes geradas pela versao anterior"
        }
        finally {
            $existingSettingsReader.Dispose()
        }
    }

    & (Join-Path $repositoryRoot "build-package.ps1") -Configuration $Configuration
    Assert-True ($LASTEXITCODE -eq 0) "Script de empacotamento conclui"
    $toolReferences = Get-AssemblyReferenceNames $toolAssemblyPath
    Assert-True (-not ($toolReferences -contains "LucasVerissimo.XrmToolBox.Shared")) "DLL do plugin nao referencia Shared externamente"
    $package = Get-ChildItem (Join-Path $toolOutput "$packageId.*.nupkg") | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    Assert-True ($null -ne $package) "Pacote NuGet foi criado"

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($package.FullName)
    try {
        $packagedPluginAssemblies = @($archive.Entries | Where-Object { $_.FullName -match "^lib/net48/Plugins/[^/]+\.dll$" })
        Assert-True ($null -ne $archive.GetEntry("lib/net48/Plugins/$packageId.dll")) "Pacote contem a DLL principal"
        Assert-True ($packagedPluginAssemblies.Count -eq 1) "Pacote contem somente a DLL consolidada do plugin"
        Assert-True ($null -eq $archive.GetEntry("lib/net48/Plugins/LucasVerissimo.XrmToolBox.Shared.dll")) "Pacote nao contem a DLL Shared separada"
        Assert-True ($null -ne $archive.GetEntry("field-change-monitor-128.png")) "Pacote contem o icone"
        Assert-True ($null -ne $archive.GetEntry("README.md")) "Pacote contem o README"
    }
    finally {
        $archive.Dispose()
    }
}
finally {
    Pop-Location
}

if ($failures.Count -gt 0) {
    throw "$($failures.Count) teste(s) falharam: $($failures -join '; ')"
}

Write-Host "$passed testes concluidos com sucesso." -ForegroundColor Cyan
