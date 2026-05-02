using System.Text.Json;
using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsBranchingExportCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldReturnBranchingJson()
		{
			var apiMock = new Mock<IFormsApiClient>();
			var branching = JsonDocument.Parse("""{"rules":[{"questionId":"q1","condition":"equals","value":"Yes","targetQuestionId":"q3"}]}""");
			apiMock.Setup(a => a.GetBranchingAsync("contoso.onmicrosoft.com", "me", "User", "f1", It.IsAny<CancellationToken>()))
				.ReturnsAsync(branching);

			var output = new OutputToMemory();
			var executor = new FormsBranchingExportCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsBranchingExportCommand { TenantId = "contoso.onmicrosoft.com", FormId = "f1" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "rules");
		}

		[TestMethod]
		public void ExecuteWithApiErrorShouldFail()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetBranchingAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("API error"));

			var executor = new FormsBranchingExportCommandExecutor(new OutputToMemory(), apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsBranchingExportCommand { TenantId = "t", FormId = "f" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "API error");
		}
	}
}
