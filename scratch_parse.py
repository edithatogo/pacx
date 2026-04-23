import os
import re
from pathlib import Path
from datetime import datetime

# Read product.md to get project name
try:
    with open("conductor/product.md", "r", encoding="utf-8") as f:
        product_content = f.read()

    project_match = re.search(r"# Initial Concept\n(.*?)(?:\n|$)", product_content, re.IGNORECASE)
    project_name = project_match.group(1).strip() if project_match else "PACX"
except Exception:
    project_name = "PACX"

tracks_dir = Path("conductor/tracks")
tracks = []

for track_dir in tracks_dir.iterdir():
    if not track_dir.is_dir() or track_dir.name == "archive":
        continue
    
    track_id = track_dir.name
    spec_file = track_dir / "spec.md"
    plan_file = track_dir / "plan.md"
    
    status = "Unknown"
    if spec_file.exists():
        spec_content = spec_file.read_text(encoding="utf-8")
        status_match = re.search(r"## Status\s*\n\s*(in-progress|pending|completed|blocked)", spec_content, re.IGNORECASE)
        if status_match:
            status = status_match.group(1).lower()
        else:
            status_match = re.search(r"\*\*Status:\*\*\s*(in-progress|pending|completed|blocked)", spec_content, re.IGNORECASE)
            if status_match:
                status = status_match.group(1).lower()

    tasks = []
    blockers = []
    if plan_file.exists():
        plan_content = plan_file.read_text(encoding="utf-8")
        for line in plan_content.splitlines():
            line = line.strip()
            # Extract task name by splitting on the checkbox
            if "- [ ]" in line:
                tasks.append({"name": line.split("- [ ]", 1)[1].strip(), "state": "pending"})
            elif "- [/]" in line:
                tasks.append({"name": line.split("- [/]", 1)[1].strip(), "state": "in-progress"})
            elif "- [x]" in line.lower():
                tasks.append({"name": line.lower().split("- [x]", 1)[1].strip(), "state": "completed"})
            
            if "BLOCKER:" in line.upper() or "❌" in line:
                blockers.append(line)

    tracks.append({
        "track_id": track_id,
        "status": status,
        "tasks": tasks,
        "blockers": blockers
    })

# Format the report
total_tracks = len(tracks)
completed_tracks = sum(1 for t in tracks if t["status"] == "completed")
in_progress_tracks = sum(1 for t in tracks if t["status"] == "in-progress")
pending_tracks = sum(1 for t in tracks if t["status"] == "pending")

total_tasks = sum(len(t["tasks"]) for t in tracks)
completed_tasks = sum(sum(1 for task in t["tasks"] if task["state"] == "completed") for t in tracks)
task_percentage = int((completed_tasks / total_tasks * 100)) if total_tasks > 0 else 0

any_blockers = any(len(t["blockers"]) > 0 for t in tracks)
if any_blockers:
    project_status = "Blocked"
elif in_progress_tracks > 0 or pending_tracks > 0:
    project_status = "On Track"
elif pending_tracks == 0 and in_progress_tracks == 0:
    project_status = "Completed"
else:
    project_status = "Pending"

# Overall In Progress and Next Action
overall_in_progress = "None"
overall_next_action = "None"

for t in tracks:
    if overall_in_progress != "None":
        break
    for task in t["tasks"]:
        if task["state"] == "in-progress":
            overall_in_progress = f"`{task['name']}` in `{t['track_id']}`"
            break

for t in tracks:
    if overall_next_action != "None":
        break
    if t["status"] in ["in-progress", "pending"]:
        for task in t["tasks"]:
            if task["state"] == "pending":
                overall_next_action = f"`{task['name']}` in `{t['track_id']}`"
                break

timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")

report = f"""---

**🕐 Timestamp:** `{timestamp}`
**📦 Project:** `{project_name}`
**📊 Project Status:** `{project_status}`

---

#### Tracks Summary

| Track | Status | ✅ Done | 🔄 In Progress | ⬜ Pending | Total |
|-------|--------|---------|----------------|-----------|-------|
"""

for t in tracks:
    done = sum(1 for task in t["tasks"] if task["state"] == "completed")
    in_prog = sum(1 for task in t["tasks"] if task["state"] == "in-progress")
    pend = sum(1 for task in t["tasks"] if task["state"] == "pending")
    tot = len(t["tasks"])
    report += f"| `{t['track_id']}` | `{t['status']}` | {done} | {in_prog} | {pend} | {tot} |\n"

report += """
---

#### Per-Track Detail
"""

for t in tracks:
    in_prog_task = next((task['name'] for task in t["tasks"] if task["state"] == "in-progress"), "None")
    pend_task = next((task['name'] for task in t["tasks"] if task["state"] == "pending"), "None")
    blocker_text = ", ".join(t["blockers"]) if t["blockers"] else "None"
    
    report += f"""
**Track: `{t['track_id']}`**
- **Status:** `{t['status']}`
- **Current Task (IN PROGRESS):** `{in_prog_task}`
- **Next Action (PENDING):** `{pend_task}`
- **Blockers:** `{blocker_text}`
"""

report += f"""
---

#### Overall Progress

| Metric | Value |
|--------|-------|
| Tracks | {total_tracks} total ({in_progress_tracks} in-progress, {pending_tracks} pending, {completed_tracks} completed) |
| Tasks | {completed_tasks} / {total_tasks} completed ({task_percentage}%) |
| In Progress | {overall_in_progress} |
| Next Action | {overall_next_action} |

---

> [!TIP]
> Run `/conductor-implement` to begin the next pending task, or `/conductor-newtrack` to add a new track.
"""

with open("scratch_report.md", "w", encoding="utf-8") as f:
    f.write(report)
