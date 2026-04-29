import json
import datetime

with open("logs/status_all.json", "r", encoding="utf-8") as f:
    tracks = json.load(f)

timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
project_name = "PACX (Greg.Xrm.Command)"

total_tracks = len(tracks)
in_progress_tracks = sum(1 for t in tracks if t["status"] == "in-progress")
pending_tracks = sum(1 for t in tracks if t["status"] == "pending")
completed_tracks = sum(1 for t in tracks if t["status"] == "completed")
blocked_tracks = sum(1 for t in tracks if t["status"] == "blocked")

project_status = "On Track"
if blocked_tracks > 0:
    project_status = "Blocked"

total_tasks = 0
completed_tasks_total = 0

tracks_summary = []
per_track_detail = []

first_in_progress = None
first_next_action = None

for t in tracks:
    t_id = t["track_id"]
    status = t["status"]
    tasks = t["tasks"]
    blockers = t["blockers"]

    total_t = len(tasks)
    done_t = sum(1 for task in tasks if task["status"] == "completed")
    prog_t = sum(1 for task in tasks if task["status"] == "in-progress")
    pend_t = sum(1 for task in tasks if task["status"] == "pending")

    total_tasks += total_t
    completed_tasks_total += done_t

    tracks_summary.append(f"| {t_id} | {status} | {done_t} | {prog_t} | {pend_t} | {total_t} |")

    current_task = next((task["name"] for task in tasks if task["status"] == "in-progress"), "None")
    next_action = next((task["name"] for task in tasks if task["status"] == "pending"), "None")
    blocker_text = ", ".join(blockers) if blockers else "None"

    if current_task != "None" and first_in_progress is None:
        first_in_progress = f"`{current_task}` in `{t_id}`"

    if next_action != "None" and first_next_action is None:
        first_next_action = f"`{next_action}` in `{t_id}`"

    per_track_detail.append(f"**Track: `{t_id}`**\n- **Status:** {status}\n- **Current Task (IN PROGRESS):** {current_task}\n- **Next Action (PENDING):** {next_action}\n- **Blockers:** {blocker_text}\n")

if first_in_progress is None: first_in_progress = "None"
if first_next_action is None: first_next_action = "None"

task_completion_pct = (completed_tasks_total / total_tasks * 100) if total_tasks > 0 else 0

report = f"""---

**?? Timestamp:** {timestamp}
**?? Project:** {project_name}
**?? Project Status:** {project_status}

---

#### Tracks Summary

| Track | Status | ? Done | ?? In Progress | ? Pending | Total |
|-------|--------|---------|----------------|-----------|-------|
"""
report += "\n".join(tracks_summary)

report += """

---

#### Per-Track Detail

"""
report += "\n".join(per_track_detail)

report += f"""---

#### Overall Progress

| Metric | Value |
|--------|-------|
| Tracks | {total_tracks} total ({in_progress_tracks} in-progress, {pending_tracks} pending, {completed_tracks} completed) |
| Tasks | {completed_tasks_total} / {total_tasks} completed ({task_completion_pct:.1f}%) |
| In Progress | {first_in_progress} |
| Next Action | {first_next_action} |

---

> [!TIP]
> Run `/conductor-implement` to begin the next pending task, or `/conductor-newtrack` to add a new track.
"""

with open("logs/report.md", "w", encoding="utf-8") as f:
    f.write(report)
