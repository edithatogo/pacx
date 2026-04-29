
using ClosedXML.Excel;
using System.IO;

namespace Greg.Xrm.Command.Commands.Settings.Imports
{
	public class ImportStrategyFromExcel : IImportStrategy
	{
		private static readonly System.Threading.Lock tempEnvironmentLock = new();
		private readonly Stream stream;

		public ImportStrategyFromExcel(Stream stream)
		{
			this.stream = stream;
		}

		public async Task<IReadOnlyList<IImportAction>> ImportAsync(CancellationToken cancellationToken)
		{
			var tempRoot = Path.Combine(Environment.CurrentDirectory, ".test-temp", "excel-temp");
			Directory.CreateDirectory(tempRoot);

			lock (tempEnvironmentLock)
			{
				var previousTemp = Environment.GetEnvironmentVariable("TEMP");
				var previousTmp = Environment.GetEnvironmentVariable("TMP");
				try
				{
					Environment.SetEnvironmentVariable("TEMP", tempRoot);
					Environment.SetEnvironmentVariable("TMP", tempRoot);

					using var workbook = new XLWorkbook(this.stream);
					var worksheet = workbook.Worksheets.FirstOrDefault()
						?? throw new CommandException(CommandException.CommandInvalidArgumentValue, "The provided workbook does not contain any worksheets.");

					var headerRow = FindHeaderRow(worksheet);
					if (headerRow == 0)
					{
						throw new CommandException(CommandException.CommandInvalidArgumentValue, "The provided workbook does not contain the expected settings header row.");
					}

					var appColumns = FindAppColumns(worksheet, headerRow);
					var actions = new List<IImportAction>();
					var seenSettings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

					var lastRowUsed = worksheet.LastRowUsed();
					var lastRow = lastRowUsed is null ? headerRow : lastRowUsed.RowNumber();

					for (var row = headerRow + 1; row <= lastRow; row++)
					{
						var uniqueName = worksheet.Cell(row, 1).GetString().Trim();
						if (string.IsNullOrWhiteSpace(uniqueName))
						{
							continue;
						}

						if (!seenSettings.Add(uniqueName))
						{
							throw new CommandException(CommandException.CommandInvalidArgumentValue, $"The provided file contains duplicated settings: {uniqueName.ToLowerInvariant()}");
						}

						var defaultValue = worksheet.Cell(row, 3).GetString();
						if (!string.IsNullOrWhiteSpace(defaultValue))
						{
							actions.Add(new ImportActionSetDefaultValue(uniqueName, defaultValue));
						}

						var environmentValue = worksheet.Cell(row, 4).GetString();
						if (!string.IsNullOrWhiteSpace(environmentValue))
						{
							actions.Add(new ImportActionSetEnvironmentValue(uniqueName, environmentValue));
						}

						foreach (var appColumn in appColumns)
						{
							var value = worksheet.Cell(row, appColumn).GetString();
							if (string.IsNullOrWhiteSpace(value))
							{
								continue;
							}

							var appName = worksheet.Cell(headerRow, appColumn).GetString().Trim();
							if (string.IsNullOrWhiteSpace(appName))
							{
								continue;
							}

							actions.Add(new ImportActionSetAppValue(uniqueName, appName, value));
						}
					}

					return actions;
				}
				finally
				{
					Environment.SetEnvironmentVariable("TEMP", previousTemp);
					Environment.SetEnvironmentVariable("TMP", previousTmp);
				}
			}
		}

		private static int FindHeaderRow(IXLWorksheet worksheet)
		{
			foreach (var row in worksheet.RowsUsed())
			{
				if (string.Equals(row.Cell(1).GetString().Trim(), "Unique Name", StringComparison.OrdinalIgnoreCase))
				{
					return row.RowNumber();
				}
			}

			return 0;
		}

		private static List<int> FindAppColumns(IXLWorksheet worksheet, int headerRow)
		{
			var result = new List<int>();
			var lastColumn = worksheet.Row(headerRow).LastCellUsed()?.Address.ColumnNumber ?? 0;
			for (var column = 5; column <= lastColumn; column++)
			{
				var header = worksheet.Cell(headerRow, column).GetString().Trim();
				if (string.IsNullOrWhiteSpace(header))
				{
					continue;
				}

				if (header.Equals("Type", StringComparison.OrdinalIgnoreCase)
					|| header.Equals("Visible", StringComparison.OrdinalIgnoreCase)
					|| header.Equals("Overridable on", StringComparison.OrdinalIgnoreCase))
				{
					break;
				}

				result.Add(column);
			}

			return result;
		}
	}
}
