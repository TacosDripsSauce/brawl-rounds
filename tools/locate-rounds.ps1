$ErrorActionPreference = 'Stop'

function Test-RoundsFolder([string]$p) {
    return $p -and (Test-Path (Join-Path $p 'Rounds.exe')) -and (Test-Path (Join-Path $p 'Rounds_Data\Managed\Assembly-CSharp.dll'))
}

$candidates = New-Object System.Collections.Generic.List[string]
if ($env:ROUNDS_DIR) { $candidates.Add($env:ROUNDS_DIR) }

# Common Steam locations across typical Windows drives.
foreach ($drive in @('C','D','E','F','G','H')) {
    foreach ($relative in @(
        'SteamLibrary\steamapps\common\ROUNDS',
        'Steam\steamapps\common\ROUNDS',
        'Games\Steam\steamapps\common\ROUNDS',
        'Program Files (x86)\Steam\steamapps\common\ROUNDS',
        'Program Files\Steam\steamapps\common\ROUNDS'
    )) {
        $candidates.Add("${drive}:\$relative")
    }
}

# Read Steam libraryfolders.vdf from any Steam root we can find.
$steamRoots = New-Object System.Collections.Generic.List[string]
foreach ($drive in @('C','D','E','F','G','H')) {
    foreach ($relative in @('Steam','SteamLibrary','Program Files (x86)\Steam','Program Files\Steam')) {
        $root = "${drive}:\$relative"
        if (Test-Path $root) { $steamRoots.Add($root) }
    }
}

foreach ($steam in ($steamRoots | Select-Object -Unique)) {
    $vdf = Join-Path $steam 'steamapps\libraryfolders.vdf'
    if (Test-Path $vdf) {
        $text = Get-Content $vdf -Raw
        [regex]::Matches($text, '"path"\s+"([^"]+)"') | ForEach-Object {
            $lib = $_.Groups[1].Value -replace '\\\\','\'
            $candidates.Add((Join-Path $lib 'steamapps\common\ROUNDS'))
        }
    }
}

foreach ($p in ($candidates | Select-Object -Unique)) {
    if (Test-RoundsFolder $p) { Write-Output $p; exit 0 }
}

throw 'ROUNDS installation not found. Set ROUNDS_DIR to the game folder and run again.'
