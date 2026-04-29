using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Gateway
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base(
			"Commands to manage on-premises data gateways (list, get)",
			"gateway")
		{
		}
	}
}
