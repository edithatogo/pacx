using Microsoft.PowerPlatform.Dataverse.Client;

namespace Greg.Xrm.Command.Model
{
	public interface ISolutionComponentRepository
	{
		Task<List<SolutionComponent>> GetBySolutionIdAsync(IOrganizationServiceAsync2 crm, Guid solutionId, CancellationToken cancellationToken = default);

		Task<SolutionComponent?> GetBySolutionIdAndObjectIdAsync(IOrganizationServiceAsync2 crm, Guid solutionId, Guid objectId, CancellationToken cancellationToken = default);
	}
}
