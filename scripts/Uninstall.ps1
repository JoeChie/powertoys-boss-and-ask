[CmdletBinding(SupportsShouldProcess=$true)]
param([Parameter(Mandatory=$true)][ValidateSet('BossCodex','BossDeepSeek','BossTelegram','BossPhoneLink','AskCodex','AskWorkBuddy','AskDeepSeek')][string]$Plugin)
$ErrorActionPreference='Stop'
if(Get-Process -Name PowerToys.PowerLauncher -ErrorAction SilentlyContinue){throw 'Restore hidden windows, then exit PowerToys before uninstalling.'}
$plugins=[IO.Path]::GetFullPath((Join-Path $env:LOCALAPPDATA 'Microsoft\PowerToys\PowerToys Run\Plugins'))
$dest=[IO.Path]::GetFullPath((Join-Path $plugins $Plugin))
if([IO.Path]::GetDirectoryName($dest) -ne $plugins){throw 'Unexpected plugin path.'}
if(!(Test-Path -LiteralPath $dest)){Write-Output 'Plugin not installed.';return}
$manifest=Get-Content -Raw -LiteralPath (Join-Path $dest 'plugin.json') | ConvertFrom-Json
if($manifest.Author -ne 'JoeChie' -or $manifest.ExecuteFileName -ne ($Plugin+'.dll')){throw 'Folder does not match this project.'}
if($PSCmdlet.ShouldProcess($dest,'Back up and uninstall plugin')){
    $backup=Join-Path $env:LOCALAPPDATA ('PowerToysCommunity\Backups\'+$Plugin+'-uninstalled-'+(Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    Copy-Item -LiteralPath $dest -Destination $backup -Recurse
    Remove-Item -LiteralPath $dest -Recurse -Force
    Write-Output ('Uninstalled; backup retained at '+$backup)
}
