using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;

namespace Greg.Xrm.Command.Commands.Tool
{
	[Command("tool", "uninstall", HelpText = "Uninstalls a PACX plugin.")]
	[Alias("tool", "remove")]
	[Alias("tool", "delete")]
	[Alias("uninstall", "tool")]
	[Alias("delete", "tool")]
	[Alias("remove", "tool")]
	public class UninstallCommand : ICanProvideUsageExample
	{
		[Option("name", "n", HelpText = "The unique name of the NuGet package containing the plugin to uninstall.")]
		[Required]
		public string Name { get; set; } = string.Empty;

		public void WriteUsageExamples(MarkdownWriter writer)
		{
			writer.WriteParagraph("You can uninstall a plugin by its unique package name.");
			writer.WriteCodeBlock("pacx tool uninstall -n MyPlugin", "Powershell");
		}
	}
}
