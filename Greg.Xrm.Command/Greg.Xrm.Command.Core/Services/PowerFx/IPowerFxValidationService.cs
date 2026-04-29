namespace Greg.Xrm.Command.Services.PowerFx
{
	public interface IPowerFxValidationService
	{
		PowerFxValidationResult ValidateExpression(string expression);

		string FormatExpression(string expression);
	}

	public sealed record PowerFxValidationResult(bool IsValid, IReadOnlyList<string> Errors, IReadOnlyList<string> Warnings);
}
