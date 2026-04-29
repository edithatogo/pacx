using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Moq;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageDeployerTest
	{
		[TestMethod]
		public async Task DeployDryRunShouldPrintPlanWithoutApplyingChanges()
		{
			var root = TestTempPath.CreateDirectory("pacx_deploy_dry_run");

			try
			{
				var initializer = new PacxPackageInitializer();
				await initializer.InitializeAsync(new PackageInitCommand
				{
					Path = root,
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Force = true
				}, CancellationToken.None);

				Directory.CreateDirectory(Path.Combine(root, "payload"));
				File.WriteAllBytes(Path.Combine(root, "payload", "solution.zip"), new byte[] { 1, 2, 3, 4 });

				var authoring = new PacxPackageAuthoringService();
				await authoring.SyncAsync(new PackageSyncCommand { Path = root }, CancellationToken.None);

				var output = new OutputToMemory();
				var repository = new Mock<IOrganizationServiceRepository>();
				repository.Setup(x => x.GetCurrentConnectionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new Mock<IOrganizationServiceAsync2>().Object);

				using var package = new PacxPackageReader().Open(root);
				var deployer = new PacxPackageDeployer(output, repository.Object);
				await deployer.DeployAsync(package, dryRun: true, CancellationToken.None);

				var text = output.ToString();
				StringAssert.Contains(text, "PACX package: Contoso Sample");
				StringAssert.Contains(text, "[DRY RUN] No changes will be applied.");
				StringAssert.Contains(text, "Deployment plan:");
				StringAssert.Contains(text, "solutionImport");
				StringAssert.Contains(text, "ready");

				repository.Verify(x => x.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()), Times.Never);
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
		public async Task DeployDryRunShouldMarkUnsupportedAndMissingSteps()
		{
			var root = TestTempPath.CreateDirectory("pacx_deploy_dry_run_invalid");

			try
			{
				var initializer = new PacxPackageInitializer();
				await initializer.InitializeAsync(new PackageInitCommand
				{
					Path = root,
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Force = true
				}, CancellationToken.None);

				Directory.CreateDirectory(Path.Combine(root, "payload"));
				File.WriteAllBytes(Path.Combine(root, "payload", "solution.zip"), new byte[] { 1, 2, 3, 4 });

				var authoring = new PacxPackageAuthoringService();
				await authoring.SyncAsync(new PackageSyncCommand { Path = root }, CancellationToken.None);

				using var package = new PacxPackageReader().Open(root);
				var manifest = package.Manifest;
				manifest.Deployment.Add(new PacxPackageDeploymentStep
				{
					Type = "custom-step",
					Artifact = "scripts/missing.js"
				});

				File.WriteAllText(Path.Combine(root, PacxPackageManifest.FileName), JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

				var output = new OutputToMemory();
				var repository = new Mock<IOrganizationServiceRepository>();
				repository.Setup(x => x.GetCurrentConnectionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new Mock<IOrganizationServiceAsync2>().Object);

				using var refreshedPackage = new PacxPackageReader().Open(root);
				var deployer = new PacxPackageDeployer(output, repository.Object);
				await deployer.DeployAsync(refreshedPackage, dryRun: true, CancellationToken.None);

				var text = output.ToString();
				StringAssert.Contains(text, "unsupported");
				StringAssert.Contains(text, "missing");
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
		public async Task DeployShouldPrintPlanAndConnectForLiveRuns()
		{
			var root = TestTempPath.CreateDirectory("pacx_deploy_live");

			try
			{
				var initializer = new PacxPackageInitializer();
				await initializer.InitializeAsync(new PackageInitCommand
				{
					Path = root,
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Force = true
				}, CancellationToken.None);

				var output = new OutputToMemory();
				var organizationService = new Mock<IOrganizationServiceAsync2>();
				var repository = new Mock<IOrganizationServiceRepository>();
				repository.Setup(x => x.GetCurrentConnectionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(organizationService.Object);

				using var package = new PacxPackageReader().Open(root);
				var deployer = new PacxPackageDeployer(output, repository.Object);
				await deployer.DeployAsync(package, dryRun: false, CancellationToken.None);

				var text = output.ToString();
				StringAssert.Contains(text, "No deployment steps declared.");
				StringAssert.Contains(text, "Connecting to Dataverse...");
				repository.Verify(x => x.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
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
