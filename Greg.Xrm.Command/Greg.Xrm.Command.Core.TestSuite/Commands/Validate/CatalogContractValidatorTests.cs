namespace Greg.Xrm.Command.Commands.Validate
{
	[TestClass]
	public class CatalogContractValidatorTests
	{
		[TestMethod]
		public void Validate_ShouldPassAgainstKnownCatalogShapes()
		{
			var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(root);

			try
			{
				Directory.CreateDirectory(Path.Combine(root, "conductor", "tool-catalog"));
				Directory.CreateDirectory(Path.Combine(root, "conductor", "source-catalog"));
				Directory.CreateDirectory(Path.Combine(root, "conductor", "skill-pack-catalog"));
				Directory.CreateDirectory(Path.Combine(root, "conductor", "flow-mcp-catalog"));

				File.WriteAllText(Path.Combine(root, "conductor", "tool-catalog", "tools.json"), """
{
  "tools": [
	{
      "id": "xrmtoolbox",
      "name": "XrmToolBox",
      "provider": "MscrmTools",
      "category": "Dataverse",
      "kind": "desktop app",
      "summary": "Plugin host and tool library for Dataverse admins.",
      "capabilities": [ "discover plugins" ]
	}
  ]
}
""");
				File.WriteAllText(Path.Combine(root, "conductor", "source-catalog", "sources.json"), """
{
  "sources": [
	{
      "name": "NuGet",
      "provider": "Microsoft",
      "category": "Packages",
      "kind": "feed",
      "summary": "Primary feed for .NET and PACX ecosystem packages.",
      "packages": [ "Microsoft.PowerApps.CLI" ]
	}
  ]
}
""");
				File.WriteAllText(Path.Combine(root, "conductor", "skill-pack-catalog", "skill-packs.json"), """
{
  "skillPacks": [
	{
      "id": "dataverse-skill-pack",
      "title": "Dataverse Skill Pack",
      "summary": "Reusable Dataverse operator guidance.",
      "docs": {
		"guide": "docs/guides/dataverse-skill-pack.md",
		"recipe": "docs/recipes/dataverse-skill-pack.md"
      },
      "commands": [ "pacx validate all" ],
      "consumers": [ "docs" ]
	}
  ]
}
""");
				File.WriteAllText(Path.Combine(root, "conductor", "flow-mcp-catalog", "flows.json"), """
{
  "flows": [
	{
      "name": "Flow Studio Debug",
      "provider": "Flow Studio",
      "category": "Debug",
      "kind": "mcp tool",
      "summary": "Inspect flow runs and failures.",
      "operations": [ "debug" ]
	}
  ]
}
""");

				var validator = new CatalogContractValidator();
				var result = validator.Validate(root);

				Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Errors));
				Assert.AreEqual(0, result.Warnings.Count);
			}
			finally
			{
				Directory.Delete(root, true);
			}
		}

		[TestMethod]
		public void Validate_ShouldReportMissingAndMalformedCatalogs()
		{
			var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(Path.Combine(root, "conductor", "tool-catalog"));
			File.WriteAllText(Path.Combine(root, "conductor", "tool-catalog", "tools.json"), "{ \"tools\": [] }");

			try
			{
				var validator = new CatalogContractValidator();
				var result = validator.Validate(root);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.Errors.Any(error => error.Contains("tool-catalog/tools.json", StringComparison.OrdinalIgnoreCase)));
				Assert.IsTrue(result.Errors.Any(error => error.Contains("Missing catalog file", StringComparison.OrdinalIgnoreCase)));
			}
			finally
			{
				Directory.Delete(root, true);
			}
		}
	}
}
