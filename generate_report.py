import json
import datetime

with open('conductor_status_dump_utf8.json', 'r', encoding='utf-8-sig') as f:
    data = json.load(f)

project_name = "PACX (Greg.Xrm.Command)"
timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

total_tracks = len(data)
tracks_in_progress = 0
tracks_pending = 0
tracks_completed = 0
tracks_blocked = 0
total_tasks = 0
total_completed_tasks = 0

overall_in_progress_task = "None"
overall_next_action_task = "None"

summary_rows = []
detail_blocks = []
summary_json = []

for track in data:
    track_id = track['Track']
    tasks = track.get('Tasks', [])
    if isinstance(tasks, dict):
        tasks = [tasks]
    if not tasks: tasks = []

    blockers = track.get('Blockers', 'None')

    t_done = sum(1 for t in tasks if t.get('Status') == 'Completed')
    t_prog = sum(1 for t in tasks if t.get('Status') == 'In Progress')
    t_pend = sum(1 for t in tasks if t.get('Status') == 'Pending')
    t_tot = len(tasks)

    # Normalize track status from task-level data to avoid stale/unknown track metadata.
    if t_pend > 0:
        status = "Pending"
    elif t_prog > 0:
        status = "In Progress"
    else:
        status = "Completed"

    if "progress" in status.lower():
        tracks_in_progress += 1
    elif "completed" in status.lower() or status == "merged":
        tracks_completed += 1
    elif "blocked" in status.lower():
        tracks_blocked += 1
    else:
        tracks_pending += 1

    total_tasks += t_tot
    total_completed_tasks += t_done

    summary_rows.append(f"| `{track_id}` | `{status}` | {t_done} | {t_prog} | {t_pend} | {t_tot} |")
    summary_json.append({
        "Track": track_id,
        "Status": status,
        "Done": t_done,
        "InProgress": t_prog,
        "Pending": t_pend,
        "Total": t_tot,
        "Current": "None",
        "Next": "None",
        "Blockers": blockers,
    })

    current_task = "None"
    next_task = "None"

    for t in tasks:
        if t.get('Status') == 'In Progress' and current_task == "None":
            current_task = t['Name']
            if overall_in_progress_task == "None":
                overall_in_progress_task = f"`{current_task}` in `{track_id}`"
        if t.get('Status') == 'Pending' and next_task == "None":
            next_task = t['Name']
            if overall_next_action_task == "None":
                overall_next_action_task = f"`{next_task}` in `{track_id}`"

    summary_json[-1]["Current"] = current_task
    summary_json[-1]["Next"] = next_task

    detail_blocks.append(f"**Track: `{track_id}`**\n- **Status:** `{status}`\n- **Current Task (IN PROGRESS):** {current_task}\n- **Next Action (PENDING):** {next_task}\n- **Blockers:** {blockers}\n")

pct = (total_completed_tasks / total_tasks * 100) if total_tasks > 0 else 0
project_status = "Blocked" if tracks_blocked > 0 else ("On Track" if tracks_in_progress > 0 or tracks_completed > 0 else "Pending")

md = f"""---

**🕐 Timestamp:** `{timestamp}`
**📦 Project:** `{project_name}`
**📊 Project Status:** `{project_status}`

---

#### Tracks Summary

| Track | Status | ✅ Done | 🔄 In Progress | ⬜ Pending | Total |
|-------|--------|---------|----------------|-----------|-------|
{chr(10).join(summary_rows)}

---

#### Per-Track Detail

{chr(10).join(detail_blocks)}
---

#### Overall Progress

| Metric | Value |
|--------|-------|
| Tracks | {total_tracks} total ({tracks_in_progress} in-progress, {tracks_pending} pending, {tracks_completed} completed) |
| Tasks | {total_completed_tasks} / {total_tasks} completed ({pct:.1f}%) |
| In Progress | {overall_in_progress_task} |
| Next Action | {overall_next_action_task} |

---

> [!TIP]
> Run `/conductor-implement` to begin the next pending task, or `/conductor-newtrack` to add a new track.
"""

with open('status_report.md', 'w', encoding='utf-8') as f:
    f.write(md)

with open('tracks_summary.json', 'w', encoding='utf-8') as f:
    json.dump(summary_json, f, ensure_ascii=False, indent=2)
