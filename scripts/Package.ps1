[CmdletBinding()]
param([string]$Version='0.1.0',[string]$OutputPath,[string]$StagingPath)
$ErrorActionPreference='Stop'
$repo=Split-Path $PSScriptRoot -Parent
$modules=Get-Content -Raw -LiteralPath (Join-Path $repo 'modules.json') | ConvertFrom-Json
$release=if($OutputPath){$OutputPath}else{Join-Path $repo 'release'}
$staging=if($StagingPath){$StagingPath}else{Join-Path $repo 'staging'}
New-Item -ItemType Directory -Path $release -Force | Out-Null
$hashes=@()
foreach($module in $modules){
    $slug=$module.slug
    $out=Join-Path $repo ('out\'+$slug)
    if(!(Test-Path -LiteralPath (Join-Path $out ($slug+'.dll')))){throw ('Build first: '+$slug)}
    $stage=Join-Path $staging $slug
    if(Test-Path -LiteralPath $stage){throw ('Staging folder already exists. Use a fresh output tree: '+$stage)}
    New-Item -ItemType Directory -Path $stage -Force | Out-Null
    $dest=Join-Path $stage $slug
    New-Item -ItemType Directory -Path $dest | Out-Null
    foreach($name in @('plugin.json','settings.ini','icon.png',($slug+'.dll'))){Copy-Item -LiteralPath (Join-Path $out $name) -Destination $dest}
    $module | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath (Join-Path $dest 'module.json') -Encoding utf8
    foreach($name in @('Install.ps1','Uninstall.ps1')){Copy-Item -LiteralPath (Join-Path $PSScriptRoot $name) -Destination $stage}
    Copy-Item -LiteralPath (Join-Path $repo 'LICENSE') -Destination $stage
    Copy-Item -LiteralPath (Join-Path $repo ('docs\'+$slug+'.md')) -Destination (Join-Path $stage 'README.md')
    Copy-Item -LiteralPath (Join-Path $repo ('docs\'+$slug+'.zh-CN.md')) -Destination (Join-Path $stage 'README.zh-CN.md')
    $zip=Join-Path $release ($slug+'-'+$Version+'-win-x64.zip')
    if(Test-Path -LiteralPath $zip){throw ('Release already exists: '+$zip)}
    Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -CompressionLevel Optimal
    $hashes+=((Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()+'  '+[IO.Path]::GetFileName($zip))
}
$hashes | Set-Content -LiteralPath (Join-Path $release 'SHA256SUMS.txt') -Encoding ascii
Write-Output ('Seven release ZIPs and SHA256SUMS.txt written to '+$release)
