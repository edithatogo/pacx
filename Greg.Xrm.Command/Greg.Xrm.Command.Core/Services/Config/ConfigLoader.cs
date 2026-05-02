using System.Text.Json;

namespace Greg.Xrm.Command.Services.Config
{
	public class ConfigLoader
	{
		private const string ConfigFileName = "pacx.config.json";
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true
		};

		public PacxConfig? Load()
		{
			var foundPath = FindConfigPath();
			if (foundPath == null)
				return null;

			return LoadFrom(foundPath);
		}

		public string? FindConfigPath()
		{
			var dir = new DirectoryInfo(Environment.CurrentDirectory);
			while (dir != null)
			{
				var candidate = Path.Combine(dir.FullName, ConfigFileName);
				if (File.Exists(candidate))
					return candidate;
				dir = dir.Parent;
			}

			var homeConfig = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				".pacx",
				ConfigFileName);

			if (File.Exists(homeConfig))
				return homeConfig;

			return null;
		}

		public PacxConfig? LoadFrom(string path)
		{
			if (!File.Exists(path))
				return null;

			var json = File.ReadAllText(path);
			return JsonSerializer.Deserialize<PacxConfig>(json, JsonOptions);
		}
	}
}
