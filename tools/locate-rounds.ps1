$ErrorActionPreference = 'Stop'

function Test-RoundsFolder([string]$p) {
    return $p -and (Test-Path (Join-Path $p 'Rounds.exe')) -and (Test-Path (Join-Path $p 'Rounds_Data\Managed\Assembly-CSharp.dll'))
}

$candidates = New-Object System.Collections.Generic.List[string]
if ($env:ROUNDS_DIR) { $candidates.Add($env:ROUNDS_DIR) }
$candidates.Add('C:\Program Files (x86)\Steam\steamapps\common\ROUNDS')
$candidates.Add('C:\Program Files\Steam\steamapps\common\ROUNDS')

$steamRoots = @('C:\Program Files (x86)\Steam','C:\Program Files\Steam') | Where-Object { Test-Path $_ }
foreach ($steam in $steamRoots) {
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
