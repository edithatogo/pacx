param(
  [string]$BaselinePath,
  [string]$PRPath,
  [double]$Threshold = 5.0
)

$regressions = @()
$baselineFiles = Get-ChildItem -Path $BaselinePath -Recurse -Filter "*.json" | Where-Object { $_.Name -like "*-report.json" -or $_.Name -like "*-summary.json" }
$prFiles = Get-ChildItem -Path $PRPath -Recurse -Filter "*.json" | Where-Object { $_.Name -like "*-report.json" -or $_.Name -like "*-summary.json" }

if ($baselineFiles.Count -eq 0) {
  throw "No baseline benchmark JSON files found in '$BaselinePath'. Expected files matching *-report.json or *-summary.json."
}
if ($prFiles.Count -eq 0) {
  throw "No PR benchmark JSON files found in '$PRPath'. Expected files matching *-report.json or *-summary.json."
}

Write-Host "=== Benchmark Comparison ==="
Write-Host "Baseline: $BaselinePath"
Write-Host "PR:       $PRPath"
Write-Host "Threshold: $Threshold%"
Write-Host ""

foreach ($prFile in $prFiles) {
  $baselineFile = $baselineFiles | Where-Object { $_.Name -eq $prFile.Name } | Select-Object -First 1
  if (-not $baselineFile) {
    Write-Host "Warning: No matching baseline found for '$($prFile.Name)' — skipping"
    continue
  }

  $prData = Get-Content $prFile.FullName -Raw | ConvertFrom-Json
  $baselineData = Get-Content $baselineFile.FullName -Raw | ConvertFrom-Json

  $prBenchmarks = $prData.Benchmarks
  $baselineBenchmarks = $baselineData.Benchmarks

  if (-not $prBenchmarks -or -not $baselineBenchmarks) {
    Write-Host "Warning: No benchmarks array found in '$($prFile.Name)' — skipping"
    continue
  }

  foreach ($prBench in $prBenchmarks) {
    $baselineBench = $baselineBenchmarks | Where-Object { $_.FullName -eq $prBench.FullName } | Select-Object -First 1
    if (-not $baselineBench) {
      Write-Host "  ? New benchmark '$($prBench.FullName)' — no baseline to compare"
      continue
    }

    $prStats = $prBench.Statistics
    $baselineStats = $baselineBench.Statistics
    if (-not $prStats -or -not $baselineStats) {
      Write-Host "  ? Missing statistics for '$($prBench.FullName)' — skipping"
      continue
    }

    $prMean = [double]$prStats.Mean
    $baselineMean = [double]$baselineStats.Mean
    if ($baselineMean -eq 0) {
      Write-Host "  ? Baseline mean is zero for '$($prBench.FullName)' — skipping"
      continue
    }

    $change = [math]::Round(($prMean - $baselineMean) / $baselineMean * 100, 2)
    $prStdDev = if ($prStats.StandardDeviation) { [double]$prStats.StandardDeviation } else { 0 }
    $baselineStdDev = if ($baselineStats.StandardDeviation) { [double]$baselineStats.StandardDeviation } else { 0 }

    if ($change -gt $Threshold) {
      $regressions += $prBench.FullName
      Write-Host "  ❌ REGRESSION: $($prBench.FullName)"
    } elseif ($change -lt (-1 * $Threshold)) {
      Write-Host "  ✅ IMPROVEMENT: $($prBench.FullName)"
    } else {
      Write-Host "  ✓ $($prBench.FullName)"
    }

    Write-Host "       Baseline: $([math]::Round($baselineMean, 2)) ns (σ=$([math]::Round($baselineStdDev, 2)))"
    Write-Host "       PR:       $([math]::Round($prMean, 2)) ns (σ=$([math]::Round($prStdDev, 2)))"
    Write-Host "       Change:   $change%"
    Write-Host ""
  }
}

Write-Host "========================"
if ($regressions.Count -gt 0) {
  Write-Host "FAILED: $($regressions.Count) benchmark(s) exceeded the $Threshold% regression threshold:"
  $regressions | ForEach-Object { Write-Host "  - $_" }
  exit 1
}

Write-Host "PASSED: All $($prFiles.Count) benchmark file(s) within the $Threshold% regression threshold."
exit 0
