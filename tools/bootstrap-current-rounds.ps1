$ErrorActionPreference = 'Stop'

function Find-Rounds {
    $candidates = @(
        'D:\Program Files (x86)\Steam\steamapps\common\ROUNDS',
        'C:\Program Files (x86)\Steam\steamapps\common\ROUNDS',
        'D:\SteamLibrary\steamapps\common\ROUNDS',
        'C:\SteamLibrary\steamapps\common\ROUNDS'
    )
    foreach ($drive in @('C','D','E','F','G','H')) {
        $candidates += "${drive}:\Steam\steamapps\common\ROUNDS"
        $candidates += "${drive}:\Games\Steam\steamapps\common\ROUNDS"
    }
    foreach ($p in ($candidates | Select-Object -Unique)) {
        if ((Test-Path (Join-Path $p 'Rounds.exe')) -and (Test-Path (Join-Path $p 'Rounds_Data\Managed\Assembly-CSharp.dll'))) { return $p }
    }
    throw 'ROUNDS installation not found.'
}

function Install-ThunderstorePackage([string]$url, [string]$name, [string]$rounds, [bool]$bepPack = $false) {
    $work = Join-Path $env:TEMP "brawl-rounds-$name"
    $zip = "$work.zip"
    Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item $zip -Force -ErrorAction SilentlyContinue
    Write-Host "Downloading $name..." -ForegroundColor Cyan
    Invoke-WebRequest -UseBasicParsing -Uri $url -OutFile $zip
    Expand-Archive -Path $zip -DestinationPath $work -Force

    if ($bepPack) {
        $pack = Get-ChildItem $work -Directory -Recurse | Where-Object { $_.Name -eq 'BepInExPack_ROUNDS' } | Select-Object -First 1
        if (-not $pack) { throw "Could not find BepInExPack_ROUNDS inside $name." }
        Copy-Item (Join-Path $pack.FullName '*') $rounds -Recurse -Force
        return
    }

    $bepDirs = @(Get-ChildItem $work -Directory -Recurse | Where-Object { $_.Name -eq 'BepInEx' })
    if ($bepDirs.Count -eq 0) {
        throw "$name did not contain a BepInEx folder; package layout was unexpected."
    }
    foreach ($dir in $bepDirs) {
        New-Item (Join-Path $rounds 'BepInEx') -ItemType Directory -Force | Out-Null
        Copy-Item (Join-Path $dir.FullName '*') (Join-Path $rounds 'BepInEx') -Recurse -Force
    }
}

$project = Join-Path $env:USERPROFILE 'Desktop\brawl-rounds-source\brawl-rounds'
if (-not (Test-Path (Join-Path $project 'tools\build-windows.ps1'))) {
    throw "Project folder not found at $project"
}

$rounds = Find-Rounds
Write-Host "ROUNDS: $rounds" -ForegroundColor Green

# Pull the latest source fixes if Git is available.
if (Get-Command git -ErrorAction SilentlyContinue) {
    try { git -C $project pull --ff-only | Out-Host } catch { Write-Warning 'git pull failed; continuing with the local copy.' }
}

Install-ThunderstorePackage 'https://thunderstore.io/package/download/BepInEx/BepInExPack_ROUNDS/5.4.1901/' 'BepInEx' $rounds $true
Install-ThunderstorePackage 'https://thunderstore.io/package/download/willis81808/MMHook/1.0.0/' 'MMHook' $rounds
Install-ThunderstorePackage 'https://thunderstore.io/package/download/willis81808/UnboundLib/3.2.14/' 'UnboundLib' $rounds
Install-ThunderstorePackage 'https://thunderstore.io/package/download/kieron_exe/DuctTape/1.1.1/' 'DuctTape' $rounds

$unbound = Get-ChildItem (Join-Path $rounds 'BepInEx') -Filter 'UnboundLib.dll' -File -Recurse | Select-Object -First 1
if (-not $unbound) { throw 'UnboundLib.dll still not found after install.' }
if (-not (Test-Path (Join-Path $rounds 'BepInEx\core\BepInEx.dll'))) { throw 'BepInEx.dll still not found after install.' }

Write-Host "UnboundLib: $($unbound.FullName)" -ForegroundColor Green
Write-Host 'Building Brawl ROUNDS...' -ForegroundColor Cyan
& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $project 'tools\build-windows.ps1') -RoundsFolder $rounds -UnboundLibPath $unbound.FullName
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$dll = Join-Path $project 'src\BrawlRounds\bin\Release\net461\BrawlRounds.dll'
if (-not (Test-Path $dll)) { throw 'Build reported success, but BrawlRounds.dll was not found.' }
$pluginDir = Join-Path $rounds 'BepInEx\plugins\BrawlRounds'
New-Item $pluginDir -ItemType Directory -Force | Out-Null
Copy-Item $dll (Join-Path $pluginDir 'BrawlRounds.dll') -Force
Write-Host ''
Write-Host 'SUCCESS: BrawlRounds.dll built and installed into ROUNDS.' -ForegroundColor Green
Write-Host "DLL: $dll"
Write-Host 'Next step: launch ROUNDS once so DuctTape can patch the current game/mod stack, then send a screenshot or BepInEx log.'
