# Troubleshooting Guide

Common issues when using PACX and how to resolve them.

## Authentication Failures

### "Unable to acquire access token" or "AADSTS700016"

**Cause:** The Azure AD app registration is not configured or lacks proper permissions.

**Fix:**
1. Verify `MSAL_CLIENT_ID` environment variable is set
2. Check the app registration exists in your tenant
3. Ensure admin consent has been granted for the required API permissions
4. Run `pacx diag` to verify environment configuration

### "AADSTS50034: User not found"

**Cause:** Using ROPC flow (username/password) with an incorrect username.

**Fix:**
1. Verify `MSAL_USERNAME` and `MSAL_PASSWORD` are correct
2. Ensure the account is not MFA-enabled (ROPC does not support MFA)
3. Use device code flow instead: `pacx auth create --auth-type device-code`

### "AADSTS7000215" or "invalid_client"

**Cause:** Invalid or expired client secret.

**Fix:**
1. Regenerate the client secret in Azure AD App Registration
2. Update `MSAL_CLIENT_SECRET` environment variable
3. Run `pacx auth create` to refresh the connection

## Connection Issues

### "Could not connect to Dataverse"

**Cause:** Network connectivity issue or incorrect environment URL.

**Fix:**
1. Verify the environment URL is correct: `https://{org}.crm.dynamics.com`
2. Check network connectivity: `ping {org}.crm.dynamics.com`
3. Ensure your IP is allowed through the firewall
4. Run `pacx diag` for connectivity diagnostics

### "Organization not found"

**Cause:** The environment URL doesn't exist or you don't have access.

**Fix:**
1. Verify the environment URL with your Power Platform admin
2. Ensure you have appropriate licenses assigned
3. Check if the environment is in a suspended state

## API Rate Limits

### "429 Too Many Requests"

**Cause:** API rate limit exceeded. The Forms API allows ~300 requests per 60 seconds.

**Fix:**
1. Wait 60 seconds and retry
2. Reduce parallel command execution
3. Use `--incremental` flag for response exports to minimize requests
4. Check `Retry-After` header in the error response

## Solution Errors

### "Solution not found"

**Cause:** The specified solution unique name doesn't exist in the current environment.

**Fix:**
1. Run `pacx solution list` to see available solutions
2. Verify the solution unique name (not display name)
3. Ensure you're connected to the correct environment with `pacx auth select`

### "Solution import failed"

**Cause:** Missing dependencies or version conflict.

**Fix:**
1. Check that all required components exist in the target environment
2. Verify the solution version is higher than the current version
3. Use `--overwrite` flag if replacing an unmanaged solution
4. Check Power Platform Admin Center for detailed error logs

## Environment Errors

### "Environment not found"

**Cause:** The environment ID doesn't exist or you don't have admin access.

**Fix:**
1. Run `pacx env list` to see available environments
2. Ensure you have Power Platform admin privileges
3. Check the environment ID format (GUID)

### "Environment reset failed"

**Cause:** The environment is in a state that cannot be reset (suspended, provisioning, deleting).

**Fix:**
1. Check environment status in Power Platform Admin Center
2. Wait for provisioning operations to complete
3. Use `--force` flag (with caution — this is irreversible)

## Forms Errors

### "Forms API error (401)"

**Cause:** Invalid or expired token for the Forms API.

**Fix:**
1. Ensure `MSAL_CLIENT_ID` and `MSAL_CLIENT_SECRET` are set
2. The Forms API requires the scope `https://forms.office.com/.default`
3. Tenant ID must be a GUID, not a domain name

### "No forms found"

**Cause:** The specified user has no forms, or the owner ID is incorrect.

**Fix:**
1. Verify the owner ID is a valid Entra ID object ID
2. For group forms, use `--owner-type Group`
3. Group forms require ROPC auth (delegated permissions)

## Power BI / Tabular Errors

### "Tabular deploy requires Premium capacity"

**Cause:** XMLA endpoints are only available on Power BI Premium or Embedded capacities.

**Fix:**
1. Ensure the target workspace is assigned to Premium capacity
2. Enable XMLA endpoints in the capacity settings
3. Use Power BI REST API as an alternative

## Plugin Errors

### "Plugin not found"

**Cause:** The specified plugin name does not match any installed plugin.

**Fix:**
1. Run `pacx tool list` to see installed plugins
2. Check the exact package name
3. Install the plugin with `pacx tool install --name <package-name>`

## Generic Troubleshooting

### Debug mode
Run any command with the environment variable `PACX_DEBUG=1` to see detailed HTTP request/response logging.

### Diagnostics
Run `pacx diag` to check:
- .NET runtime version
- PACX CLI version
- Dataverse connectivity (WhoAmI)
- Authentication status
- Environment variable configuration

### Logs
PACX logs are written to stderr. To capture logs:
```powershell
pacx solution list 2> pacx-debug.log
```

### Version check
```powershell
pacx version
```

### Update
```powershell
pacx self-update
```

### Report a bug
Open an issue at https://github.com/edithatogo/pac_pacx/issues with:
1. PACX version (`pacx version`)
2. Command run
3. Full error output
4. Steps to reproduce
