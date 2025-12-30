param(
    [string]$ResultsPath = "Tests/bin/Debug/net8.0/Reports/AllureResults",
    [string]$ReportPath = "Reports/AllureReport",
    [switch]$Open
)

# Resolve absolute paths relative to repo root (parent of scripts folder)
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$results = Resolve-Path -LiteralPath (Join-Path $repoRoot $ResultsPath) -ErrorAction SilentlyContinue
if (-not $results) { $results = Join-Path $repoRoot $ResultsPath }
$report = Resolve-Path -LiteralPath (Join-Path $repoRoot $ReportPath) -ErrorAction SilentlyContinue
if (-not $report) { $report = Join-Path $repoRoot $ReportPath }

New-Item -ItemType Directory -Force -Path $results | Out-Null
New-Item -ItemType Directory -Force -Path $report | Out-Null

# Preserve history for trends
$historySource = Join-Path $report "history"
$historyDest = Join-Path $results "history"
if ((Test-Path $historySource) -and -not (Test-Path $historyDest)) {
    Write-Host "Preserving history for trends..." -ForegroundColor Cyan
    Copy-Item -Path $historySource -Destination $historyDest -Recurse -Force
}

$allureCli = Get-Command allure -ErrorAction SilentlyContinue
if ($allureCli) {
    Write-Host "Using Allure CLI at $($allureCli.Source)" -ForegroundColor Green
    allure generate "$results" --clean -o "$report"
    if ($Open) {
        allure open "$report"
    }
    return
}

function Ensure-LocalAllure {
    param(
        [string]$BaseDir,
        [string]$Version = "2.25.0"
    )
    $target = Join-Path $BaseDir "allure-$Version"
    $exe = Join-Path $target "bin\allure.bat"
    if (Test-Path $exe) { return $exe }

    Write-Host "Downloading Allure CLI $Version..." -ForegroundColor Yellow
    $zipUrl = "https://repo.maven.apache.org/maven2/io/qameta/allure/allure-commandline/$Version/allure-commandline-$Version.zip"
    $tmp = Join-Path $env:TEMP "allure-$Version.zip"
    Invoke-WebRequest -Uri $zipUrl -OutFile $tmp -UseBasicParsing
    if (Test-Path $target) { Remove-Item -Recurse -Force $target }
    Expand-Archive -Path $tmp -DestinationPath $BaseDir -Force
    Remove-Item $tmp -Force
    if (-not (Test-Path $exe)) { throw "Failed to prepare Allure CLI" }
    return $exe
}

$toolsDir = Join-Path $repoRoot ".tools"
New-Item -ItemType Directory -Force -Path $toolsDir | Out-Null

try {
    $localAllure = Ensure-LocalAllure -BaseDir $toolsDir
    Write-Host "Using downloaded Allure CLI at $localAllure" -ForegroundColor Green
    
    # Preserve history for trends before clean
    $historySource = Join-Path $report "history"
    $historyDest = Join-Path $results "history"
    if ((Test-Path $historySource) -and -not (Test-Path $historyDest)) {
        Write-Host "Preserving history for trends..." -ForegroundColor Cyan
        Copy-Item -Path $historySource -Destination $historyDest -Recurse -Force
    }
    
    & $localAllure generate "$results" --clean -o "$report"
    if ($Open) {
        & $localAllure open "$report"
    }
} catch {
    Write-Error "Could not obtain Allure CLI automatically. Install Allure manually or ensure network access. Details: $_"
}