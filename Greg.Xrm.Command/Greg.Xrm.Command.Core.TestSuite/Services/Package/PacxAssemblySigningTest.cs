using System.Reflection;
using System.Runtime.CompilerServices;

namespace Greg.Xrm.Command.Services.Package
{
	/// <summary>
	/// Verifies that assemblies are strong-name signed and that signing
	/// infrastructure is properly configured across the build.
	/// </summary>
	[TestClass]
	public class PacxAssemblySigningTest
	{
		private static readonly byte[] ExpectedPublicKeyToken = new byte[]
		{
			0x36, 0xea, 0xa0, 0xbb, 0x48, 0x0e, 0x90, 0x2f
		};

		[TestMethod]
		public void CoreAssemblyShouldBeStrongNameSigned()
		{
			var assembly = typeof(PacxAssemblySigningTest).Assembly;
			var name = assembly.GetName();

			Assert.IsNotNull(name.GetPublicKeyToken(), "Assembly must have a public key token (strong-name signed).");
			CollectionAssert.AreEqual(
				ExpectedPublicKeyToken,
				name.GetPublicKeyToken(),
				"Assembly public key token must match the registered pacx_strongname.snk.");
		}

		[TestMethod]
		public void CoreAssemblyShouldHaveInternalsVisibleToTestSuiteWithPublicKey()
		{
			var coreAssembly = typeof(Greg.Xrm.Command.Services.Package.PacxPackageReleaseVerifier).Assembly;
			var attributes = coreAssembly.GetCustomAttributes<InternalsVisibleToAttribute>();

			var testSuiteAttr = attributes.FirstOrDefault(
				a => a.AssemblyName.StartsWith("Greg.Xrm.Command.Core.TestSuite"));

			Assert.IsNotNull(testSuiteAttr, "Assembly must declare InternalsVisibleTo for TestSuite.");
			Assert.IsTrue(
				testSuiteAttr.AssemblyName.Contains("PublicKey="),
				"InternalsVisibleTo must include the PublicKey for strong-name friend access.");
		}

		[TestMethod]
		public void SnkFileShouldExistInBuildContext()
		{
			// Verify the SNK file is accessible from the build context
			var snkCandidates = new[]
			{
				Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "pacx_strongname.snk"),
				Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "pacx_strongname.snk"),
				Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "pacx_strongname.snk"),
			};

			var snkPath = snkCandidates.FirstOrDefault(File.Exists);
			Assert.IsNotNull(snkPath, "pacx_strongname.snk should exist relative to the build output.");
		}
	}
}
