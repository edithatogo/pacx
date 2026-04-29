using Greg.Xrm.Command.Services.Output;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Model;

[TestClass]
public class CancellationTokenPlumbingTests
{
	[TestMethod]
	public async Task SolutionRepositoryShouldForwardCancellationToken()
	{
		var output = new OutputToMemory();
		var repository = new Solution.Repository(output);
		var crm = new Mock<IOrganizationServiceAsync2>();
		using var cts = new CancellationTokenSource();

		var entity = new Entity("solution")
		{
			["uniquename"] = "sample-solution",
			["ismanaged"] = false,
			["version"] = "1.0.0",
			["publisherid"] = new EntityReference("publisher", Guid.NewGuid())
		};

		crm.Setup(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), cts.Token))
			.ReturnsAsync(new EntityCollection([entity]));

		var solution = await repository.GetByUniqueNameAsync(crm.Object, "sample-solution", cts.Token);

		Assert.IsNotNull(solution);
		Assert.AreEqual("sample-solution", solution!.uniquename);
		crm.Verify(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), cts.Token), Times.Once);
	}

	[TestMethod]
	public async Task WebResourceRepositoryShouldForwardCancellationToken()
	{
		var output = new OutputToMemory();
		var repository = new WebResource.Repository(output);
		var crm = new Mock<IOrganizationServiceAsync2>();
		using var cts = new CancellationTokenSource();

		var entity = new Entity("webresource")
		{
			["name"] = "sample.js",
			["displayname"] = "sample.js",
			["webresourcetype"] = new OptionSetValue((int)WebResourceType.Script),
			["description"] = "sample"
		};

		crm.Setup(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), cts.Token))
			.ReturnsAsync(new EntityCollection([entity]));

		var webResources = await repository.GetBySolutionAsync(crm.Object, "sample-solution", cancellationToken: cts.Token);

		Assert.AreEqual(1, webResources.Count);
		Assert.AreEqual("sample.js", webResources[0].name);
		crm.Verify(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), cts.Token), Times.Once);
	}

	[TestMethod]
	public async Task TemporarySolutionShouldForwardCancellationToken()
	{
		var output = new OutputToMemory();
		var crm = new Mock<IOrganizationServiceAsync2>();
		using var cts = new CancellationTokenSource();

		var solutionEntity = new Entity("solution")
		{
			["uniquename"] = "sample-solution",
			["ismanaged"] = false,
			["version"] = "1.0.0",
			["publisherid"] = new EntityReference("publisher", Guid.NewGuid())
		};

		crm.Setup(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), cts.Token))
			.ReturnsAsync(new EntityCollection([solutionEntity]));
		crm.Setup(x => x.ExecuteAsync(It.IsAny<AddSolutionComponentRequest>(), cts.Token))
			.ReturnsAsync(new AddSolutionComponentResponse());

		var repository = new Solution.Repository(output);
		var solution = await repository.GetByUniqueNameAsync(crm.Object, "sample-solution", cts.Token);
		Assert.IsNotNull(solution);

		var tempSolution = new TemporarySolution(crm.Object, output, solution!);
		await tempSolution.AddComponentAsync(Guid.NewGuid(), ComponentType.SystemForm, cts.Token);

		crm.Verify(x => x.ExecuteAsync(It.IsAny<AddSolutionComponentRequest>(), cts.Token), Times.Once);
	}
}
