using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ManagementApp
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base(
			"Commands to manage Power Platform management applications (list, add)",
			"managementapp")
		{
		}
	}
}
