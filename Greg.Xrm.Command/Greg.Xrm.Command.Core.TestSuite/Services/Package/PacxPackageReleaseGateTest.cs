namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageReleaseGateTest
	{
		[TestMethod]
		public void EvaluateShouldFailWhenReleaseDirectoryDoesNotExist()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			var verifier = new PacxPackageReleaseVerifier();
			var gate = new PacxPackageReleaseGate(verifier);

			var result = gate.Evaluate(tempDir);

			Assert.IsFalse(result.AllGatesPass);
			Assert.IsTrue(result.Errors.Count > 0);
		}

		[TestMethod]
		public void EvaluateShouldFailWhenDirectoryIsEmpty()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_gate_empty");

			try
			{
				var verifier = new PacxPackageReleaseVerifier();
				var gate = new PacxPackageReleaseGate(verifier);

				var result = gate.Evaluate(tempDir);

				Assert.IsFalse(result.AllGatesPass);
				Assert.IsFalse(result.ProvenanceValid);
				Assert.IsFalse(result.SbomComplete);
				Assert.IsFalse(result.AssembliesSigned);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void EvaluateShouldReportSigstoreMissingWhenNoAttestationsPresent()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_gate_sigstore");

			try
			{
				// Create a minimal valid release structure
				File.WriteAllText(Path.Combine(tempDir, "provenance.json"), "{}");
				File.WriteAllText(Path.Combine(tempDir, "sbom.json"), "{}");
				File.WriteAllText(Path.Combine(tempDir, "checksums.txt"), "");
				File.WriteAllText(Path.Combine(tempDir, "RELEASE_NOTES.md"), "");
				File.WriteAllText(Path.Combine(tempDir, "pacx.release.json"), "{}");

				var verifier = new PacxPackageReleaseVerifier();
				var gate = new PacxPackageReleaseGate(verifier);

				var result = gate.Evaluate(tempDir);

				Assert.IsFalse(result.HasSigstoreSignatures, "Expected no Sigstore signatures in empty release dir.");
				Assert.IsFalse(result.AssembliesSigned, "Expected no signed assemblies in empty release dir.");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void EvaluateShouldPassSigstoreCheckWhenAttestationExists()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_gate_sigstore_pass");

			try
			{
				// Fake a .sigstore bundle file
				File.WriteAllText(Path.Combine(tempDir, "package.nupkg.sigstore"), "{\"fake\":true}");

				var verifier = new PacxPackageReleaseVerifier();
				var gate = new PacxPackageReleaseGate(verifier);

				var result = gate.Evaluate(tempDir);

				Assert.IsTrue(result.HasSigstoreSignatures, "Expected Sigstore check to pass with .sigstore file.");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void EvaluateShouldPassSigstoreCheckWithIntotoAttestation()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_gate_intoto");

			try
			{
				// Fake a SLSA attestation file
				File.WriteAllText(Path.Combine(tempDir, "package.intoto.jsonl"), "{\"fake\":true}");

				var verifier = new PacxPackageReleaseVerifier();
				var gate = new PacxPackageReleaseGate(verifier);

				var result = gate.Evaluate(tempDir);

				Assert.IsTrue(result.HasSigstoreSignatures, "Expected Sigstore check to pass with .intoto.jsonl.");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void GateResultAllGatesPassFalseWhenAnyGateFails()
		{
			var verifier = new PacxPackageReleaseVerifier();
			var gate = new PacxPackageReleaseGate(verifier);

			var result = gate.Evaluate(Path.Combine(Path.GetTempPath(), "nonexistent_" + Guid.NewGuid()));

			Assert.IsFalse(result.AllGatesPass);
		}
	}
}
