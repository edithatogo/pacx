using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Config
{
	[TestClass]
	public class ConfigLoaderTest
	{
		[TestMethod]
		public void LoadFromShouldReturnNullForMissingPath()
		{
			var loader = new ConfigLoader();
			var result = loader.LoadFrom(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nope.json"));
			Assert.IsNull(result);
		}

		[TestMethod]
		public void LoadFromShouldParseCompleteConfig()
		{
			var tempDir = TestTempPath.CreateDirectory("config_test");
			try
			{
				var configPath = Path.Combine(tempDir, "pacx.config.json");
				File.WriteAllText(configPath, """
				{
					"defaultTenantId": "tenant-123",
					"defaultEnvironmentUrl": "https://org.crm.dynamics.com",
					"outputFormat": "json",
					"connections": {
						"prod": "AuthType=OAuth;Url=https://prod.crm.dynamics.com"
					},
					"options": {
						"timeout": 30
					}
				}
				""");

				var loader = new ConfigLoader();
				var config = loader.LoadFrom(configPath);

				Assert.IsNotNull(config);
				Assert.AreEqual("tenant-123", config.DefaultTenantId);
				Assert.AreEqual("https://org.crm.dynamics.com", config.DefaultEnvironmentUrl);
				Assert.AreEqual("json", config.OutputFormat);
				Assert.AreEqual(1, config.Connections.Count);
				Assert.IsTrue(config.Connections.ContainsKey("prod"));
				Assert.AreEqual(1, config.Options.Count);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void LoadFromShouldParsePartialConfig()
		{
			var tempDir = TestTempPath.CreateDirectory("config_partial_test");
			try
			{
				var configPath = Path.Combine(tempDir, "pacx.config.json");
				File.WriteAllText(configPath, """
				{
					"defaultTenantId": "tenant-456"
				}
				""");

				var loader = new ConfigLoader();
				var config = loader.LoadFrom(configPath);

				Assert.IsNotNull(config);
				Assert.AreEqual("tenant-456", config.DefaultTenantId);
				Assert.IsNull(config.DefaultEnvironmentUrl);
				Assert.AreEqual("table", config.OutputFormat);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void FindConfigPathShouldDiscoverAncestorConfig()
		{
			var tempDir = TestTempPath.CreateDirectory("config_discover");
			try
			{
				var configPath = Path.Combine(tempDir, "pacx.config.json");
				File.WriteAllText(configPath, "{}");

				var originalDir = Environment.CurrentDirectory;
				try
				{
					var subDir = Directory.CreateDirectory(Path.Combine(tempDir, "sub", "deep")).FullName;
					Environment.CurrentDirectory = subDir;

					var loader = new ConfigLoader();
					var found = loader.FindConfigPath();

					Assert.IsNotNull(found);
					Assert.AreEqual(configPath, found);
				}
				finally
				{
					Environment.CurrentDirectory = originalDir;
				}
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void FindConfigPathShouldReturnNullWhenNoConfigExists()
		{
			var tempDir = TestTempPath.CreateDirectory("no_config_test");
			try
			{
				var originalDir = Environment.CurrentDirectory;
				try
				{
					Environment.CurrentDirectory = tempDir;
					var loader = new ConfigLoader();
					var found = loader.FindConfigPath();
					Assert.IsNull(found);
				}
				finally
				{
					Environment.CurrentDirectory = originalDir;
				}
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void LoadShouldReturnNullWithNoConfig()
		{
			var tempDir = TestTempPath.CreateDirectory("no_config_loader_test");
			try
			{
				var originalDir = Environment.CurrentDirectory;
				try
				{
					Environment.CurrentDirectory = tempDir;
					var loader = new ConfigLoader();
					var config = loader.Load();
					Assert.IsNull(config);
				}
				finally
				{
					Environment.CurrentDirectory = originalDir;
				}
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
