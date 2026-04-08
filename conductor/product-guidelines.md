# Tone and Voice
Professional, concise, and helpful. Use technical terminology appropriate for Power Platform developers.

# Visual Guidelines (CLI)
- Use standard terminal colors for status (e.g., green for success, red for errors).
- Implement consistent indentation and spacing for structured output.
- Support interactive elements (menus, search) with clear navigation prompts.

# User Experience (UX)
- Prioritize command discoverability.
- Provide comprehensive inline help for all commands and options.
- Ensure commands are idempotent where possible to support automation.

# Documentation Style
Clear, example-driven documentation using Markdown. All commands should have usage examples.

# Core Contribution Rules
- **No Console Output:** All CLI writing MUST go through the injected `IOutput` interface to support formatting, colors, and structured outputs via Spectre.Console. Never use `Console.WriteLine`.
- **Command Separation:** Every CLI command must be split into a `[Command]` class (for argument parsing and validation) and an `ICommandExecutor<T>` class (for business logic).
- **Return Values:** Always return a structured `CommandResult` containing the operation's outcome data.
