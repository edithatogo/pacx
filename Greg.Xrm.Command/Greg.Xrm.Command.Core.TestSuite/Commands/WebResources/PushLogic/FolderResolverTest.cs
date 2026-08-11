namespace Greg.Xrm.Command.Commands.WebResources.PushLogic
{
	[TestClass]
	[DoNotParallelize]
	public class FolderResolverTest
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		private FolderResolver _resolver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		[TestInitialize]
		public void Initialize()
		{
			_resolver = new FolderResolver();
		}



		[TestMethod]
		public void Resolve_WithoutPublisher_ShouldThrowArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => _resolver.ResolveFrom(null, string.Empty));
		}

		[TestMethod]
		public void Resolve_WithFullyQualifiedPath_WithoutProjectRoot_ShouldThrowArgumentNullException()
		{
			var path = @"c:\temp\folder";
			Assert.Throws<ArgumentException>(() => _resolver.ResolveFrom(path, "greg"));
		}

		[TestMethod]
		public void Resolve_WithDefaultPath_WithoutProjectRoot_ShouldThrowArgumentNullException()
		{
			Assert.Throws<ArgumentException>(() => _resolver.ResolveFrom(null, "greg"));
		}


		[TestMethod]
		public void Resolve_WithFullyQualifiedPath_ShouldReturnFolder()
		{
			var root = Utils.CreateTempFolder();
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, "greg_\\images");
			Utils.CreateFolder(root, "greg_\\script");
			Utils.CreateFolder(root, "greg_\\src");

			try
			{
				var currentFolder = Path.Combine(root, "greg_\\script");

				var result = _resolver.ResolveFrom(currentFolder, "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(currentFolder, result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Utils.DeleteFolder(root);
			}
		}


		[TestMethod]
		public void Resolve_WithRelativePath_ShouldReturnFolder()
		{
			var originalDirectory = Environment.CurrentDirectory;
			var testDirectory = TestTempPath.CreateDirectory("folder_resolver_relative");
			var root = Path.Combine(testDirectory, "project");
			Directory.CreateDirectory(root);
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, Path.Combine("greg_", "images"));
			Utils.CreateFolder(root, Path.Combine("greg_", "script"));
			Utils.CreateFolder(root, Path.Combine("greg_", "src"));

			try
			{
				Environment.CurrentDirectory = testDirectory;
				var currentFolder = Path.GetRelativePath(Environment.CurrentDirectory, Path.Combine(root, "greg_", "script"));
				var expectedFolder = Path.GetFullPath(currentFolder);

				var result = _resolver.ResolveFrom(currentFolder, "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(expectedFolder, result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
				Directory.Delete(testDirectory, recursive: true);
			}
		}


		[TestMethod]
		public void Resolve_WithRootPath_ShouldReturnFolder()
		{
			var currentDir = Environment.CurrentDirectory;
			var root = Utils.CreateLocalTempFolder();
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, "greg_\\images");
			Utils.CreateFolder(root, "greg_\\script");
			Utils.CreateFolder(root, "greg_\\src");



			try
			{
				Environment.CurrentDirectory = Path.Combine(root, "greg_\\script");

				var result = _resolver.ResolveFrom("$", "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(root, result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Environment.CurrentDirectory = currentDir;
				Utils.DeleteFolder(root);
			}
		}


		[TestMethod]
		public void Resolve_WithRootPathAndSubfolder_ShouldReturnFolder()
		{
			var currentDir = Environment.CurrentDirectory;
			var root = Utils.CreateLocalTempFolder();
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, "greg_\\images");
			Utils.CreateFolder(root, "greg_\\script");
			Utils.CreateFolder(root, "greg_\\src");

			try
			{
				Environment.CurrentDirectory = Path.Combine(root, "greg_\\script");

				var result = _resolver.ResolveFrom("$" + Path.DirectorySeparatorChar + "pages", "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(Path.Combine(root, "pages"), result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Environment.CurrentDirectory = currentDir;
				Utils.DeleteFolder(root);
			}
		}


		[TestMethod]
		public void Resolve_WithSpecialChars_ShouldReturnFolder01()
		{
			var root = Utils.CreateLocalTempFolder();
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, "greg_\\images");
			Utils.CreateFolder(root, "greg_\\script");
			Utils.CreateFolder(root, "greg_\\src");

			try
			{
				var folder = Path.Combine(root, "greg_\\**\\*.txt");
				var result = _resolver.ResolveFrom(folder, "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(folder, result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Utils.DeleteFolder(root);
			}
		}


		[TestMethod]
		public void Resolve_WithSpecialChars_ShouldReturnFolder02()
		{
			var root = Utils.CreateLocalTempFolder();
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, "greg_\\images");
			Utils.CreateFolder(root, "greg_\\script");
			Utils.CreateFolder(root, "greg_\\src");

			try
			{
				var folder = Path.Combine(root, "**\\script\\*.txt");
				var result = _resolver.ResolveFrom(folder, "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(folder, result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Utils.DeleteFolder(root);
			}
		}


		[TestMethod]
		public void Resolve_WithSpecialChars_ShouldReturnFolder03()
		{
			var root = Utils.CreateLocalTempFolder();
			Utils.CreateFile(root, ".wr.pacx", string.Empty);
			Utils.CreateFolder(root, "greg_\\images");
			Utils.CreateFolder(root, "greg_\\script");
			Utils.CreateFolder(root, "greg_\\src");

			try
			{
				var folder = Path.Combine(root, "**");
				var result = _resolver.ResolveFrom(folder, "greg");

				Assert.IsNotNull(result);
				Assert.AreEqual("greg", result.PublisherPrefix);
				Assert.AreEqual(root, result.RequestedPath);
				Assert.AreEqual(root, result.ProjectRootPath);
			}
			finally
			{
				Utils.DeleteFolder(root);
			}
		}
	}
}
