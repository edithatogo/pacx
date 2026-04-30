using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;

namespace Greg.Xrm.Command.Commands.TenantSettings
{
	[TestClass]
	public class TenantSettingsCommandTests
	{
		[TestMethod]
		public async Task TenantSettingsList_ShouldCallGetTenantSettings()
		{
			var client = new RecordingAdminClient(JsonDocument.Parse("""{"walkMeOptOut":false,"disableEnvironmentCreationByNonAdminUsers":false}"""));
			var output = new OutputToMemory();
			var executor = new TenantSettingsListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new TenantSettingsListCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "walkMeOptOut");
		}

		[TestMethod]
		public async Task TenantSettingsList_Error_ShouldReturnFailResult()
		{
			var client = new ThrowingAdminClient(new InvalidOperationException("API error"));
			var output = new OutputToMemory();
			var executor = new TenantSettingsListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new TenantSettingsListCommand(), CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "API error");
		}

		private sealed class RecordingAdminClient(JsonDocument response) : IPowerPlatformAdminClient
		{
			public Task<JsonDocument> ListManagementAppsAsync(CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse(response.RootElement.GetRawText()));

			public Task<JsonDocument> GetTenantSettingsAsync(CancellationToken cancellationToken)
				=> Task.FromResult(JsonDocument.Parse(response.RootElement.GetRawText()));

			public Task SetTenantSettingsAsync(object settings, CancellationToken cancellationToken)
				=> Task.CompletedTask;
		}

		private sealed class ThrowingAdminClient(Exception exception) : IPowerPlatformAdminClient
		{
			public Task<JsonDocument> ListManagementAppsAsync(CancellationToken cancellationToken) => throw exception;
			public Task<JsonDocument> GetTenantSettingsAsync(CancellationToken cancellationToken) => throw exception;
			public Task SetTenantSettingsAsync(object settings, CancellationToken cancellationToken) => throw exception;
		}
	}
}
