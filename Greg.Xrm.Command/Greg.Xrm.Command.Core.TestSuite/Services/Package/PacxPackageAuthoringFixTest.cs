using System.Linq;
using System.Text.Json;
using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageAuthoringFixTest
	{
		[TestMethod]
		public async Task FixShouldDeduplicateNormalizeAndPruneManifestEntries()
		{
			var root = TestTempPath.CreateDirectory("pacx_fix");

			try
			{
				Directory.CreateDirectory(Path.Combine(root, "payload"));
				Directory.CreateDirectory(Path.Combine(root, "data"));
				Directory.CreateDirectory(Path.Combine(root, "scripts"));
				File.WriteAllBytes(Path.Combine(root, "payload", "solution.zip"), new byte[] { 1, 2, 3, 4 });
				File.WriteAllText(Path.Combine(root, "data", "accounts.json"), "[]");
				File.WriteAllText(Path.Combine(root, "scripts", "cleanup.js"), "console.log('ok');");

				var manifest = new PacxPackageManifest
				{
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Description = "Sample PACX package",
					Kind = "bundle",
					Artifacts =
					[
						new PacxPackageArtifact { Path = "payload/solution.zip", Role = "solution" },
						new PacxPackageArtifact { Path = "payload/solution.zip", Role = "solution" },
						new PacxPackageArtifact { Path = "data/accounts.json", Role = "data" },
						new PacxPackageArtifact { Path = "data/accounts.json", Role = "data" },
						new PacxPackageArtifact { Path = "payload/old.zip", Role = "solution" }
					],
					Deployment =
					[
						new PacxPackageDeploymentStep { Type = "solutionImport", Artifact = "payload/solution.zip" },
						new PacxPackageDeploymentStep { Type = "solutionImport", Artifact = "payload/solution.zip" },
						new PacxPackageDeploymentStep { Type = "dataImport", Artifact = "data/accounts.json", Table = "account", Mode = "upsert" },
						new PacxPackageDeploymentStep { Type = "dataImport", Artifact = "data/missing.json", Table = "contact", Mode = "upsert" }
					]
				};

				File.WriteAllText(Path.Combine(root, PacxPackageManifest.FileName), JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

				var authoring = new PacxPackageAuthoringService();
				var result = await authoring.FixAsync(new PackageFixCommand
				{
					Path = root
				}, CancellationToken.None);

				Assert.AreEqual(1, result.AddedArtifacts);
				Assert.AreEqual(0, result.AddedSteps);
				Assert.AreEqual(2, result.DedupedArtifacts);
				Assert.AreEqual(1, result.DedupedSteps);
				Assert.AreEqual(1, result.PrunedArtifacts);
				Assert.AreEqual(1, result.PrunedSteps);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual(3, package.Manifest.Artifacts.Count);
				Assert.AreEqual(2, package.Manifest.Deployment.Count);
				Assert.IsTrue(package.Manifest.Artifacts.Any(x => x.Path == "scripts/cleanup.js"));
				Assert.IsFalse(package.Manifest.Artifacts.Any(x => x.Path == "payload/old.zip"));
				Assert.IsFalse(package.Manifest.Deployment.Any(x => x.Artifact == "data/missing.json"));
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
			}
		}

		[TestMethod]
		public async Task FixShouldNormalizeDataOnlyPackagesToDataKind()
		{
			var root = TestTempPath.CreateDirectory("pacx_fix_data_kind");

			try
			{
				Directory.CreateDirectory(Path.Combine(root, "data"));
				File.WriteAllText(Path.Combine(root, "data", "accounts.json"), "[]");

				var manifest = new PacxPackageManifest
				{
					PackageId = "contoso.data",
					Version = "1.0.0",
					Name = "Contoso Data",
					Description = "Sample data package",
					Kind = "bundle",
					Artifacts =
					[
						new PacxPackageArtifact { Path = "data/accounts.json", Role = "data" }
					],
					Deployment =
					[
						new PacxPackageDeploymentStep { Type = "dataImport", Artifact = "data/accounts.json", Table = "account", Mode = "upsert" }
					]
				};

				File.WriteAllText(Path.Combine(root, PacxPackageManifest.FileName), JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

				var authoring = new PacxPackageAuthoringService();
				var result = await authoring.FixAsync(new PackageFixCommand
				{
					Path = root
				}, CancellationToken.None);

				Assert.AreEqual(0, result.AddedArtifacts);
				Assert.AreEqual(0, result.AddedSteps);
				Assert.AreEqual(0, result.DedupedArtifacts);
				Assert.AreEqual(0, result.DedupedSteps);
				Assert.AreEqual(0, result.PrunedArtifacts);
				Assert.AreEqual(0, result.PrunedSteps);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual("data", package.Manifest.Kind);
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
			}
		}
	}
}
