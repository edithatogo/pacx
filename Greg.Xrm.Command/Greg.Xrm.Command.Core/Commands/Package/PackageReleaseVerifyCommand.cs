using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Package
{
	[Command("package", "release", "verify", HelpText = "Verify a staged PACX release folder.")]
	public class PackageReleaseVerifyCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the staged release folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;
	}
}
