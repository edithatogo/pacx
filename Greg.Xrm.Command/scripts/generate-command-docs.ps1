param(
	[string] $SourceRoot = "Greg.Xrm.Command",
	[string] $OutputRoot = "docs/reference/commands/generated"
)

$ErrorActionPreference = "Stop"

function Get-QuotedValues {
	param([string] $Text)
	[regex]::Matches($Text, '"([^"]+)"') | ForEach-Object { $_.Groups[1].Value }
}

function Get-HelpText {
	param([string] $Text)
	$match = [regex]::Match($Text, 'HelpText\s*=\s*"([^"]+)"')
	if ($match.Success) { return $match.Groups[1].Value }
	return ""
}

function Convert-ToSlug {
	param([string] $Text)
	return ($Text.ToLowerInvariant() -replace '[^a-z0-9]+', '-' -replace '(^-|-$)', '')
}

$repoRoot = (Resolve-Path ".").Path
$sourcePath = Join-Path $repoRoot $SourceRoot
$outputPath = Join-Path $repoRoot $OutputRoot
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

Get-ChildItem -LiteralPath $outputPath -Filter "*.md" -File | Remove-Item -Force

$commandFiles = Get-ChildItem -LiteralPath $sourcePath -Recurse -Filter "*.cs" |
	Where-Object { $_.FullName -match '\\Commands\\' }

$commands = @()

foreach ($file in $commandFiles) {
	$text = Get-Content -LiteralPath $file.FullName -Raw
	if ($null -eq $text) { continue }
	$matches = [regex]::Matches($text, '(?:\[Command\((?<args>[^\r\n]*)\)\]\s*(?:\[[^\r\n]+\]\s*)*)+public\s+(?:sealed\s+|partial\s+)?class\s+(?<class>[A-Za-z0-9_]+)', [System.Text.RegularExpressions.RegexOptions]::None)

	for ($i = 0; $i -lt $matches.Count; $i++) {
		$match = $matches[$i]
		$commandArgCaptures = @($match.Groups["args"].Captures)
		foreach ($commandArgCapture in $commandArgCaptures) {
		$commandArgs = $commandArgCapture.Value
		$quoted = @(Get-QuotedValues $commandArgs)
		if ($quoted.Count -eq 0) { continue }

		$parts = @()
		foreach ($value in $quoted) {
			if ($value -eq (Get-HelpText $commandArgs)) { continue }
			$parts += $value
		}

		$name = ($parts -join " ")
		if ([string]::IsNullOrWhiteSpace($name)) { continue }

		$nextStart = if ($i + 1 -lt $matches.Count) { $matches[$i + 1].Index } else { $text.Length }
		$classText = $text.Substring($match.Index, $nextStart - $match.Index)
		$options = New-Object System.Collections.Generic.List[object]
		foreach ($option in [regex]::Matches($classText, '\[Option\((?<args>[^\r\n]*)\)\]\s*public\s+(?<type>[A-Za-z0-9_<>\?\[\]]+)\s+(?<property>[A-Za-z0-9_]+)', [System.Text.RegularExpressions.RegexOptions]::None)) {
			$optionArgs = $option.Groups["args"].Value
			$optionQuoted = @(Get-QuotedValues $optionArgs)
			if ($optionQuoted.Count -eq 0) { continue }
			$options.Add([pscustomobject]@{
				Name = $optionQuoted[0]
				ShortName = if ($optionQuoted.Count -gt 1 -and $optionQuoted[1] -ne (Get-HelpText $optionArgs)) { $optionQuoted[1] } else { "" }
				Type = $option.Groups["type"].Value
				Property = $option.Groups["property"].Value
				Required = $optionArgs -match 'Required\s*=\s*true'
				HelpText = (Get-HelpText $optionArgs)
			})
		}

		$relativeSource = [IO.Path]::GetRelativePath($repoRoot, $file.FullName).Replace('\', '/')
		$slug = Convert-ToSlug $name
		$className = $match.Groups["class"].Value
		$helpText = Get-HelpText $commandArgs
		$optionList = $options.ToArray()
		$commands += [pscustomobject]@{
			Name = $name
			Slug = $slug
			ClassName = $className
			HelpText = $helpText
			Options = $optionList
			Source = $relativeSource
		}
		}
	}
}

$commands = $commands | Sort-Object Name, ClassName

$toc = New-Object System.Collections.Generic.List[string]
$toc.Add("# Generated Command Reference")
$toc.Add("")
$toc.Add("This folder is generated from `[Command]` and `[Option]` attributes.")
$toc.Add("")

foreach ($command in $commands) {
	$fileName = "$($command.Slug).md"
	$toc.Add("- [$($command.Name)]($fileName)")

	$page = New-Object System.Collections.Generic.List[string]
	$page.Add("# $($command.Name)")
	$page.Add("")
	if (-not [string]::IsNullOrWhiteSpace($command.HelpText)) {
		$page.Add($command.HelpText)
		$page.Add("")
	}
	$page.Add("## Usage")
	$page.Add("")
	$page.Add('```powershell')
	$page.Add("pacx $($command.Name)")
	$page.Add('```')
	$page.Add("")
	if ($command.Options.Count -gt 0) {
		$page.Add("## Options")
		$page.Add("")
		$page.Add("| Option | Short | Type | Required | Description |")
		$page.Add("| --- | --- | --- | --- | --- |")
		foreach ($option in $command.Options) {
			$page.Add("| ``--$($option.Name)`` | $($option.ShortName) | $($option.Type) | $($option.Required) | $($option.HelpText) |")
		}
		$page.Add("")
	}
	$page.Add("## Source")
	$page.Add("")
	$page.Add('`' + $command.Source + '`')
	$page.Add("")
	Set-Content -LiteralPath (Join-Path $outputPath $fileName) -Value $page -Encoding utf8
}

Set-Content -LiteralPath (Join-Path $outputPath "index.md") -Value $toc -Encoding utf8
Write-Output "Generated $($commands.Count) command reference page(s)."
