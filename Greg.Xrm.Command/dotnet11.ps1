param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $DotNetArgs
)

$dotnetRoot = Join-Path $env:LOCALAPPDATA 'Microsoft\dotnet'
$dotnetExe = Join-Path $dotnetRoot 'dotnet.exe'

if (-not (Test-Path -LiteralPath $dotnetExe)) {
    throw "dotnet.exe not found at $dotnetExe"
}

Remove-Item Env:MSBuildSDKsPath -ErrorAction SilentlyContinue
$env:DOTNET_ROOT = $dotnetRoot
$env:DOTNET_HOST_PATH = $dotnetExe
$env:DOTNET_CLI_HOME = Join-Path (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path '.dotnet-cli'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_ADD_GLOBAL_TOOLS_TO_PATH = '0'

& $dotnetExe @DotNetArgs '--disable-build-servers' '-p:UseSharedCompilation=false'
exit $LASTEXITCODE
