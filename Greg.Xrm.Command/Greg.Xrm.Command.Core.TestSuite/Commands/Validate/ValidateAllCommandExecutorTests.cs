using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Parsing;
using Moq;

namespace Greg.Xrm.Command.Commands.Validate
{
	[TestClass]
	public class ValidateAllCommandExecutorTests
	{
		[TestMethod]
		public void ExecuteAsync_ShouldReportParityAndCatalogSuccess()
		{
			var output = new OutputToMemory();
			var registry = new Mock<ICommandRegistry>().Object;
			var parityValidator = new Mock<ICommandReferenceParityValidator>();
			parityValidator.Setup(v => v.Validate(registry, It.IsAny<string>()))
				.Returns(new CommandReferenceParityResult(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));
			var catalogValidator = new Mock<ICatalogContractValidator>();
			catalogValidator.Setup(v => v.Validate(It.IsAny<string>()))
				.Returns(new CatalogContractValidationResult(Array.Empty<string>(), Array.Empty<string>()));

			var executor = new ValidateAllCommandExecutor(output, registry, parityValidator.Object, catalogValidator.Object);
			var result = executor.ExecuteAsync(new ValidateAllCommand(), CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "Command reference parity passed.");
			StringAssert.Contains(output.ToString(), "Catalog contract validation passed.");
		}

		[TestMethod]
		public void ExecuteAsync_ShouldFailWhenCatalogContractsAreInvalid()
		{
			var output = new OutputToMemory();
			var registry = new Mock<ICommandRegistry>().Object;
			var parityValidator = new Mock<ICommandReferenceParityValidator>();
			parityValidator.Setup(v => v.Validate(registry, It.IsAny<string>()))
				.Returns(new CommandReferenceParityResult(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));
			var catalogValidator = new Mock<ICatalogContractValidator>();
			catalogValidator.Setup(v => v.Validate(It.IsAny<string>()))
				.Returns(new CatalogContractValidationResult(["conductor/tool-catalog/tools.json must contain at least one entry in 'tools'."], Array.Empty<string>()));

			var executor = new ValidateAllCommandExecutor(output, registry, parityValidator.Object, catalogValidator.Object);
			var result = executor.ExecuteAsync(new ValidateAllCommand(), CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "Catalog contract issues:");
			StringAssert.Contains(output.ToString(), "tools.json must contain at least one entry");
		}

		[TestMethod]
		public void ExecuteAsync_ShouldFailWhenCommandReferenceContentIsInvalid()
		{
			var output = new OutputToMemory();
			var registry = new Mock<ICommandRegistry>().Object;
			var parityValidator = new Mock<ICommandReferenceParityValidator>();
			parityValidator.Setup(v => v.Validate(registry, It.IsAny<string>()))
				.Returns(new CommandReferenceParityResult(Array.Empty<string>(), Array.Empty<string>(), ["validate-all.md option 1 description differs."]));
			var catalogValidator = new Mock<ICatalogContractValidator>();
			catalogValidator.Setup(v => v.Validate(It.IsAny<string>()))
				.Returns(new CatalogContractValidationResult(Array.Empty<string>(), Array.Empty<string>()));

			var executor = new ValidateAllCommandExecutor(output, registry, parityValidator.Object, catalogValidator.Object);
			var result = executor.ExecuteAsync(new ValidateAllCommand(), CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "Command reference content issues:");
			StringAssert.Contains(output.ToString(), "description differs");
		}
	}
}
