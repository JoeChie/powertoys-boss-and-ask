[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
$framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$csc=Join-Path $framework 'csc.exe'
$refs=@('UIAutomationClient','UIAutomationTypes','WindowsBase' | ForEach-Object {'/r:'+(Join-Path $framework ('WPF\'+$_+'.dll'))})
$modules=Get-Content -Raw -LiteralPath (Join-Path $repo 'modules.json') | ConvertFrom-Json
foreach($module in $modules){
    $dest=Join-Path $repo ('out\tests\'+$module.slug)
    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    $files=@((Join-Path $repo ('src\'+$module.slug+'\Main.cs')),(Join-Path $repo 'src\Shared\CommunityConfig.cs'),(Join-Path $repo 'tests\WoxStub.cs'),(Join-Path $repo 'tests\ContractTests.cs'))
    if($module.key){$files+=(Join-Path $repo 'src\Shared\Boss.cs');$files+=(Join-Path $repo 'src\Shared\HotkeyHost.cs')}
    $exe=Join-Path $dest 'ContractTests.exe'
    & $csc /nologo /codepage:65001 /target:exe /main:ContractTests ('/out:'+$exe) @refs @files
    if($LASTEXITCODE){throw ('Test build failed: '+$module.slug)}
    & $exe $module.slug $module.process $module.keyword
    if($LASTEXITCODE){throw ('Test failed: '+$module.slug)}
}
Get-ChildItem -LiteralPath (Join-Path $repo 'scripts') -Filter '*.ps1' | ForEach-Object {
    $tokens=$null;$errors=$null
    [Management.Automation.Language.Parser]::ParseFile($_.FullName,[ref]$tokens,[ref]$errors) | Out-Null
    if($errors.Count){throw ($_.Name+': '+($errors -join '; '))}
}
Write-Output 'PASS PowerShell syntax checks. These tests do not replace real-window smoke testing.'
