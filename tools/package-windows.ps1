param([string]$DllPath)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
if (-not $DllPath) { $DllPath = Join-Path $root 'src\BrawlRounds\bin\Release\net461\BrawlRounds.dll' }
if (-not (Test-Path $DllPath)) { throw "Missing compiled DLL: $DllPath" }
$stage = Join-Path $root '_build\BrawlRounds-0.1.0'
$plugins = Join-Path $stage 'BepInEx\plugins\BrawlRounds'
Remove-Item $stage -Recurse -Force -ErrorAction SilentlyContinue
New-Item $plugins -ItemType Directory -Force | Out-Null
Copy-Item $DllPath (Join-Path $plugins 'BrawlRounds.dll')
Copy-Item (Join-Path $root 'README.md') (Join-Path $stage 'README.md')
$zip = Join-Path $root 'dist\BrawlRounds-0.1.0.zip'
Remove-Item $zip -Force -ErrorAction SilentlyContinue
Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip
$hash = (Get-FileHash $zip -Algorithm SHA256).Hash.ToLowerInvariant()
$size = (Get-Item $zip).Length
Write-Output "ZIP=$zip"
Write-Output "SIZE=$size"
Write-Output "SHA256=$hash"
