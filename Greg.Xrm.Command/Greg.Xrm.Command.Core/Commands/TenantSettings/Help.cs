using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.TenantSettings
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base(
			"Commands to manage Power Platform tenant settings (list)",
			"tenant", "settings")
		{
		}
	}
}
