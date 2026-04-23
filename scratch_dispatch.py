import os
import subprocess
import time
import json
import glob
from pathlib import Path

tracks_dir = Path("conductor/tracks")
logs_dir = Path("logs")
logs_dir.mkdir(exist_ok=True)

dispatch_script = r"C:\Users\60217257\.gemini\antigravity\skills\antigravity-swarm\scripts\dispatch_agent.py"
tracks = [d.name for d in tracks_dir.iterdir() if d.is_dir() and d.name != "archive"]

print(f"Total tracks to process: {len(tracks)}")

# Clean up old logs
for log_file in logs_dir.glob("status_*.json"):
    log_file.unlink()

# Process in batches to avoid rate limits / overloading
batch_size = 5
results = []

for i in range(0, len(tracks), batch_size):
    batch = tracks[i:i+batch_size]
    print(f"Processing batch {i//batch_size + 1} with {len(batch)} tracks...")
    procs = []
    
    for track in batch:
        cmd = [
            "python", dispatch_script,
            f"Read conductor/tracks/{track}/spec.md and conductor/tracks/{track}/plan.md. Output a JSON object with keys: track_id, status (from spec.md Status field), tasks (array of {{name, done}} from plan.md checkboxes). Print the JSON object only.",
            "--log-file", f"logs/status_{track}.json",
            "--format", "json",
            "--exit-on-idle"
        ]
        p = subprocess.Popen(cmd, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        procs.append((p, track))
    
    # Wait for batch
    for p, track in procs:
        p.wait()
        
print("All agents finished.")
