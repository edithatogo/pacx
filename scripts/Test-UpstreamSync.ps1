[CmdletBinding()]
param([switch]$Fetch)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$manifest = Get-Content -Raw -LiteralPath (Join-Path $root 'upstream.json') | ConvertFrom-Json

if ($manifest.canonical.repository -notmatch '^https://github\.com/neronotte/Greg\.Xrm\.Command\.git$') { throw 'Unexpected PACX upstream repository.' }
if ($manifest.canonical.branch -ne 'master') { throw 'Unexpected PACX upstream branch.' }
if ($manifest.integration.mode -ne 'detect-and-prepare') { throw 'Upstream integration must remain detect-and-prepare.' }

$upstreamUrl = [string]$manifest.canonical.repository
$remote = git -C $root remote get-url upstream 2>$null
if (-not $remote) { git -C $root remote add upstream $upstreamUrl }
elseif ($remote -ne $upstreamUrl) { throw "Upstream remote points to '$remote', not '$upstreamUrl'." }

if ($Fetch) { git -C $root fetch upstream --prune }
$head = (git -C $root rev-parse HEAD).Trim()
$upstreamHead = (git -C $root rev-parse 'upstream/master').Trim()
$common = git -C $root merge-base HEAD upstream/master 2>$null
$status = if ($common) { 'related-history' } else { 'structurally-divergent' }

[pscustomobject]@{
    repository = $upstreamUrl
    branch = 'master'
    localHead = $head
    upstreamHead = $upstreamHead
    commonAncestor = if ($common) { $common.Trim() } else { $null }
    status = $status
    integrationPolicy = $manifest.integration.mode
} | ConvertTo-Json -Depth 4
