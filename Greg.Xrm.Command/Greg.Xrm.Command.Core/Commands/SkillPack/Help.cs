using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	public class Help : NamespaceHelperBase
	{
		public Help() : base("Allows listing and installing PACX skill packs", "skill", "pack")
		{
		}
	}
}
