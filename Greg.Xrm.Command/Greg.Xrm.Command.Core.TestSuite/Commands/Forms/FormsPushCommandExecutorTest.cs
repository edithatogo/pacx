using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsPushCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldPushFormAndQuestions()
		{
			// 1. Create a temporary local manifest
			var tempFile = Path.GetTempFileName();
			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Create("Test Push Form", "A test form to push", false);
				store.AddQuestion(document, "Question 1", "text", true);
				store.AddSection(document, "Section 1", "A test section", 1);
				store.Save(tempFile, document);

				// 2. Setup Mock API Client
				var apiMock = new Mock<IFormsApiClient>();
				apiMock.Setup(a => a.CreateFormAsync(
						"contoso.onmicrosoft.com", "me", "User", "Test Push Form", "A test form to push", It.IsAny<CancellationToken>()))
					.ReturnsAsync(new FormsForm { Id = "online_f1", Title = "Test Push Form" });

				apiMock.Setup(a => a.CreateQuestionAsync(
						"contoso.onmicrosoft.com", "me", "User", "online_f1", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);

				// 3. Execute
				var output = new OutputToMemory();
				var executor = new FormsPushCommandExecutor(output, apiMock.Object);
				var result = executor.ExecuteAsync(
					new FormsPushCommand
					{
						FilePath = tempFile,
						TenantId = "contoso.onmicrosoft.com"
					},
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				apiMock.Verify(a => a.CreateFormAsync("contoso.onmicrosoft.com", "me", "User", "Test Push Form", "A test form to push", It.IsAny<CancellationToken>()), Times.Once);
				apiMock.Verify(a => a.CreateQuestionAsync("contoso.onmicrosoft.com", "me", "User", "online_f1", "Section 1", "Page", false, null, It.IsAny<CancellationToken>()), Times.Once);
				apiMock.Verify(a => a.CreateQuestionAsync("contoso.onmicrosoft.com", "me", "User", "online_f1", "Question 1", "text", true, It.IsAny<List<string>>(), It.IsAny<CancellationToken>()), Times.Once);
			}
			finally
			{
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}
			}
		}
	}
}
