using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Flows
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base("Commands to work with Power Automate cloud flows (list, get, enable, disable, remove, export)", "flow", "flows")
		{
		}
	}
}
