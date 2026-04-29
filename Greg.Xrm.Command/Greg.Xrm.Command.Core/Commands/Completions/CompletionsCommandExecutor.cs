using System.Text;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Completions
{
	public class CompletionsCommandExecutor(ICommandRegistry registry, IOutput output) : ICommandExecutor<CompletionsCommand>
	{
		public Task<CommandResult> ExecuteAsync(CompletionsCommand command, CancellationToken cancellationToken)
		{
			return WriteCompletionsAsync(command.Shell, cancellationToken);
		}

		protected Task<CommandResult> WriteCompletionsAsync(CompletionShell shell, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var script = shell switch
			{
				CompletionShell.Pwsh => BuildPowerShellScript(registry.Commands),
				CompletionShell.Bash => BuildBashScript(registry.Commands),
				CompletionShell.Zsh => BuildZshScript(registry.Commands),
				CompletionShell.Fish => BuildFishScript(registry.Commands),
				_ => throw new ArgumentOutOfRangeException(nameof(shell), "Unsupported completion shell."),
			};

			output.WriteLine(script);
			return Task.FromResult(CommandResult.Success());
		}

		private static string BuildPowerShellScript(IReadOnlyList<CommandDefinition> commands)
		{
			var words = BuildWords(commands);
			return $@"Register-ArgumentCompleter -Native -CommandName pacx -ScriptBlock {{
	param($wordToComplete, $commandAst, $cursorPosition)
	$words = @({string.Join(", ", words.Select(w => "'" + EscapePowerShell(w) + "'"))})
	$words | Where-Object {{ $_ -like ""$wordToComplete*"" }} | ForEach-Object {{
		[System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
	}}
}}";
		}

		private static string BuildBashScript(IReadOnlyList<CommandDefinition> commands)
		{
			var words = string.Join(" ", BuildWords(commands).Select(EscapeShellWord));
			return $@"_pacx_complete()
{{
	local cur
	cur=""${{COMP_WORDS[COMP_CWORD]}}""
	COMPREPLY=( $(compgen -W ""{words}"" -- ""$cur"") )
}}
complete -F _pacx_complete pacx";
		}

		private static string BuildZshScript(IReadOnlyList<CommandDefinition> commands)
		{
			var words = string.Join(" ", BuildWords(commands).Select(EscapeShellWord));
			return $@"#compdef pacx
_pacx() {{
	local -a words
	words=({words})
	_describe 'pacx commands and options' words
}}
compdef _pacx pacx";
		}

		private static string BuildFishScript(IReadOnlyList<CommandDefinition> commands)
		{
			var sb = new StringBuilder();
			foreach (var word in BuildWords(commands))
			{
				sb.Append("complete -c pacx -f -a '")
					.Append(EscapeFish(word))
					.AppendLine("'");
			}

			return sb.ToString();
		}

		private static IReadOnlyList<string> BuildWords(IReadOnlyList<CommandDefinition> commands)
		{
			var words = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var option in GlobalCommandOptions.Names)
			{
				words.Add(option);
			}

			foreach (var command in commands.Where(c => !c.Hidden))
			{
				foreach (var verb in command.Verbs)
				{
					words.Add(verb);
				}

				words.Add(command.ExpandedVerbs);

				foreach (var option in command.Options)
				{
					words.Add("--" + option.Option.LongName);
					if (!string.IsNullOrWhiteSpace(option.Option.ShortName))
					{
						words.Add("-" + option.Option.ShortName);
					}
				}
			}

			return words.ToList();
		}

		private static string EscapePowerShell(string value) => value.Replace("'", "''", StringComparison.Ordinal);

		private static string EscapeShellWord(string value) => value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);

		private static string EscapeFish(string value) => value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("'", "\\'", StringComparison.Ordinal);
	}

	public class CompletionsPwshCommandExecutor(ICommandRegistry registry, IOutput output) : CompletionsCommandExecutor(registry, output), ICommandExecutor<CompletionsPwshCommand>
	{
		public Task<CommandResult> ExecuteAsync(CompletionsPwshCommand command, CancellationToken cancellationToken) => WriteCompletionsAsync(CompletionShell.Pwsh, cancellationToken);
	}

	public class CompletionsBashCommandExecutor(ICommandRegistry registry, IOutput output) : CompletionsCommandExecutor(registry, output), ICommandExecutor<CompletionsBashCommand>
	{
		public Task<CommandResult> ExecuteAsync(CompletionsBashCommand command, CancellationToken cancellationToken) => WriteCompletionsAsync(CompletionShell.Bash, cancellationToken);
	}

	public class CompletionsZshCommandExecutor(ICommandRegistry registry, IOutput output) : CompletionsCommandExecutor(registry, output), ICommandExecutor<CompletionsZshCommand>
	{
		public Task<CommandResult> ExecuteAsync(CompletionsZshCommand command, CancellationToken cancellationToken) => WriteCompletionsAsync(CompletionShell.Zsh, cancellationToken);
	}

	public class CompletionsFishCommandExecutor(ICommandRegistry registry, IOutput output) : CompletionsCommandExecutor(registry, output), ICommandExecutor<CompletionsFishCommand>
	{
		public Task<CommandResult> ExecuteAsync(CompletionsFishCommand command, CancellationToken cancellationToken) => WriteCompletionsAsync(CompletionShell.Fish, cancellationToken);
	}
}
