import os
import json
import re

tracks_dir = "conductor/tracks"
results = []

for track_name in os.listdir(tracks_dir):
    if track_name == "archive":
        continue
        
    track_path = os.path.join(tracks_dir, track_name)
    if not os.path.isdir(track_path):
        continue
        
    spec_path = os.path.join(track_path, "spec.md")
    plan_path = os.path.join(track_path, "plan.md")
    
    status = "unknown"
    tasks = []
    blockers = []
    
    if os.path.exists(spec_path):
        with open(spec_path, 'r', encoding='utf-8') as f:
            lines = f.readlines()
            
        status_found = False
        for line in lines:
            line = line.strip()
            if re.match(r'^##\s+Status', line):
                status_found = True
                continue
            
            if status_found and re.match(r'^[A-Za-z*]', line):
                s = line.strip().lower().replace('*', '')
                if "pending" in s: status = "pending"
                elif "in-progress" in s: status = "in-progress"
                elif "completed" in s: status = "completed"
                elif "blocked" in s: status = "blocked"
                else: status = s
                status_found = False
                
            if "BLOCKER:" in line or "?" in line:
                blockers.append(line.replace("BLOCKER:", "").strip())
                
    if os.path.exists(plan_path):
        with open(plan_path, 'r', encoding='utf-8') as f:
            lines = f.readlines()
            
        for line in lines:
            match = re.match(r'^\s*-\s*\[(\s|x|/)\]\s*(.*)', line)
            if match:
                checked = match.group(1)
                task_name = match.group(2).strip()
                if checked == "x":
                    task_status = "completed"
                elif checked == "/":
                    task_status = "in-progress"
                else:
                    task_status = "pending"
                tasks.append({"name": task_name, "status": task_status})
                
    results.append({
        "track_id": track_name,
        "status": status,
        "tasks": tasks,
        "blockers": blockers
    })

os.makedirs("logs", exist_ok=True)
with open("logs/status_all.json", "w", encoding="utf-8") as f:
    json.dump(results, f, indent=2)
