# Implementation Plan: CLI Excellence

## Anti-Stub Preamble
Every task produces a **verifiable artifact**: a compilable command class + executor + passing test. No task may be marked [x] without its verification step. `/conductor-review` auto-triggers at every phase boundary. Any stub, TODO, "deferred", or "future work" comment causes immediate rejection.

## Overview
State-of-the-art CLI features: self-update, diagnostics, silent/quiet modes, progress bars, YAML output, and auto-discovered configuration — all following the established PACX command patterns.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Self-Update Command

**Step 1: Analyze**
- Read `Program.cs` — understand how the CLI entry point works
- Read `Greg.Xrm.Command.csproj` — find assembly version source
- Read existing command patterns (`Commands/`) — model + executor pairs
- Check if any update-related code exists
- Read GitHub Releases API format

**Step 2: Implement**

Create **`Commands/Update/SelfUpdateCommand.cs`**:
```csharp
namespace Greg.Xrm.Command.Commands.Update
{
    [Command("self-update", HelpText = "Check for and install the latest version of PACX.")]
    [Alias("update")]
    public class SelfUpdateCommand
    {
        [Option("check", "c", Order = 1, HelpText = "Only check for updates without installing.")]
        public bool CheckOnly { get; set; }

        [Option("version", "v", Order = 2, HelpText = "Install a specific version instead of latest.")]
        public string? Version { get; set; }

        [Option("pre-release", "p", Order = 3, HelpText = "Include pre-release versions.")]
        public bool IncludePrerelease { get; set; }
    }
}
```

Create **`Commands/Update/SelfUpdateCommandExecutor.cs`**:
- Fetch latest release from `https://api.github.com/repos/{owner}/{repo}/releases/latest`
- Compare current assembly version (`Assembly.GetEntryAssembly().GetName().Version`)
- If newer version exists, download and extract the asset
- On Windows: replace the running assembly (copy to temp, swap, restart)
- On Linux/macOS: download to current directory, make executable
- Tests: mock GitHub API with `HttpMessageHandler`

Create **`Commands/Update/Help.cs`**:
```csharp
namespace Greg.Xrm.Command.Commands.Update
{
    public class Help : NamespaceHelperBase
    {
        public Help() : base("Manages PACX updates", "self-update", "update") { }
    }
}
```

**Step 3: Anti-Stub Verification**
- [ ] Command class defined with `[Command("self-update")]` — verified by reading
- [ ] Executor implements `ICommandExecutor<SelfUpdateCommand>` — verified
- [ ] GitHub API is called (no stub) — verified by reading executor code
- [ ] Version comparison logic exists — verified
- [ ] Download and replace logic exists — verified
- [ ] Parse tests written — verified by test file existence
- [ ] Executor tests with mocked HTTP — verified
- [ ] Namespace helper created — verified

**Step 4: Auto-Review**
Run `/conductor-review`. If review finds:
- No real HTTP call → reject, make it call GitHub API
- TODO comments → remove them
- Stub implementation → replace with real code

**Step 5: Proceed**
All checks pass → mark Phase 1 complete, advance to Phase 2.

### Phase 2: Diagnostics Command

**Step 1: Analyze**
- Read `IOrganizationServiceRepository` to understand Dataverse connection pattern
- Read `ITokenProvider` to understand auth patterns
- Check how `WhoAmIRequest` is used elsewhere
- Read `System.Runtime.InteropServices.RuntimeInformation` for .NET version

**Step 2: Implement**

Create **`Commands/Diag/DiagCommand.cs`**:
```csharp
namespace Greg.Xrm.Command.Commands.Diag
{
    [Command("diag", HelpText = "Run diagnostics to troubleshoot PACX setup.")]
    public class DiagCommand
    {
        [Option("verbose", "v", Order = 1, HelpText = "Show detailed diagnostic information.")]
        public bool Verbose { get; set; }
    }
}
```

Create **`Commands/Diag/DiagCommandExecutor.cs`**:
- Inject `IOutput`, `IOrganizationServiceRepository`
- Check 1: .NET runtime version (`RuntimeInformation.FrameworkDescription`)
- Check 2: PACX assembly version (`Assembly.GetEntryAssembly()?.GetName()?.Version`)
- Check 3: OS platform (`RuntimeInformation.OSDescription`)
- Check 4: Dataverse connectivity — call `WhoAmIRequest`, show `UserId`, `OrganizationId`
- Check 5: Authentication status — check token cache or env vars
- Check 6: Environment variables — `MSAL_CLIENT_ID`, `PACX_INTEGRATION_ENV_URL` (redacted)
- Render results in a table: Pas s / Fail / Warning per check
- If `--verbose`, show detailed error messages
- Tests: mock organization service, mock environment variables

Create **`Commands/Diag/Help.cs`**:
```csharp
namespace Greg.Xrm.Command.Commands.Diag
{
    public class Help : NamespaceHelperBase
    {
        public Help() : base("Diagnose PACX setup and connectivity", "diag") { }
    }
}
```

**Step 3: Anti-Stub Verification**
- [ ] Real WhoAmI call to Dataverse (not stubbed) — verified
- [ ] Real version detection from assembly — verified
- [ ] Real OS/platform detection — verified
- [ ] Environment variable check with value redaction — verified
- [ ] Table output format — verified
- [ ] Tests with mocked dependencies — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 3.

### Phase 3: Silent/Quiet Mode

**Step 1: Analyze**
- Read `IOutput` interface — understand the output abstraction
- Read `OutputToConsole` and `OutputToMemory` implementations
- Read `CommandRunnerBase` — understand where output is created
- Read `Program.cs` — understand DI setup for output

**Step 2: Implement**

Create **`Services/Output/SilentOutput.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Output
{
    public class SilentOutput : IOutput
    {
        // Suppresses all output — silent mode
        // Used when --silent is passed
        public void Write(string text) { }
        public void Write(string text, ConsoleColor color) { }
        public void WriteLine(string text) { }
        public void WriteLine(string text, ConsoleColor color) { }
        public void WriteLine() { }
        // ... other IOutput methods all no-op
    }
}
```

Create **`Services/Output/QuietOutput.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Output
{
    public class QuietOutput : IOutput
    {
        // Only shows warnings and errors
        // Used when --quiet or --silent is passed
        // Delegates Write/Writeline to wrapped output but filters based on color
    }
}
```

Modify **`CommandRunnerBase.cs`**:
- Check for `--silent` or `--quiet` global flags after arg parsing
- If silent: replace `IOutput` with `SilentOutput` in the scope
- If quiet: replace with `QuietOutput`

Modify **global option handling** (likely in `CommandParser.cs` or `Program.cs`):
- Add `--silent` and `--quiet` to the list of recognized global flags
- These should be stripped before command parsing (like `--verbose`, `--help`)

**Step 3: Anti-Stub Verification**
- [ ] `SilentOutput` suppresses all output — verified by reading
- [ ] `QuietOutput` suppresses info, shows warnings/errors — verified by reading
- [ ] Global flags `--silent` and `--quiet` recognized — verified
- [ ] Output is swapped before command execution — verified
- [ ] Tests: silent mode produces no output — verified by test
- [ ] Tests: quiet mode only shows warnings — verified by test

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 4.

### Phase 4: Progress Bars

**Step 1: Analyze**
- Identify long-running operations: `env create --wait`, `env backup`, `tabular deploy`, `forms responses export`
- Read console cursor control patterns for progress display
- Check for existing progress/spinner code

**Step 2: Implement**

Create **`Services/Output/IProgressIndicator.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Output
{
    public interface IProgressIndicator : IDisposable
    {
        void Report(double progress, string message);
        void SetMessage(string message);
        void Complete(string? completionMessage = null);
        void Fail(string? errorMessage = null);
    }
}
```

Create **`Services/Output/ConsoleProgressBar.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Output
{
    public class ConsoleProgressBar : IProgressIndicator
    {
        private readonly int totalWidth = 40;
        private readonly bool showSpinner;
        private int currentProgress;
        private readonly Timer? spinnerTimer;
        private static readonly string[] spinnerChars = ['|', '/', '-', '\\'];

        public ConsoleProgressBar(bool showSpinner = true) { /* ... */ }
        public void Report(double progress, string message) { /* render bar */ }
        public void SetMessage(string message) { /* update message */ }
        public void Complete(string? msg) { /* render 100% */ }
        public void Fail(string? msg) { /* render failed */ }
        public void Dispose() { /* cleanup */ }
    }
}
```

Modify long-running executors:
- `EnvCreateCommandExecutor` — wrap BAP API call in progress reporting
- `EnvBackupCommandExecutor` — report while polling async job
- `TabularDeployCommandExecutor` — report while deploying to Power BI
- Executors should auto-disable progress when `--silent` or `--output json` is active

**Step 3: Anti-Stub Verification**
- [ ] Progress bar renders in real-time (not buffered) — verified by reading
- [ ] Spinner animates for unknown-duration operations — verified
- [ ] Auto-disables for non-interactive output — verified
- [ ] Properly disposed on complete/fail — verified
- [ ] Tests: progress reported correctly — verified by test
- [ ] Tests: auto-disable works — verified by test

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 5.

### Phase 5: YAML Output

**Step 1: Analyze**
- Check if `YamlDotNet` NuGet is already referenced
- Read existing `--format json` implementations for pattern
- Understand when output is produced (in executors vs. in base classes)

**Step 2: Implement**

Create **`Services/Output/YamlSerializer.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Output
{
    public static class YamlSerializer
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                .WithTypeInspector(inner => new YamlDotNet.Serialization.TypeInspectors.NamingConventionTypeInspector(
                    inner, new YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention()))
                .Build();
            return serializer.Serialize(obj);
        }
    }
}
```

Add `YamlDotNet` to the project:
```xml
<PackageReference Include="YamlDotNet" Version="16.0.0" />
```

Create a shared output helper (extend `CommandResult` or add extension method):
```csharp
namespace Greg.Xrm.Command
{
    public static class CommandOutputExtensions
    {
        public static string ToOutputString(this CommandResult result, string format)
        {
            return format.ToLowerInvariant() switch
            {
                "json" => System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }),
                "yaml" => Services.Output.YamlSerializer.Serialize(result),
                _ => result.ToString() ?? ""
            };
        }
    }
}
```

**Step 3: Anti-Stub Verification**
- [ ] `YamlDotNet` referenced in csproj — verified by reading
- [ ] `--output yaml` recognized by commands — verified
- [ ] YAML output is valid YAML — verified by reading output
- [ ] Tests with JSON and YAML output comparison — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 6.

### Phase 6: Config Auto-Discovery

**Step 1: Analyze**
- Read existing `Config` commands if any
- Understand how `IStorage` works (file-based storage in LocalApplicationData)
- Read how options are currently processed

**Step 2: Implement**

Define **`Services/Config/PacxConfig.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Config
{
    public sealed class PacxConfig
    {
        public string? DefaultTenantId { get; set; }
        public string? DefaultEnvironmentUrl { get; set; }
        public string? OutputFormat { get; set; } = "table";
        public Dictionary<string, string> Connections { get; set; } = [];
        public Dictionary<string, object> Options { get; set; } = [];
    }
}
```

Create **`Services/Config/ConfigLoader.cs`**:
```csharp
namespace Greg.Xrm.Command.Services.Config
{
    public class ConfigLoader
    {
        private const string ConfigFileName = "pacx.config.json";

        public PacxConfig? Load()
        {
            // Search order:
            // 1. Current directory
            // 2. Parent directories (up to root)
            // 3. ~/.pacx/config.json
            // Return first found, or null if none
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, ConfigFileName);
                if (File.Exists(candidate))
                    return LoadFrom(candidate);
                dir = dir.Parent;
            }

            // Check user home directory
            var homeConfig = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".pacx", ConfigFileName);
            if (File.Exists(homeConfig))
                return LoadFrom(homeConfig);

            return null;
        }

        private static PacxConfig? LoadFrom(string path) { /* JSON deserialize */ }
    }
}
```

Create **`Commands/Config/ConfigShowCommand.cs`** + executor:
- Shows discovered config file path
- Shows current config values (with secrets redacted)
- Validates config format

Create **`Commands/Config/Help.cs`**:
```csharp
namespace Greg.Xrm.Command.Commands.Config
{
    public class Help : NamespaceHelperBase
    {
        public Help() : base("Manage PACX configuration", "config") { }
    }
}
```

Modify **`CommandRunnerBase.cs`** (or the bootstrapper):
- Load config on startup
- For any option not specified on CLI, fall back to config value
- CLI args always win over config values

**Step 3: Anti-Stub Verification**
- [ ] Config file found by walking up directory tree — verified by test
- [ ] Config merged with CLI args — verified by test
- [ ] CLI args override config — verified by test
- [ ] Secrets redacted in config display — verified
- [ ] Config show command works — verified
- [ ] Tests for: no config, partial config, full config, config override — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Track complete.

## Rollback Plan
If any phase fails review:
1. Revert the last change
2. Fix the specific issue
3. Re-verify
4. Re-run `/conductor-review`
5. Only proceed on pass
