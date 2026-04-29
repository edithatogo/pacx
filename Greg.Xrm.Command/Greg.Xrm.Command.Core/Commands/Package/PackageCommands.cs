using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;

namespace Greg.Xrm.Command.Commands.Package
{
	[Command("package", "show", HelpText = "Inspect a PACX-native package or package folder.")]
	public class PackageShowCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to a .pacx package file or a package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;
	}

	[Command("package", "list", HelpText = "List the editable contents of a PACX-native package folder.")]
	public class PackageListCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to a .pacx package file or a package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;
	}

	[Command("package", "sync", HelpText = "Synchronize a PACX package manifest with folder contents.")]
	public class PackageSyncCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("prune-missing", Order = 2, HelpText = "Remove manifest entries for artifacts that no longer exist in the folder.")]
		public bool PruneMissing { get; set; }
	}

	[Command("package", "fix", HelpText = "Fix and normalize a PACX package manifest from folder contents.")]
	public class PackageFixCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("prune-missing", Order = 2, DefaultValue = true, HelpText = "Remove manifest entries for artifacts that no longer exist in the folder.")]
		public bool PruneMissing { get; set; } = true;
	}

	[Command("package", "publish", HelpText = "Publish a PACX package archive and release manifest to a destination folder.")]
	public class PackagePublishCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to a .pacx package file or a package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("destination", "d", Order = 2, Required = true, HelpText = "Destination folder for the published archive and release metadata.")]
		[Required]
		public string DestinationPath { get; set; } = string.Empty;

		[Option("version", "v", Order = 3, HelpText = "Override the package version for the published archive and release manifest.")]
		public string? Version { get; set; }

		[Option("force", "f", Order = 4, HelpText = "Overwrite existing published artifacts in the destination folder.")]
		public bool Force { get; set; }
	}

	[Command("package", "release", HelpText = "Stage a PACX release folder with archive, manifest, notes, and checksums.")]
	public class PackageReleaseCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to a .pacx package file or a package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("destination", "d", Order = 2, Required = true, HelpText = "Destination folder for the staged release folder.")]
		[Required]
		public string DestinationPath { get; set; } = string.Empty;

		[Option("version", "v", Order = 3, HelpText = "Override the package version for the staged release folder.")]
		public string? Version { get; set; }

		[Option("force", "f", Order = 4, HelpText = "Overwrite existing release folders and files.")]
		public bool Force { get; set; }
	}

	[Command("package", "deploy", HelpText = "Deploy a PACX-native package to the current Dataverse environment.")]
	public class PackageDeployCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to a .pacx package file or a package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("dry-run", Order = 2, HelpText = "Validate the package and print the deployment plan without making changes.")]
		public bool DryRun { get; set; }
	}

	[Command("package", "build", HelpText = "Build a PACX-native package archive from a package folder.")]
	public class PackageBuildCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder containing pacx.package.json.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("output", "o", Order = 2, HelpText = "Output .pacx file. If omitted, PACX chooses a name based on the manifest.")]
		public string? OutputPath { get; set; }
	}

	[Command("package", "validate", HelpText = "Validate a PACX-native package or package folder.")]
	public class PackageValidateCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to a .pacx package file or a package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;
	}

	[Command("package", "init", HelpText = "Scaffold a starter PACX package folder.")]
	public class PackageInitCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder to create.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("package-id", "id", Order = 2, HelpText = "Package identifier. Defaults to the folder name.")]
		public string? PackageId { get; set; }

		[Option("version", "v", Order = 3, DefaultValue = "1.0.0", HelpText = "Package version.")]
		public string Version { get; set; } = "1.0.0";

		[Option("name", "n", Order = 4, HelpText = "Display name. Defaults to the package identifier.")]
		public string? Name { get; set; }

		[Option("description", "d", Order = 5, HelpText = "Package description.")]
		public string? Description { get; set; }

		[Option("kind", "k", Order = 6, DefaultValue = "bundle", HelpText = "Package kind. Use bundle, solution, or data.")]
		public string Kind { get; set; } = "bundle";

		[Option("force", "f", Order = 7, HelpText = "Overwrite existing files and folders.")]
		public bool Force { get; set; }
	}

	[Command("package", "add", "solution", HelpText = "Add a solution payload to a PACX package folder.")]
	public class PackageAddSolutionCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("source", "s", Order = 2, Required = true, HelpText = "Path to the source .zip solution payload.")]
		[Required]
		public string SourcePath { get; set; } = string.Empty;

		[Option("artifact", "a", Order = 3, HelpText = "Relative artifact path inside the package. Defaults to payload/<file>.")]
		public string? ArtifactPath { get; set; }

		[Option("force", "f", Order = 4, HelpText = "Overwrite an existing artifact with the same path.")]
		public bool Force { get; set; }

		[Option("overwrite-unmanaged-customizations", Order = 5, DefaultValue = true, HelpText = "Default deployment flag for solution import.")]
		public bool OverwriteUnmanagedCustomizations { get; set; } = true;

		[Option("publish-workflows", Order = 6, DefaultValue = true, HelpText = "Default deployment flag for solution import.")]
		public bool PublishWorkflows { get; set; } = true;
	}

	[Command("package", "add", "data", HelpText = "Add a data payload to a PACX package folder.")]
	public class PackageAddDataCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("source", "s", Order = 2, Required = true, HelpText = "Path to the source .json data payload.")]
		[Required]
		public string SourcePath { get; set; } = string.Empty;

		[Option("table", "t", Order = 3, HelpText = "Target Dataverse table name. Defaults to the source file name.")]
		public string? Table { get; set; }

		[Option("artifact", "a", Order = 4, HelpText = "Relative artifact path inside the package. Defaults to data/<file>.")]
		public string? ArtifactPath { get; set; }

		[Option("force", "f", Order = 5, HelpText = "Overwrite an existing artifact with the same path.")]
		public bool Force { get; set; }

		[Option("mode", "m", Order = 6, DefaultValue = "upsert", HelpText = "Default deployment mode for data import.")]
		public string Mode { get; set; } = "upsert";
	}

	[Command("package", "remove", "solution", HelpText = "Remove a solution payload from a PACX package folder.")]
	public class PackageRemoveSolutionCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("artifact", "a", Order = 2, Required = true, HelpText = "Relative artifact path inside the package.")]
		[Required]
		public string ArtifactPath { get; set; } = string.Empty;

		[Option("force", "f", Order = 3, HelpText = "Remove the manifest entries even if the artifact file is missing.")]
		public bool Force { get; set; }
	}

	[Command("package", "remove", "data", HelpText = "Remove a data payload from a PACX package folder.")]
	public class PackageRemoveDataCommand
	{
		[Option("path", "p", Order = 1, Required = true, HelpText = "Path to the package folder.")]
		[Required]
		public string Path { get; set; } = string.Empty;

		[Option("artifact", "a", Order = 2, Required = true, HelpText = "Relative artifact path inside the package.")]
		[Required]
		public string ArtifactPath { get; set; } = string.Empty;

		[Option("force", "f", Order = 3, HelpText = "Remove the manifest entries even if the artifact file is missing.")]
		public bool Force { get; set; }
	}
}
