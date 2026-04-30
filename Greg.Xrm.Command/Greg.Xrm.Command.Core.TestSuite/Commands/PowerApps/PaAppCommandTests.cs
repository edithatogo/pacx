using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerApps;

namespace Greg.Xrm.Command.Commands.PowerApps
{
	[TestClass]
	public class PaAppCommandTests
	{
		private const string TestAppName = "app-123";
		private const string TestEnvironment = "test-env";
		private const string TestPrincipalId = "user-456";

		[TestMethod]
		public async Task PaAppList_ShouldCallListAppsWithCorrectParameters()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("""{"value":[{"name":"app-1"}]}"""));
			var output = new OutputToMemory();
			var executor = new PaAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppListCommand
			{
				EnvironmentName = TestEnvironment,
				AsAdmin = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListApps", client.LastCall);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.IsTrue(client.LastAsAdmin);
			StringAssert.Contains(output.ToString(), "\"name\"");
		}

		[TestMethod]
		public async Task PaAppList_WithoutAdmin_ShouldNotPassEnvironment()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new PaAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppListCommand
			{
				AsAdmin = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListApps", client.LastCall);
			Assert.IsFalse(client.LastAsAdmin);
			Assert.IsNull(client.LastEnvironment);
		}

		[TestMethod]
		public async Task PaAppGet_ShouldCallGetAppWithCorrectParameters()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("""{"name":"app-1","properties":{"displayName":"Test App"}}"""));
			var output = new OutputToMemory();
			var executor = new PaAppGetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppGetCommand
			{
				AppName = TestAppName,
				EnvironmentName = TestEnvironment,
				AsAdmin = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GetApp", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.IsTrue(client.LastAsAdmin);
			StringAssert.Contains(output.ToString(), "Test App");
		}

		[TestMethod]
		public async Task PaAppGet_WithoutAdmin_ShouldNotPassEnvironment()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("""{"name":"app-1"}"""));
			var output = new OutputToMemory();
			var executor = new PaAppGetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppGetCommand
			{
				AppName = TestAppName,
				AsAdmin = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("GetApp", client.LastCall);
			Assert.IsFalse(client.LastAsAdmin);
		}

		[TestMethod]
		public async Task PaAppRemove_WithConfirm_ShouldCallDeleteApp()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppRemoveCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppRemoveCommand
			{
				AppName = TestAppName,
				EnvironmentName = TestEnvironment,
				AsAdmin = true,
				Confirm = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("DeleteApp", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.IsTrue(client.LastAsAdmin);
			StringAssert.Contains(output.ToString(), "deleted successfully");
		}

		[TestMethod]
		public async Task PaAppRemove_WithoutConfirm_ShouldPromptAndNotDelete()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppRemoveCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppRemoveCommand
			{
				AppName = TestAppName,
				Confirm = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.IsNull(client.LastCall);
			StringAssert.Contains(output.ToString(), "Are you sure");
			StringAssert.Contains(output.ToString(), "Deletion cancelled");
		}

		[TestMethod]
		public async Task PaAppExport_ShouldCallExportApp()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppExportCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppExportCommand
			{
				AppName = TestAppName,
				EnvironmentName = TestEnvironment
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ExportApp", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			StringAssert.Contains(output.ToString(), "exported to");
		}

		[TestMethod]
		public async Task PaAppConsentSet_ShouldCallSetAppConsent()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppConsentSetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppConsentSetCommand
			{
				AppName = TestAppName,
				EnvironmentName = TestEnvironment,
				BypassConsent = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("SetAppConsent", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			Assert.IsTrue(client.LastBypassConsent);
			StringAssert.Contains(output.ToString(), "Consent bypass");
		}

		[TestMethod]
		public async Task PaAppOwnerSet_ShouldCallSetAppOwner()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppOwnerSetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppOwnerSetCommand
			{
				AppName = TestAppName,
				EnvironmentName = TestEnvironment,
				NewOwnerId = TestPrincipalId,
				RoleForOldOwner = "CanView"
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("SetAppOwner", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestEnvironment, client.LastEnvironment);
			StringAssert.Contains(output.ToString(), "transferred");
		}

		[TestMethod]
		public async Task PaAppPermissionList_ShouldCallListPermissions()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new PaAppPermissionListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppPermissionListCommand
			{
				AppName = TestAppName,
				AsAdmin = true
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("ListAppPermissions", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
		}

		[TestMethod]
		public async Task PaAppPermissionAdd_ShouldCallAddPermission()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppPermissionAddCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppPermissionAddCommand
			{
				AppName = TestAppName,
				PrincipalId = TestPrincipalId,
				PrincipalType = "User",
				RoleName = "CanEdit",
				AsAdmin = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("AddAppPermission", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestPrincipalId, client.LastPrincipalId);
			StringAssert.Contains(output.ToString(), "Permission added");
		}

		[TestMethod]
		public async Task PaAppPermissionRemove_ShouldCallRemovePermission()
		{
			var client = new RecordingPowerAppsClient(JsonDocument.Parse("{}"));
			var output = new OutputToMemory();
			var executor = new PaAppPermissionRemoveCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppPermissionRemoveCommand
			{
				AppName = TestAppName,
				PrincipalId = TestPrincipalId,
				AsAdmin = false
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			Assert.AreEqual("RemoveAppPermission", client.LastCall);
			Assert.AreEqual(TestAppName, client.LastAppName);
			Assert.AreEqual(TestPrincipalId, client.LastPrincipalId);
			StringAssert.Contains(output.ToString(), "Permission removed");
		}

		[TestMethod]
		public async Task PowerAppsClientError_ShouldReturnFailResult()
		{
			var client = new ThrowingPowerAppsClient(new InvalidOperationException("API error"));
			var output = new OutputToMemory();
			var executor = new PaAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new PaAppListCommand(), CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "API error");
		}

		private sealed class RecordingPowerAppsClient(JsonDocument response) : IPowerAppsClient
		{
			public string? LastCall { get; private set; }
			public string? LastAppName { get; private set; }
			public string? LastEnvironment { get; private set; }
			public bool LastAsAdmin { get; private set; }
			public string? LastPrincipalId { get; private set; }
			public bool LastBypassConsent { get; private set; }

			public Task<JsonDocument> ListAppsAsync(string? environmentName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "ListApps";
				LastEnvironment = environmentName;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "GetApp";
				LastAppName = appName;
				LastEnvironment = environmentName;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task DeleteAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "DeleteApp";
				LastAppName = appName;
				LastEnvironment = environmentName;
				LastAsAdmin = asAdmin;
				return Task.CompletedTask;
			}

			public Task<(byte[] content, string fileName)> ExportAppAsync(string appName, string environmentName, CancellationToken cancellationToken)
			{
				LastCall = "ExportApp";
				LastAppName = appName;
				LastEnvironment = environmentName;
				return Task.FromResult<(byte[], string)>((Array.Empty<byte>(), $"{appName}.zip"));
			}

			public Task SetAppConsentAsync(string appName, string environmentName, bool bypass, CancellationToken cancellationToken)
			{
				LastCall = "SetAppConsent";
				LastAppName = appName;
				LastEnvironment = environmentName;
				LastBypassConsent = bypass;
				return Task.CompletedTask;
			}

			public Task SetAppOwnerAsync(string appName, string environmentName, string newOwnerId, string? roleForOldOwner, CancellationToken cancellationToken)
			{
				LastCall = "SetAppOwner";
				LastAppName = appName;
				LastEnvironment = environmentName;
				return Task.CompletedTask;
			}

			public Task<JsonDocument> ListAppPermissionsAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
			{
				LastCall = "ListAppPermissions";
				LastAppName = appName;
				LastEnvironment = environmentName;
				LastAsAdmin = asAdmin;
				return Task.FromResult(Clone(response));
			}

			public Task AddAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, string principalType, string roleName, CancellationToken cancellationToken)
			{
				LastCall = "AddAppPermission";
				LastAppName = appName;
				LastEnvironment = environmentName;
				LastAsAdmin = asAdmin;
				LastPrincipalId = principalId;
				return Task.CompletedTask;
			}

			public Task RemoveAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, CancellationToken cancellationToken)
			{
				LastCall = "RemoveAppPermission";
				LastAppName = appName;
				LastEnvironment = environmentName;
				LastAsAdmin = asAdmin;
				LastPrincipalId = principalId;
				return Task.CompletedTask;
			}

			private static JsonDocument Clone(JsonDocument document)
			{
				return JsonDocument.Parse(document.RootElement.GetRawText());
			}
		}

		private sealed class ThrowingPowerAppsClient(Exception exception) : IPowerAppsClient
		{
			public Task<JsonDocument> ListAppsAsync(string? environmentName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> GetAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task DeleteAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task<(byte[] content, string fileName)> ExportAppAsync(string appName, string environmentName, CancellationToken cancellationToken)
				=> throw exception;

			public Task SetAppConsentAsync(string appName, string environmentName, bool bypass, CancellationToken cancellationToken)
				=> throw exception;

			public Task SetAppOwnerAsync(string appName, string environmentName, string newOwnerId, string? roleForOldOwner, CancellationToken cancellationToken)
				=> throw exception;

			public Task<JsonDocument> ListAppPermissionsAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken)
				=> throw exception;

			public Task AddAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, string principalType, string roleName, CancellationToken cancellationToken)
				=> throw exception;

			public Task RemoveAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, CancellationToken cancellationToken)
				=> throw exception;
		}
	}
}
