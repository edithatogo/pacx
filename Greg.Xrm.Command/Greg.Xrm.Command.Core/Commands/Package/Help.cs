using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Package
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base("PACX-native package commands for authoring, validation, build, deploy, and release flows. Kinds are bundle, solution, and data.", "package")
		{
		}
	}
}
