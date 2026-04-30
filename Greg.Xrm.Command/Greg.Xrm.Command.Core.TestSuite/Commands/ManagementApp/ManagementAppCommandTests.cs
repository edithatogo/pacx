using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;

namespace Greg.Xrm.Command.Commands.ManagementApp
{
	[TestClass]
	public class ManagementAppCommandTests
	{
		[TestMethod]
		public async Task ManagementAppList_ShouldCallListManagementApps()
		{
			var client = new RecordingAdminClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new ManagementAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new ManagementAppListCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
		}

		[TestMethod]
		public async Task ManagementAppList_Error_ShouldReturnFailResult()
		{
			var client = new ThrowingAdminClient(new InvalidOperationException("API error"));
			var output = new OutputToMemory();
			var executor = new ManagementAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new ManagementAppListCommand(), CancellationToken.None);

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
