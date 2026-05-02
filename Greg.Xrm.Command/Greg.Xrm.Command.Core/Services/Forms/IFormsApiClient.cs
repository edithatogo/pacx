using System.Text.Json;

namespace Greg.Xrm.Command.Services.Forms
{
	public interface IFormsApiClient
	{
		Task<IReadOnlyList<FormsForm>> GetFormsAsync(string tenantId, string ownerId, string ownerType, CancellationToken ct);
		Task<FormsForm?> GetFormDetailAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct);
		Task<int> GetResponseCountAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct);
		Task<IReadOnlyList<FormsResponse>> GetResponsesAsync(string tenantId, string ownerId, string ownerType, string formId, int top, int skip, CancellationToken ct);
		Task<FormsResponse?> GetResponseAsync(string tenantId, string ownerId, string ownerType, string formId, int responseId, CancellationToken ct);
		Task<JsonDocument?> GetBranchingAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct);
		Task<JsonDocument?> GetAnalyticsAsync(string tenantId, string ownerId, string ownerType, string formId, CancellationToken ct);
		Task ShareFormAsync(string tenantId, string ownerId, string ownerType, string formId, string groupId, string role, CancellationToken ct);
		Task TransferOwnershipAsync(string tenantId, string ownerId, string ownerType, string formId, string targetUpn, CancellationToken ct);
		Task<IReadOnlyList<FormsForm>> ListTemplatesAsync(string tenantId, CancellationToken ct);
		Task CreateTemplateAsync(string tenantId, string formId, string scope, CancellationToken ct);
		Task ShareTemplateAsync(string tenantId, string templateId, string groupId, CancellationToken ct);
	}

	public sealed class FormsForm
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string? OwnerId { get; set; }
		public int RowCount { get; set; }
		public string? Type { get; set; }
		public bool SoftDeleted { get; set; }
	}

	public sealed class FormsResponse
	{
		public int Id { get; set; }
		public DateTime SubmittedAt { get; set; }
		public string? Answers { get; set; }
	}
}
