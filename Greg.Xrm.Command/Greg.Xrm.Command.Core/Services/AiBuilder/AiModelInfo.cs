using System;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public class AiModelInfo
	{
		public string Id { get; set; } = "";
		public string Name { get; set; } = "";
		public string Status { get; set; } = "";
		public string? ErrorMessage { get; set; }
		public DateTime? CreatedOn { get; set; }
		public DateTime? TrainedOn { get; set; }
		public double? Accuracy { get; set; }
	}
}
