using System.Diagnostics;
using System.Drawing.Text;
using System.ServiceModel;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;
using Microsoft.Xrm.Sdk;

namespace Greg.Xrm.Command.Services.Connection
{
	public class OrganizationServiceRepository(
		IOutput output,
		ISettingsRepository settings,
		IPacxProjectRepository pacxProjectRepository
		) : IOrganizationServiceRepository
	{
		private readonly ISettingsRepository settings = settings ?? throw new ArgumentNullException(nameof(settings));
		private readonly IPacxProjectRepository pacxProjectRepository = pacxProjectRepository ?? throw new ArgumentNullException(nameof(pacxProjectRepository));

		public string GetTokenCachePath()
		{
			return TokenCache.GetTokenCachePath();
		}


		private static byte[] GetAesKey() => Convert.FromBase64String(Properties.Resources.AesKey);

		private static byte[] GetAesIV() => Convert.FromBase64String(Properties.Resources.AesIV);



		private ConnectionSetting? cache = null;






		private async Task<ConnectionSetting?> GetConnectionSettingAsync(CancellationToken cancellationToken = default)
		{
			if (this.cache != null) return this.cache;

			var connectionSettings = await this.settings.GetAsync<ConnectionSetting>("connections", cancellationToken).ConfigureAwait(false);

			if (connectionSettings != null && !connectionSettings.IsSecured.GetValueOrDefault())
			{
				connectionSettings.SecureSettings();
				await this.settings.SetAsync("connections", connectionSettings, cancellationToken).ConfigureAwait(false);
			}
			cache = connectionSettings;
			return connectionSettings;
		}





		/// <summary>
		/// Given the name of an authentication profile, it returns the environment name.
		/// </summary>
		/// <param name="connectionName">The name of the authentication profile.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<string?> GetEnvironmentFromConnectioStringAsync(string connectionName, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken).ConfigureAwait(false);
			if (connectionStrings == null) return null;

			if (!connectionStrings.TryGetConnectionString(connectionName, out var connectionString))
				return null;

			if (string.IsNullOrWhiteSpace(connectionString))
				return null;

			var parts = connectionString.Split(';').ToList();

			var environmentToken = parts.Find(p => p.StartsWith("Url", StringComparison.OrdinalIgnoreCase))
									?? parts.Find(p => p.StartsWith("ServiceUri", StringComparison.OrdinalIgnoreCase))
									?? parts.Find(p => p.StartsWith("Service Uri", StringComparison.OrdinalIgnoreCase))
									?? parts.Find(p => p.StartsWith("Server", StringComparison.OrdinalIgnoreCase));

			if (string.IsNullOrWhiteSpace(environmentToken))
				return null;

			var startIndex = environmentToken.IndexOf('=') + 1;
			if (startIndex < 0)
				return null;

			var value = environmentToken[startIndex..]?.Trim();
			return value;
		}



		public async Task<ConnectionSetting> GetAllConnectionDefinitionsAsync(CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken).ConfigureAwait(false);
			return connectionStrings ?? new ConnectionSetting() { IsSecured = true };
		}





		public async Task<IOrganizationServiceAsync2> GetCurrentConnectionAsync(CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken)
				?? throw new CommandException(CommandException.ConnectionNotSet, "Dataverse connection has not been set yet.");

			string? connectionString;
			bool found;
			string? connectionName;
			var project = await this.pacxProjectRepository.GetCurrentProjectAsync().ConfigureAwait(false);
			if (project != null && !project.IsSuspended)
			{
				connectionName = project.AuthProfileName;
				found = connectionStrings.TryGetConnectionString(project.AuthProfileName, out connectionString);
				if (!found)
					throw new CommandException(CommandException.ConnectionNotSet, $"Unable to find an authentication profile called {project.AuthProfileName}.");
			}
			else
			{
				connectionName = connectionStrings.CurrentConnectionStringKey;
				found = connectionStrings.TryGetCurrentConnectionString(out connectionString);
				if (!found)
					throw new CommandException(CommandException.ConnectionNotSet, "Dataverse connection has not been set yet.");
			}

			return await this.CreateServiceClientAsync(connectionName!, connectionString!, cancellationToken).ConfigureAwait(false);
		}





		public async Task<IOrganizationServiceAsync2> GetConnectionByNameAsync(string connectionName, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken)
				?? throw new CommandException(CommandException.ConnectionNotSet, "Dataverse connection has not been set yet.");

			if (!connectionStrings.TryGetConnectionString(connectionName, out var connectionString))
				throw new CommandException(CommandException.ConnectionNotSet, "Dataverse connection has not been set yet.");

			return await this.CreateServiceClientAsync(connectionName, connectionString!, cancellationToken).ConfigureAwait(false);
		}




		public async Task<string?> GetConnectionStringAsync(string? connectionName = null, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken).ConfigureAwait(false);
			if (connectionStrings == null) return null;

			if (string.IsNullOrEmpty(connectionName))
			{
				var project = await this.pacxProjectRepository.GetCurrentProjectAsync().ConfigureAwait(false);
				if (project != null && !project.IsSuspended)
				{
					connectionName = project.AuthProfileName;
				}
				else
				{
					connectionName = connectionStrings.CurrentConnectionStringKey;
				}
			}

			if (string.IsNullOrEmpty(connectionName)) return null;

			if (!connectionStrings.TryGetConnectionString(connectionName, out var connectionString))
				return null;

			return connectionString;
		}



		public async Task SetConnectionAsync(string name, string connectionString, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name), "The connection name is empty.");
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException(nameof(connectionString), "The connection string is empty.");

			try
			{
				var crm = await CreateServiceClientAsync(name, connectionString, cancellationToken).ConfigureAwait(false);
				await crm.ExecuteAsync(new WhoAmIRequest(), cancellationToken).ConfigureAwait(false);


				var connectionStrings = await GetConnectionSettingAsync(cancellationToken).ConfigureAwait(false);
				connectionStrings ??= new ConnectionSetting() { IsSecured = true };

				connectionStrings.UpsertConnectionString(name, connectionString);
				connectionStrings.CurrentConnectionStringKey = name;

				await this.settings.SetAsync("connections", connectionStrings, cancellationToken).ConfigureAwait(false);
			}
			catch (FaultException<OrganizationServiceFault> ex)
			{
				throw new CommandException(CommandException.ConnectionInvalid, "Dataverse connection has not been set yet.", ex);
			}
		}

		public async Task RenameConnectionAsync(string oldName, string newName, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken).ConfigureAwait(false)
				?? throw new CommandException(CommandException.ConnectionNotSet, "No connections set, nothing to update");
			if (!connectionStrings.Exists(oldName))
			{
				throw new CommandException(CommandException.CommandInvalidArgumentValue, "Invalid connection name: " + oldName);
			}

			if (connectionStrings.Exists(newName))
			{
				throw new CommandException(CommandException.CommandInvalidArgumentValue, $"The new connection name '{newName}' is already in use!");
			}

			connectionStrings.Rename(oldName, newName);

			await this.settings.SetAsync("connections", connectionStrings, cancellationToken).ConfigureAwait(false);
		}




		public async Task DeleteConnectionAsync(string name, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await this.settings.GetAsync<ConnectionSetting>("connections", cancellationToken)
				?? throw new CommandException(CommandException.ConnectionNotSet, "No connections set, nothing to update");
			if (!connectionStrings.Exists(name))
			{
				throw new CommandException(CommandException.CommandInvalidArgumentValue, "Invalid connection name: " + name);
			}

			connectionStrings.Remove(name);
			await TokenCache.ClearAccessTokenAsync(name).ConfigureAwait(false);

			await this.settings.SetAsync("connections", connectionStrings, cancellationToken).ConfigureAwait(false);
		}




		public async Task SetDefaultAsync(string name, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken)
				?? throw new CommandException(CommandException.ConnectionInvalid, "No connection has been set yet.");

			if (!connectionStrings.Exists(name))
				throw new CommandException(CommandException.ConnectionInvalid, "Invalid connection name: " + name);

			if (connectionStrings.CurrentConnectionStringKey == name)
				return; // already set as default

			connectionStrings.CurrentConnectionStringKey = name;
			await this.settings.SetAsync("connections", connectionStrings, cancellationToken).ConfigureAwait(false);
		}



		public async Task SetDefaultSolutionAsync(string uniqueName, CancellationToken cancellationToken = default)
		{
			var connectionStrings = await GetConnectionSettingAsync(cancellationToken)
				?? throw new CommandException(CommandException.ConnectionInvalid, "No connection has been set yet.");
			if (connectionStrings.CurrentConnectionStringKey == null)
				throw new CommandException(CommandException.ConnectionInvalid, "No connection has been set yet.");

			connectionStrings.DefaultSolutions[connectionStrings.CurrentConnectionStringKey] = uniqueName;
			await this.settings.SetAsync("connections", connectionStrings, cancellationToken).ConfigureAwait(false);
		}

		public async Task<string?> GetCurrentDefaultSolutionAsync(CancellationToken cancellationToken = default)
		{
			var project = await this.pacxProjectRepository.GetCurrentProjectAsync().ConfigureAwait(false);
			if (project != null && !project.IsSuspended)
			{
				return project.SolutionName;
			}


			var connectionStrings = await GetConnectionSettingAsync(cancellationToken)
				?? throw new CommandException(CommandException.ConnectionInvalid, "No connection has been set yet.");


			if (connectionStrings.CurrentConnectionStringKey == null)
				throw new CommandException(CommandException.ConnectionInvalid, "No connection has been set yet.");

			if (!connectionStrings.DefaultSolutions.TryGetValue(connectionStrings.CurrentConnectionStringKey, out var uniqueName))
				return null;

			return uniqueName;
		}


		private string GetFullConnectionString(string? connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException(nameof(connectionString), "The connection string is empty.");

			if (connectionString.Contains("TokenCacheStorePath="))
				return connectionString;

			if (!connectionString.EndsWith(';'))
				connectionString += ";";

			return connectionString + "TokenCacheStorePath=" + GetTokenCachePath();
		}



		/// <summary>
		/// This method tries to create a service client
		/// </summary>
		/// <param name="connectionName"></param>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		private async Task<ServiceClient> CreateServiceClientAsync(string connectionName, string connectionString, CancellationToken cancellationToken = default)
		{
			var accessToken = await TokenCache.TryGetAccessTokenAsync(connectionName).ConfigureAwait(false);
			if (accessToken == null)
			{
				return await CreateServiceClientFromConnectionString(connectionName, connectionString).ConfigureAwait(false);
			}


			var crm = new ServiceClient(accessToken.ServiceUri, uri => Task.FromResult(accessToken.AccessToken));
			if (!crm.IsReady)
			{
				return await CreateServiceClientFromConnectionString(connectionName, connectionString).ConfigureAwait(false);
			}

			try
			{
				await crm.ExecuteAsync(new WhoAmIRequest(), cancellationToken).ConfigureAwait(false);
			}
			catch (DataverseConnectionException)
			{
				return await CreateServiceClientFromConnectionString(connectionName, connectionString).ConfigureAwait(false);
			}
			catch (System.ServiceModel.Security.MessageSecurityException)
			{
				return await CreateServiceClientFromConnectionString(connectionName, connectionString).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Debug.Print(ex.GetType().FullName);
			}

			return crm;
		}


		private void RecourseInnerExceptions(Exception ex, int level)
		{
			if (ex?.InnerException != null && level <= 10)
			{
				output.WriteLine($"Inner exception ({level}): {ex.InnerException.Message}", ConsoleColor.Yellow);
				RecourseInnerExceptions(ex.InnerException, level + 1);
			}
		}

		private async Task<ServiceClient> CreateServiceClientFromConnectionString(string connectionName, string connectionString)
		{

			try
			{
				var crm = new ServiceClient(this.GetFullConnectionString(connectionString));
				if (crm.ActiveAuthenticationType == Microsoft.PowerPlatform.Dataverse.Client.AuthenticationType.OAuth
					&& !string.IsNullOrWhiteSpace(crm.CurrentAccessToken))
				{
					await TokenCache.SaveAccessTokenAsync(connectionName, crm.ConnectedOrgUriActual, crm.CurrentAccessToken).ConfigureAwait(false);
				}
				else
				{
					await TokenCache.ClearAccessTokenAsync(connectionName).ConfigureAwait(false);
				}
				return crm;
			}
			catch (DataverseConnectionException ex)
			{
				output.WriteLine($"Failed to create ServiceClient for connection '{connectionName}': {ex.Message}", ConsoleColor.Yellow);
				RecourseInnerExceptions(ex, 1);
				throw;
			}
		}
	}
}
