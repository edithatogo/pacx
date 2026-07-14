[CmdletBinding()]
param(
    [switch]$Fetch,
    [string]$OutputPath = (Join-Path ([System.IO.Path]::GetTempPath()) 'pacx-upstream-capability-report.json')
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$upstreamUrl = 'https://github.com/neronotte/Greg.Xrm.Command.git'

$remote = git -C $root remote get-url upstream 2>$null
if (-not $remote) {
    git -C $root remote add upstream $upstreamUrl
} elseif ($remote -ne $upstreamUrl) {
    throw "Upstream remote points to '$remote', not '$upstreamUrl'."
}

if ($Fetch) { git -C $root fetch upstream --prune }

$localHead = (git -C $root rev-parse HEAD).Trim()
$upstreamHead = (git -C $root rev-parse upstream/master).Trim()
$commonAncestor = git -C $root merge-base HEAD upstream/master 2>$null
$localFiles = @((git -C $root ls-tree -r --name-only HEAD) | Where-Object { $_ } | ForEach-Object {
    # The PACX fork stores the application under this directory; upstream stores it at repository root.
    $_ -replace '^Greg\.Xrm\.Command/', ''
})
$upstreamFiles = @((git -C $root ls-tree -r --name-only upstream/master) | Where-Object { $_ })
$localSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
$upstreamSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
$localFiles | ForEach-Object { [void]$localSet.Add($_) }
$upstreamFiles | ForEach-Object { [void]$upstreamSet.Add($_) }

$upstreamOnly = @($upstreamFiles | Where-Object { -not $localSet.Contains($_) })
$localOnly = @($localFiles | Where-Object { -not $upstreamSet.Contains($_) })

function Get-Area([string]$Path) {
    if ($Path -match '^\.github/workflows/') { return 'github-workflows' }
    if ($Path -match '^(?:Greg\.Xrm\.Command/)?Greg\.Xrm\.Command\.Core/Commands/([^/]+)/') { return "commands:$($Matches[1])" }
    if ($Path -match '^(?:Greg\.Xrm\.Command/)?Greg\.Xrm\.Command\.Core/Services/([^/]+)/') { return "services:$($Matches[1])" }
    if ($Path -match '^(?:Greg\.Xrm\.Command/)?(?:test|Greg\.Xrm\.Command\.Core\.TestSuite)/') { return 'tests' }
    if ($Path -match '^docs/') { return 'docs' }
    if ($Path -match '^tools/') { return 'tools' }
    return 'other'
}

function Get-AreaCounts([string[]]$Paths) {
    @($Paths | ForEach-Object { Get-Area $_ } | Group-Object | Sort-Object Count -Descending | ForEach-Object {
        [pscustomobject]@{ area = $_.Name; files = $_.Count }
    })
}

$report = [ordered]@{
    schemaVersion = 1
    repository = 'https://github.com/edithatogo/pacx.git'
    upstream = $upstreamUrl
    localHead = $localHead
    upstreamHead = $upstreamHead
    commonAncestor = if ($commonAncestor) { $commonAncestor.Trim() } else { $null }
    status = if ($commonAncestor) { 'related-history' } else { 'structurally-divergent' }
    policy = 'Inventory only. Do not merge or copy files without a bounded capability decision.'
    localFileCount = $localFiles.Count
    upstreamFileCount = $upstreamFiles.Count
    upstreamOnlyFileCount = $upstreamOnly.Count
    localOnlyFileCount = $localOnly.Count
    upstreamOnlyAreas = @(Get-AreaCounts $upstreamOnly)
    localOnlyAreas = @(Get-AreaCounts $localOnly)
    upstreamOnlySamples = @($upstreamOnly | Select-Object -First 50)
}

$parent = Split-Path -Parent $OutputPath
if ($parent -and -not (Test-Path -LiteralPath $parent)) {
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
}
$report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $OutputPath -Encoding utf8
$report | ConvertTo-Json -Depth 8
