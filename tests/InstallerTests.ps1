$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
$caseRoot=Join-Path $repo ('out\installer-test-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $caseRoot -Force | Out-Null
$priorLocal=$env:LOCALAPPDATA
try {
    $env:LOCALAPPDATA=$caseRoot
    # Test process only: model a stopped PowerToys; never stop any real process.
    function Get-Process { [CmdletBinding()]param([string]$Name) }
    $install=Join-Path $repo 'scripts\Install.ps1'
    $uninstall=Join-Path $repo 'scripts\Uninstall.ps1'
    & $install -SourcePath (Join-Path $repo 'out') -Plugin BossCodex
    $dest=Join-Path $caseRoot 'Microsoft\PowerToys\PowerToys Run\Plugins\BossCodex'
    $settings=Join-Path $dest 'settings.ini'
    if(!(Test-Path -LiteralPath (Join-Path $dest 'BossCodex.dll'))){throw 'Install missed DLL'}
    (Get-Content -Raw -LiteralPath $settings).Replace('Hotkey=E','Hotkey=Z') | Set-Content -LiteralPath $settings -Encoding utf8
    & $install -SourcePath (Join-Path $repo 'out') -Plugin BossCodex
    if((Get-Content -Raw -LiteralPath $settings) -notmatch 'Hotkey=Z'){throw 'Reinstall overwrote user configuration'}
    & $uninstall -Plugin BossCodex -WhatIf
    if(!(Test-Path -LiteralPath $dest)){throw 'WhatIf removed plugin'}
    & $uninstall -Plugin BossCodex -Confirm:$false
    if(Test-Path -LiteralPath $dest){throw 'Uninstall did not remove selected plugin'}
    $backups=@(Get-ChildItem -LiteralPath (Join-Path $caseRoot 'PowerToysCommunity\Backups') -Directory)
    if($backups.Count -lt 2){throw 'Expected install and uninstall backups'}
    Write-Output 'PASS isolated installer: initial install, preserved config, backup, WhatIf, selected-module uninstall.'
} finally {$env:LOCALAPPDATA=$priorLocal}
