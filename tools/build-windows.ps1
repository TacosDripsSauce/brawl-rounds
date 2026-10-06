param(
    [string]$RoundsFolder,
    [string]$UnboundLibPath,
    [ValidateSet('Debug','Release')][string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

if (-not $RoundsFolder) {
    $RoundsFolder = & (Join-Path $PSScriptRoot 'locate-rounds.ps1')
}
if (-not (Test-Path (Join-Path $RoundsFolder 'Rounds_Data\Managed\Assembly-CSharp.dll'))) {
    throw "Invalid ROUNDS folder: $RoundsFolder"
}

if (-not $UnboundLibPath) {
    $bep = Join-Path $RoundsFolder 'BepInEx'
    if (Test-Path $bep) {
        $found = Get-ChildItem $bep -Filter 'UnboundLib.dll' -File -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($found) { $UnboundLibPath = $found.FullName }
    }
}
if (-not $UnboundLibPath -or -not (Test-Path $UnboundLibPath)) {
    throw 'UnboundLib.dll was not found. Install the Melty/ROUNDS loader profile first or pass -UnboundLibPath explicitly.'
}

python (Join-Path $PSScriptRoot 'preflight.py')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
python (Join-Path $PSScriptRoot 'generate.py')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
python (Join-Path $PSScriptRoot 'static_check.py')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$proj = Join-Path $root 'src\BrawlRounds\BrawlRounds.csproj'
dotnet build $proj -c $Configuration -p:RoundsFolder="$RoundsFolder" -p:UnboundLibPath="$UnboundLibPath"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$out = Join-Path $root "src\BrawlRounds\bin\$Configuration\net461\BrawlRounds.dll"
if (-not (Test-Path $out)) { throw 'Build completed but BrawlRounds.dll was not found.' }
Write-Output $out
