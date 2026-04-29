using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.PowerApps
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base(
			"Commands to manage Power Apps (list, get, remove, export, consent, owner, permissions)",
			"pa")
		{
		}
	}
}
