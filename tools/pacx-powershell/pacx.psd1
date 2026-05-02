@{
    RootModule        = 'pacx.psm1'
    ModuleVersion     = '0.1.0'
    GUID              = 'a3b9e5c1-8d4f-4a7e-9b2c-6f1d3e8a0c7b'
    Author            = 'Greg.Xrm.Command Contributors'
    CompanyName       = 'Greg.Xrm.Command'
    Copyright         = '(c) Greg.Xrm.Command. All rights reserved.'
    Description       = 'PowerShell module for PACX - Power Platform automation CLI extension for Dataverse, ALM, AI Builder, and governance workflows.'

    PowerShellVersion = '5.1'

    FunctionsToExport = @(
        'Get-PacxForm'
        'Get-PacxFormResponse'
        'Invoke-PacxSolution'
        'Get-PacxEnvironment'
        'New-PacxEnvironment'
    )

    CmdletsToExport   = @()
    VariablesToExport = @()
    AliasesToExport   = @()

    PrivateData = @{
        PSData = @{
            Tags         = @('PACX', 'PowerPlatform', 'Dataverse', 'D365', 'ALM')
            LicenseUri   = 'https://github.com/edithatogo/Greg.Xrm.Command/blob/master/LICENSE'
            ProjectUri   = 'https://github.com/edithatogo/Greg.Xrm.Command'
            IconUri      = 'https://raw.githubusercontent.com/edithatogo/Greg.Xrm.Command/master/Greg.Xrm.Command/icon128.png'
        }
    }
}
