[CmdletBinding()]
param([string]$PowerToysPath=(Join-Path $env:LOCALAPPDATA 'PowerToys'),[string]$OutputPath)
$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
if(!$OutputPath){$OutputPath=Join-Path $repo 'out'}
if(!(Test-Path -LiteralPath (Join-Path $PowerToysPath 'Wox.Plugin.dll'))){throw 'Set -PowerToysPath to the folder containing Wox.Plugin.dll.'}
$csc=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if(!(Test-Path -LiteralPath $csc)){throw 'The x64 .NET Framework compiler is required.'}
$names=@('System.Private.CoreLib','System.Runtime','System.Collections','System.Diagnostics.Process','System.ComponentModel.Primitives','System.Private.Uri','System.Threading','System.Threading.Thread','System.Threading.ThreadPool','System.IO.FileSystem','System.Runtime.InteropServices','System.Text.Encoding.Extensions','System.Globalization','UIAutomationClient','UIAutomationTypes','WindowsBase','Wox.Plugin')
$refs=@($names | ForEach-Object {'/r:'+(Join-Path $PowerToysPath ($_+'.dll'))})
$modules=Get-Content -Raw -LiteralPath (Join-Path $repo 'modules.json') | ConvertFrom-Json
foreach($module in $modules){
    $source=Join-Path $repo ('src\'+$module.slug)
    $dest=Join-Path $OutputPath $module.slug
    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    $files=@((Join-Path $source 'Main.cs'),(Join-Path $repo 'src\Shared\CommunityConfig.cs'))
    if($module.key){$files+=(Join-Path $repo 'src\Shared\Boss.cs');$files+=(Join-Path $repo 'src\Shared\HotkeyHost.cs')}
    & $csc /nologo /noconfig /nostdlib /codepage:65001 /target:library /optimize+ /platform:x64 ('/out:'+(Join-Path $dest ($module.slug+'.dll'))) @refs @files
    if($LASTEXITCODE){throw ('Build failed: '+$module.slug)}
    foreach($name in @('plugin.json','settings.ini')){Copy-Item -LiteralPath (Join-Path $source $name) -Destination $dest -Force}
    Copy-Item -LiteralPath (Join-Path $repo ('assets\'+$module.slug+'.png')) -Destination (Join-Path $dest 'icon.png') -Force
    Write-Output ('Built '+$module.slug)
}
