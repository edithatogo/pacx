$script:PacxPath = if (Get-Command pacx -ErrorAction SilentlyContinue) {
    (Get-Command pacx).Source
} else {
    "pacx"
}

function Invoke-Pacx {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromRemainingArguments)]
        [string[]]$Arguments
    )

    $process = Start-Process -FilePath $script:PacxPath -ArgumentList $Arguments -NoNewWindow -PassThru -Wait -RedirectStandardOutput "stdout.tmp" -RedirectStandardError "stderr.tmp"
    $output = Get-Content "stdout.tmp" -Raw
    $errorOutput = Get-Content "stderr.tmp" -Raw
    Remove-Item "stdout.tmp", "stderr.tmp" -Force -ErrorAction SilentlyContinue

    if ($process.ExitCode -ne 0) {
        $errorMessage = if ($errorOutput) { $errorOutput.Trim() } else { $output.Trim() }
        Write-Error "pacx exited with code $($process.ExitCode): $errorMessage"
        return
    }

    if ($output) { $output.Trim() }
}

<#
.SYNOPSIS
Lists all Microsoft Forms with metadata in a tenant.
.DESCRIPTION
Wraps `pacx forms list`. Returns form ID, title, status, response count, owner, and timestamps.
.PARAMETER TenantId
Tenant ID or domain (e.g., contoso.onmicrosoft.com).
.PARAMETER OwnerId
Owner user ID. Defaults to current user.
.PARAMETER OwnerType
Owner type: User or Group. Defaults to User.
.PARAMETER Format
Output format: table or json. Defaults to json for PowerShell consumption.
.PARAMETER Token
OAuth2 access token. Reads from MSAL cache or environment if not provided.
.EXAMPLE
Get-PacxForm -TenantId contoso.onmicrosoft.com
.EXAMPLE
Get-PacxForm -TenantId contoso.onmicrosoft.com -Format table
#>
function Get-PacxForm {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$TenantId,

        [Parameter()]
        [string]$OwnerId,

        [Parameter()]
        [ValidateSet('User', 'Group')]
        [string]$OwnerType = 'User',

        [Parameter()]
        [ValidateSet('table', 'json')]
        [string]$Format = 'json',

        [Parameter()]
        [string]$Token
    )

    $args = @('forms', 'list', '--tenant', $TenantId, '--owner-type', $OwnerType, '--format', $Format)
    if ($OwnerId) { $args += '--owner'; $args += $OwnerId }
    if ($Token) { $args += '--token'; $args += $Token }

    $result = Invoke-Pacx @args
    if ($Format -eq 'json' -and $result) {
        $result | ConvertFrom-Json
    } else {
        $result
    }
}

<#
.SYNOPSIS
Gets responses from a Microsoft Form.
.DESCRIPTION
Wraps `pacx forms response get` for single responses or `pacx forms responses export`.
.PARAMETER TenantId
Tenant ID or domain (e.g., contoso.onmicrosoft.com).
.PARAMETER FormId
Form ID to get responses from.
.PARAMETER ResponseId
Optional response ID to retrieve a single response.
.PARAMETER OutputPath
File path to export responses. If not provided, returns JSON to pipeline.
.PARAMETER Incremental
Only export responses since last export.
.PARAMETER OwnerId
Owner user ID.
.PARAMETER OwnerType
Owner type: User or Group.
.PARAMETER Token
OAuth2 access token.
.EXAMPLE
Get-PacxFormResponse -TenantId contoso.onmicrosoft.com -FormId "form-id-123"
.EXAMPLE
Get-PacxFormResponse -TenantId contoso.onmicrosoft.com -FormId "form-id-123" -ResponseId 42
#>
function Get-PacxFormResponse {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$TenantId,

        [Parameter(Mandatory)]
        [string]$FormId,

        [Parameter()]
        [int]$ResponseId,

        [Parameter()]
        [string]$OutputPath,

        [Parameter()]
        [switch]$Incremental,

        [Parameter()]
        [string]$OwnerId,

        [Parameter()]
        [ValidateSet('User', 'Group')]
        [string]$OwnerType = 'User',

        [Parameter()]
        [string]$Token
    )

    if ($ResponseId) {
        $args = @('forms', 'response', 'get', '--tenant', $TenantId, '--form-id', $FormId, '--response-id', $ResponseId)
        if ($OwnerId) { $args += '--owner'; $args += $OwnerId }
        if ($OwnerType) { $args += '--owner-type'; $args += $OwnerType }
        if ($Token) { $args += '--token'; $args += $Token }
        $result = Invoke-Pacx @args
        if ($result) { $result | ConvertFrom-Json }
        return
    }

    $format = if ($OutputPath) {
        switch -Wildcard ([System.IO.Path]::GetExtension($OutputPath)) {
            '.json' { 'json' }
            '.sql'  { 'sql' }
            default { 'csv' }
        }
    } else {
        $OutputPath = [System.IO.Path]::GetTempFileName() + '.json'
        $format = 'json'
    }

    $args = @('forms', 'responses', 'export', '--tenant', $TenantId, '--form-id', $FormId, '--output', $OutputPath, '--format', $format)
    if ($OwnerId) { $args += '--owner'; $args += $OwnerId }
    if ($OwnerType) { $args += '--owner-type'; $args += $OwnerType }
    if ($Incremental) { $args += '--incremental' }
    if ($Token) { $args += '--token'; $args += $Token }

    Invoke-Pacx @args

    if ($format -eq 'json' -and -not $OutputPath) {
        Get-Content $OutputPath -Raw | ConvertFrom-Json
        Remove-Item $OutputPath -Force
    } elseif ($OutputPath) {
        Get-Item $OutputPath
    }
}

<#
.SYNOPSIS
Invokes PACX solution commands.
.DESCRIPTION
Wraps `pacx solution` commands including list, create, delete, export, and component management.
.PARAMETER Command
Solution subcommand: list, create, delete, export, component-add, component-list, component-remove.
.PARAMETER Type
Filter by solution type: Managed, Unmanaged, Both.
.PARAMETER Hidden
Show hidden solutions.
.PARAMETER Format
Output format: table, json, table-compact.
.PARAMETER OrderBy
Sort order: Name, CreatedOn, ModifiedOn, Type.
.PARAMETER Name
Solution display name (for create).
.PARAMETER PublisherPrefix
Publisher prefix (for create).
.PARAMETER SolutionId
Solution ID (for delete, component operations).
.PARAMETER ComponentType
Component type name (for component-add).
.PARAMETER ComponentId
Component schema name or ID (for component-add, component-remove).
.EXAMPLE
Invoke-PacxSolution -Command list
.EXAMPLE
Invoke-PacxSolution -Command create -Name "MySolution" -PublisherPrefix "contoso"
#>
function Invoke-PacxSolution {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateSet('list', 'create', 'delete', 'export', 'component-add', 'component-list', 'component-remove')]
        [string]$Command,

        [Parameter()]
        [ValidateSet('Managed', 'Unmanaged', 'Both')]
        [string]$Type,

        [Parameter()]
        [switch]$Hidden,

        [Parameter()]
        [ValidateSet('table', 'json', 'table-compact')]
        [string]$Format = 'table',

        [Parameter()]
        [ValidateSet('Name', 'CreatedOn', 'ModifiedOn', 'Type')]
        [string]$OrderBy,

        [Parameter()]
        [string]$Name,

        [Parameter()]
        [string]$PublisherPrefix,

        [Parameter()]
        [string]$SolutionId,

        [Parameter()]
        [string]$ComponentType,

        [Parameter()]
        [string]$ComponentId
    )

    $args = @('solution', $Command)
    switch ($Command) {
        'list' {
            if ($Type) { $args += '--type'; $args += $Type }
            if ($Hidden) { $args += '--hidden' }
            if ($Format) { $args += '--format'; $args += $Format }
            if ($OrderBy) { $args += '--orderby'; $args += $OrderBy }
        }
        'create' {
            $args += '--name'; $args += $Name
            $args += '--publisher-prefix'; $args += $PublisherPrefix
        }
        'delete' {
            $args += '--solution-id'; $args += $SolutionId
        }
        'export' {
            $args += '--solution-id'; $args += $SolutionId
        }
        'component-add' {
            $args += '--solution-id'; $args += $SolutionId
            $args += '--component-type'; $args += $ComponentType
            $args += '--component-id'; $args += $ComponentId
        }
        'component-list' {
            $args += '--solution-id'; $args += $SolutionId
        }
        'component-remove' {
            $args += '--solution-id'; $args += $SolutionId
            $args += '--component-type'; $args += $ComponentType
            $args += '--component-id'; $args += $ComponentId
        }
    }

    Invoke-Pacx @args
}

<#
.SYNOPSIS
Gets Power Platform environments.
.DESCRIPTION
Wraps `pacx env` commands. Lists environments or gets capacity report.
.PARAMETER Command
Environment subcommand: list, capacity.
.PARAMETER EnvironmentId
Filter by specific environment ID.
.PARAMETER Format
Output format: table or json.
.EXAMPLE
Get-PacxEnvironment
.EXAMPLE
Get-PacxEnvironment -Command capacity -EnvironmentId "env-id-123" -Format json
#>
function Get-PacxEnvironment {
    [CmdletBinding()]
    param(
        [Parameter()]
        [ValidateSet('list', 'capacity')]
        [string]$Command = 'list',

        [Parameter()]
        [string]$EnvironmentId,

        [Parameter()]
        [ValidateSet('table', 'json')]
        [string]$Format = 'table'
    )

    $args = @('env', 'capacity', 'report', '--format', $Format)
    if ($EnvironmentId) { $args += '--environment'; $args += $EnvironmentId }

    Invoke-Pacx @args
}

<#
.SYNOPSIS
Creates a new Power Platform environment.
.DESCRIPTION
Wraps `pacx env create`. Supports Developer, Sandbox, Production, and Trial types.
.PARAMETER Name
Environment display name.
.PARAMETER Type
Environment type: Developer, Sandbox, Production, Trial.
.PARAMETER Region
Geographic region (e.g., unitedstates, europe, asia).
.PARAMETER Currency
Base currency code (default: USD).
.PARAMETER Language
Base language code (default: en-US).
.PARAMETER SecurityGroupId
Azure AD security group ID for access control.
.PARAMETER Wait
Wait for environment provisioning to complete.
.PARAMETER Format
Output format: table or json.
.EXAMPLE
New-PacxEnvironment -Name "Dev Environment" -Type Developer
.EXAMPLE
New-PacxEnvironment -Name "Sandbox" -Type Sandbox -Region europe -Wait
#>
function New-PacxEnvironment {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Name,

        [Parameter()]
        [ValidateSet('Developer', 'Sandbox', 'Production', 'Trial')]
        [string]$Type = 'Sandbox',

        [Parameter()]
        [string]$Region,

        [Parameter()]
        [string]$Currency = 'USD',

        [Parameter()]
        [string]$Language = 'en-US',

        [Parameter()]
        [string]$SecurityGroupId,

        [Parameter()]
        [switch]$Wait,

        [Parameter()]
        [ValidateSet('table', 'json')]
        [string]$Format = 'table'
    )

    $args = @('env', 'create', '--name', $Name, '--type', $Type, '--currency', $Currency, '--language', $Language, '--format', $Format)
    if ($Region) { $args += '--region'; $args += $Region }
    if ($SecurityGroupId) { $args += '--security-group'; $args += $SecurityGroupId }
    if ($Wait) { $args += '--wait' }

    Invoke-Pacx @args
}

Export-ModuleMember -Function Get-PacxForm, Get-PacxFormResponse, Invoke-PacxSolution, Get-PacxEnvironment, New-PacxEnvironment
