[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

function Assert([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
}

Assert (Test-Path -LiteralPath (Join-Path $root 'global.json')) 'global.json is required.'
Assert (Test-Path -LiteralPath (Join-Path $root 'Directory.Packages.props')) 'Central package management is required.'
Assert (Test-Path -LiteralPath (Join-Path $root '.github\dependabot.yml')) 'Dependabot configuration is required.'
Assert (Test-Path -LiteralPath (Join-Path $root 'upstream.json')) 'Canonical upstream manifest is required.'

$workflowFiles = Get-ChildItem -LiteralPath (Join-Path $root '.github\workflows') -Filter '*.yml' -File
foreach ($workflow in $workflowFiles) {
    $content = Get-Content -Raw -LiteralPath $workflow.FullName
    $actionLines = [regex]::Matches($content, '(?m)^[ \t]*-[ \t]*uses:[ \t]*(?<reference>[^#\r\n]+)')
    $unpinned = @($actionLines | Where-Object { $_.Groups['reference'].Value.Trim() -notmatch '@[0-9a-f]{40}(?:[ \t]|$)' })
    if ($unpinned.Count -gt 0) {
        throw "Workflow '$($workflow.Name)' contains unpinned action references: $(($unpinned | ForEach-Object { $_.Groups['reference'].Value.Trim() }) -join '; ')"
    }
}

$sensitivePatterns = @(
    '(?i)clientsecret\s*=\s*["''](?!(?:your-|account-)?password\b|passcode\b|\{)[^"'']{8,}["'']',
    '(?i)password\s*=\s*["''](?!(?:your-|account-)?password\b|passcode\b|\{)[^"'']{8,}["'']',
    '(?i)Bearer\s+["''][A-Za-z0-9._-]{20,}["'']',
    '(?i)token\s*[:=]\s*["''][A-Za-z0-9._-]{20,}["'']'
)
$tracked = git -C $root ls-files
foreach ($file in $tracked) {
    if ($file -match '(^|/)(bin|obj|\.git|node_modules|packages)(/|$)') { continue }
    $path = Join-Path $root $file
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { continue }
    $text = Get-Content -Raw -LiteralPath $path -ErrorAction SilentlyContinue
    foreach ($pattern in $sensitivePatterns) {
        if ($text -match $pattern) { throw "Potential secret pattern found in tracked file '$file'." }
    }
}

Write-Output "PACX harness checks passed: $($workflowFiles.Count) workflows, pinned actions, central packages, upstream manifest, and secret-pattern scan."
