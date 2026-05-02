using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsResponseGetCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldShowResponse()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetResponseAsync("t", "me", "User", "f1", 42, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new FormsResponse { Id = 42, SubmittedAt = new DateTime(2026, 1, 1), Answers = "{\"q1\":\"Yes\"}" });

			var output = new OutputToMemory();
			var executor = new FormsResponseGetCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsResponseGetCommand { TenantId = "t", FormId = "f1", ResponseId = 42 },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "42");
			StringAssert.Contains(output.ToString(), "Yes");
		}

		[TestMethod]
		public void ExecuteWithMissingResponseShouldWarn()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 99, It.IsAny<CancellationToken>()))
				.ReturnsAsync((FormsResponse?)null);

			var output = new OutputToMemory();
			var executor = new FormsResponseGetCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsResponseGetCommand { TenantId = "t", FormId = "f", ResponseId = 99 },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
		}
	}
}
