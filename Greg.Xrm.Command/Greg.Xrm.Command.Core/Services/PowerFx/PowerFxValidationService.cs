using System.Reflection;

namespace Greg.Xrm.Command.Services.PowerFx
{
	public class PowerFxValidationService : IPowerFxValidationService
	{
		public PowerFxValidationResult ValidateExpression(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return new PowerFxValidationResult(false, ["Expression is required."], Array.Empty<string>());
			}

			var powerFxResult = TryValidateWithPowerFx(expression);
			if (powerFxResult != null)
			{
				return powerFxResult;
			}

			var errors = new List<string>();
			CheckBalanced(expression, '(', ')', errors);
			CheckBalanced(expression, '[', ']', errors);
			CheckBalanced(expression, '{', '}', errors);

			return new PowerFxValidationResult(
				errors.Count == 0,
				errors,
				["Microsoft.PowerFx.Core validation was not available at runtime; fallback structural validation was used."]);
		}

		public string FormatExpression(string expression)
		{
			return string.Join(
				" ",
				(expression ?? string.Empty)
					.Replace("\r", " ", StringComparison.Ordinal)
					.Replace("\n", " ", StringComparison.Ordinal)
					.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
		}

		private static PowerFxValidationResult? TryValidateWithPowerFx(string expression)
		{
			try
			{
				var engineType = Type.GetType("Microsoft.PowerFx.Engine, Microsoft.PowerFx.Core");
				if (engineType == null)
				{
					return null;
				}

				var engine = Activator.CreateInstance(engineType);
				var checkMethod = engineType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(method => method.Name == "Check" && method.GetParameters().Length >= 1 && method.GetParameters()[0].ParameterType == typeof(string));
				if (engine == null || checkMethod == null)
				{
					return null;
				}

				var parameters = checkMethod.GetParameters();
				var arguments = new object?[parameters.Length];
				arguments[0] = expression;
				var result = checkMethod.Invoke(engine, arguments);
				if (result == null)
				{
					return null;
				}

				var isSuccess = result.GetType().GetProperty("IsSuccess")?.GetValue(result) as bool? ?? true;
				var errors = ReadErrors(result);
				return new PowerFxValidationResult(isSuccess && errors.Count == 0, errors, Array.Empty<string>());
			}
			catch
			{
				return null;
			}
		}

		private static IReadOnlyList<string> ReadErrors(object result)
		{
			var errorsProperty = result.GetType().GetProperty("Errors");
			if (errorsProperty?.GetValue(result) is not System.Collections.IEnumerable errors)
			{
				return Array.Empty<string>();
			}

			return errors.Cast<object>().Select(error => error.ToString() ?? "Power Fx validation error.").ToArray();
		}

		private static void CheckBalanced(string expression, char open, char close, List<string> errors)
		{
			var depth = 0;
			foreach (var c in expression)
			{
				if (c == open)
				{
					depth++;
				}
				else if (c == close)
				{
					depth--;
				}

				if (depth < 0)
				{
					errors.Add($"Unexpected '{close}' before matching '{open}'.");
					return;
				}
			}

			if (depth > 0)
			{
				errors.Add($"Missing {depth} closing '{close}' character(s).");
			}
		}
	}
}
