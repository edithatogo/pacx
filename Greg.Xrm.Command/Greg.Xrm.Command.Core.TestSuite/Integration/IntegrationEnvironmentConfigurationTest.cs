namespace Greg.Xrm.Command.Integration
{
	[TestClass]
	[TestCategory("Integration")]
	public class IntegrationEnvironmentConfigurationTest : IntegrationTestBase
	{
		[TestMethod]
		public void IntegrationEnvironmentUrl_ShouldBeConfigured()
		{
			StringAssert.StartsWith(IntegrationEnvironmentUrl, "https://");
		}
	}
}
