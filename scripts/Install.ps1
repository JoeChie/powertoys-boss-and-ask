[CmdletBinding()]
param([string]$SourcePath=$PSScriptRoot,[string]$Plugin,[string]$AppPath)
$ErrorActionPreference='Stop'
if(Get-Process -Name PowerToys.PowerLauncher -ErrorAction SilentlyContinue){throw 'Exit PowerToys from its tray menu before installing.'}
if(Test-Path -LiteralPath (Join-Path $SourcePath '..\out')){$SourcePath=Join-Path $SourcePath '..\out'}
$candidates=@(Get-ChildItem -LiteralPath $SourcePath -Directory | Where-Object {Test-Path -LiteralPath (Join-Path $_.FullName 'plugin.json')})
if($Plugin){$candidates=@($candidates | Where-Object Name -eq $Plugin)}
if($candidates.Count -ne 1){throw 'Select one plugin with -Plugin NAME, or run this script from an extracted single-plugin release.'}
$source=$candidates[0].FullName
$name=$candidates[0].Name
$manifest=Get-Content -Raw -LiteralPath (Join-Path $source 'plugin.json') | ConvertFrom-Json
$plugins=Join-Path $env:LOCALAPPDATA 'Microsoft\PowerToys\PowerToys Run\Plugins'
$dest=Join-Path $plugins $name
New-Item -ItemType Directory -Path $plugins -Force | Out-Null
$duplicate=@(Get-ChildItem -LiteralPath $plugins -Directory | Where-Object {
    $file=Join-Path $_.FullName 'plugin.json'
    (Test-Path -LiteralPath $file) -and $_.Name -ne $name -and ((Get-Content -Raw -LiteralPath $file | ConvertFrom-Json).ID -eq $manifest.ID)
})
if($duplicate.Count){throw 'The same plugin ID is already installed under a different folder. Resolve the duplicate first.'}
if(Test-Path -LiteralPath $dest){
    $old=Get-Content -Raw -LiteralPath (Join-Path $dest 'plugin.json') | ConvertFrom-Json
    if($old.ID -ne $manifest.ID){throw 'Destination belongs to a different plugin.'}
    $backup=Join-Path $env:LOCALAPPDATA ('PowerToysCommunity\Backups\'+$name+'-'+(Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    Copy-Item -LiteralPath $dest -Destination $backup -Recurse
    Write-Output ('Backup: '+$backup)
}
if($AppPath){$AppPath=(Resolve-Path -LiteralPath $AppPath).Path;if([IO.Path]::GetExtension($AppPath) -ne '.exe'){throw 'AppPath must be an executable.'}}
New-Item -ItemType Directory -Path $dest -Force | Out-Null
foreach($file in @('plugin.json','icon.png',$manifest.ExecuteFileName)){Copy-Item -LiteralPath (Join-Path $source $file) -Destination $dest -Force}
$settings=Join-Path $dest 'settings.ini'
if(!(Test-Path -LiteralPath $settings)){Copy-Item -LiteralPath (Join-Path $source 'settings.ini') -Destination $settings}
if(!$AppPath -and (Test-Path -LiteralPath (Join-Path $source 'module.json'))){
    $module=Get-Content -Raw -LiteralPath (Join-Path $source 'module.json') | ConvertFrom-Json
    if($module.process -notin @('ChatGPT','PhoneExperienceHost')){
        $shell=New-Object -ComObject WScript.Shell
        $menuRoots=@([Environment]::GetFolderPath('StartMenu'),[Environment]::GetFolderPath('CommonStartMenu'))
        $matches=@(foreach($menu in $menuRoots){Get-ChildItem -LiteralPath $menu -Recurse -Filter '*.lnk' -ErrorAction SilentlyContinue | ForEach-Object {
            $target=$shell.CreateShortcut($_.FullName).TargetPath
            if([IO.Path]::GetFileNameWithoutExtension($target) -eq $module.process -and (Test-Path -LiteralPath $target)){$target}
        }}) | Select-Object -Unique
        if(@($matches).Count -eq 1){$AppPath=@($matches)[0]}
    }
}
if($AppPath){$lines=Get-Content -LiteralPath $settings;$lines | ForEach-Object {if($_ -match '^AppPath='){'AppPath='+$AppPath}else{$_}} | Set-Content -LiteralPath $settings -Encoding utf8}
Write-Output ('Installed '+$name+'. Open settings.ini to check AppPath / AppId / Hotkey, then restart PowerToys.')
Write-Output 'Enable this plugin in PowerToys Run settings. No whole settings.json file was replaced.'
