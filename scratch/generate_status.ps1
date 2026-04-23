$tracksDir = "C:\Users\60217257\repos\pac_pacx\conductor\tracks"
$tracks = Get-ChildItem -Path $tracksDir -Directory | Where-Object { $_.Name -ne 'archive' }

$report = @()
$totalTasks = 0
$completedTasks = 0
$totalPendingTasks = 0
$totalInProgressTasks = 0
$firstInProgressTask = "None"
$firstInProgressTrack = "None"
$firstNextActionTask = "None"
$firstNextActionTrack = "None"

$trackInProg = 0
$trackPending = 0
$trackCompleted = 0

foreach ($dir in $tracks) {
    $trackId = $dir.Name
    $metaFile = Join-Path $dir.FullName "metadata.json"
    $planFile = Join-Path $dir.FullName "plan.md"
    
    $status = "pending"
    if (Test-Path $metaFile) {
        $meta = Get-Content $metaFile | ConvertFrom-Json
        $status = $meta.status
    }
    
    if ($status -eq "in-progress") { $trackInProg++ }
    elseif ($status -eq "completed") { $trackCompleted++ }
    else { $trackPending++ }
    
    $tasksDone = 0
    $tasksInProg = 0
    $tasksPending = 0
    $totalLocal = 0
    
    $currentTask = "None"
    $nextAction = "None"
    $blockers = "None"
    
    if (Test-Path $planFile) {
        $lines = Get-Content $planFile
        foreach ($line in $lines) {
            if ($line -match "BLOCKER:(.+)") {
                if ($blockers -eq "None") { $blockers = $matches[1].Trim() }
                else { $blockers += ", " + $matches[1].Trim() }
            }
            if ($line -match "- \[(x|/)\] (Task:.*|.*)") {
                $taskName = $line -replace "^\s*- \[(x|/)\]\s*", ""
                if ($line -match "\[x\]") { 
                    $tasksDone++; $totalTasks++; $totalLocal++; $completedTasks++ 
                }
                elseif ($line -match "\[/\]") { 
                    $tasksInProg++; $totalTasks++; $totalLocal++; $totalInProgressTasks++
                    if ($currentTask -eq "None") { $currentTask = $taskName }
                    if ($firstInProgressTask -eq "None") {
                        $firstInProgressTask = $taskName
                        $firstInProgressTrack = $trackId
                    }
                }
            }
            elseif ($line -match "- \[ \] (Task:.*|.*)") {
                $taskName = $line -replace "^\s*- \[ \]\s*", ""
                $tasksPending++; $totalTasks++; $totalLocal++; $totalPendingTasks++
                if ($nextAction -eq "None") { $nextAction = $taskName }
                if ($firstNextActionTask -eq "None" -and $status -ne "completed") {
                    $firstNextActionTask = $taskName
                    $firstNextActionTrack = $trackId
                }
            }
        }
    }
    
    $report += [PSCustomObject]@{
        Track = $trackId
        Status = $status
        Done = $tasksDone
        InProgress = $tasksInProg
        Pending = $tasksPending
        Total = $totalLocal
        CurrentTask = $currentTask
        NextAction = $nextAction
        Blockers = $blockers
    }
}

$date = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
$projName = "PACX (Greg.Xrm.Command)"
$projStatus = if ($trackInProg -gt 0 -or $trackCompleted -gt 0) { "On Track" } else { "Behind Schedule" }
if ($report.Blockers -ne "None") { $projStatus = "Blocked" }

$md = @"
---

**🕐 Timestamp:** $date
**📦 Project:** $projName
**📊 Project Status:** $projStatus

---

#### Tracks Summary

| Track | Status | ✅ Done | 🔄 In Progress | ⬜ Pending | Total |
|-------|--------|---------|----------------|-----------|-------|
"@

foreach ($r in $report) {
    if ($r.Status -eq "in-progress" -or $r.Status -eq "pending" -or $r.Status -eq "completed") {
        $md += "`n| $($r.Track) | $($r.Status) | $($r.Done) | $($r.InProgress) | $($r.Pending) | $($r.Total) |"
    } else {
        $md += "`n| $($r.Track) | $($r.Status) | $($r.Done) | $($r.InProgress) | $($r.Pending) | $($r.Total) |"
    }
}

$md += @"

---

#### Per-Track Detail

"@

foreach ($r in $report) {
    $md += "
**Track: $($r.Track)**
- **Status:** $($r.Status)
- **Current Task (IN PROGRESS):** $($r.CurrentTask)
- **Next Action (PENDING):** $($r.NextAction)
- **Blockers:** $($r.Blockers)
"
}

$pct = 0
if ($totalTasks -gt 0) { $pct = [math]::Round(($completedTasks / $totalTasks) * 100, 2) }

$md += @"
---

#### Overall Progress

| Metric | Value |
|--------|-------|
| Tracks | $($tracks.Count) total ($trackInProg in-progress, $trackPending pending, $trackCompleted completed) |
| Tasks | $completedTasks / $totalTasks completed ($pct%) |
| In Progress | $firstInProgressTask in $firstInProgressTrack |
| Next Action | $firstNextActionTask in $firstNextActionTrack |

---

> [!TIP]
> Run `/conductor-implement` to begin the next pending task, or `/conductor-newtrack` to add a new track.
"@

$md | Out-File -FilePath "$tracksDir\..\status_report.md" -Encoding utf8
Write-Output "Report generated at $tracksDir\..\status_report.md"
