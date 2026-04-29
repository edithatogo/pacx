using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Completions
{
	[Command("completions", HelpText = "Print a shell completion script for pacx.")]
	public class CompletionsCommand
	{
		[Option("shell", "s", Order = 1, HelpText = "Shell to generate completions for: pwsh, bash, zsh, fish.", DefaultValue = CompletionShell.Pwsh)]
		public CompletionShell Shell { get; set; } = CompletionShell.Pwsh;
	}

	[Command("completions", "pwsh", HelpText = "Print a PowerShell completion script for pacx.")]
	public class CompletionsPwshCommand
	{
	}

	[Command("completions", "bash", HelpText = "Print a Bash completion script for pacx.")]
	public class CompletionsBashCommand
	{
	}

	[Command("completions", "zsh", HelpText = "Print a Zsh completion script for pacx.")]
	public class CompletionsZshCommand
	{
	}

	[Command("completions", "fish", HelpText = "Print a Fish completion script for pacx.")]
	public class CompletionsFishCommand
	{
	}
}
