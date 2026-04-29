namespace Greg.Xrm.Command.Integration
{
	public abstract class IntegrationTestBase
	{
		protected static string IntegrationEnvironmentUrl
		{
			get
			{
				var value = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");
				if (string.IsNullOrWhiteSpace(value))
				{
					Assert.Inconclusive("Set PACX_INTEGRATION_ENV_URL to run Dataverse integration tests.");
				}

				return value;
			}
		}
	}
}
