$tracks = Get-ChildItem conductor/tracks -Directory | Where-Object { $_.Name -ne "archive" }
$results = @()

foreach ($track in $tracks) {
    $track_id = $track.Name
    $specPath = "conductor/tracks/$track_id/spec.md"
    $planPath = "conductor/tracks/$track_id/plan.md"
    
    $status = "Unknown"
    $tasks = @()
    $blockers = @()
    
    if (Test-Path $specPath) {
        $specLines = Get-Content $specPath
        $statusFound = $false
        foreach ($line in $specLines) {
            if ($line -match "^##\s*Status") {
                $statusFound = $true
                continue
            }
            if ($statusFound -and $line -match "^([A-Za-z].*)") {
                $statusText = $matches[1].Trim()
                if ($statusText -match "(pending|in-progress|completed|blocked)") {
                    $status = $matches[1].ToLower()
                } else {
                    $status = $statusText
                }
                $statusFound = $false
            }
            
            if ($line -match "BLOCKER:" -or $line -match "?") {
                $blockers += $line.Trim()
            }
        }
    }
    
    if (Test-Path $planPath) {
        $planLines = Get-Content $planPath
        foreach ($line in $planLines) {
            if ($line -match "^\s*-\s*\[(\s|x|/)\]\s*(.*)") {
                $checked = $matches[1]
                $taskName = $matches[2]
                $taskStatus = if ($checked -eq "x") { "completed" } elseif ($checked -eq "/") { "in-progress" } else { "pending" }
                $tasks += [PSCustomObject]@{ name = $taskName; status = $taskStatus }
            }
        }
    }
    
    $results += [PSCustomObject]@{
        track_id = $track_id
        status = $status
        tasks = $tasks
        blockers = $blockers
    }
}

$results | ConvertTo-Json -Depth 4 > "logs/status_all.json"
