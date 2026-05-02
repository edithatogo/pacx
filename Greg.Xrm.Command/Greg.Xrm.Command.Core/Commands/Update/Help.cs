using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Update
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base("Manages PACX updates", "self-update", "update")
		{
		}
	}
}
