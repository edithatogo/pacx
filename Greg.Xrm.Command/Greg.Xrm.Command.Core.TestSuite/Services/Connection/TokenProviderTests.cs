using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.Connection;

[TestClass]
public class TokenProviderTests
{
	[TestMethod]
	public void BuildScopes_UsesResourceDefaultScopeWithoutDoubleSlash()
	{
		var scopes = TokenProvider.BuildScopes("https://management.azure.com/");

		CollectionAssert.AreEqual(new[] { "https://management.azure.com/.default" }, scopes);
	}

	[TestMethod]
	public void BuildScopes_UsesManagementResourceAsOpaqueAuthorityInput()
	{
		var scopes = TokenProvider.BuildScopes("https://management.azure.com");

		CollectionAssert.AreEqual(new[] { "https://management.azure.com/.default" }, scopes);
	}
}
