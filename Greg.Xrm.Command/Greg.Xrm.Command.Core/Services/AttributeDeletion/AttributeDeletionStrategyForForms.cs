using System.Xml.Linq;
using System.Xml.XPath;
using Greg.Xrm.Command.Model;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Greg.Xrm.Command.Services.AttributeDeletion
{
	public class AttributeDeletionStrategyForForms(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository,
		ISolutionRepository solutionRepository
		)
	: AttributeDeletionStrategyBase
	{
		public override ComponentType ComponentType => ComponentType.SystemForm;

		protected override async Task HandleInternalAsync(IOrganizationServiceAsync2 crm, AttributeMetadata attribute, IReadOnlyList<Dependency> dependencies, CancellationToken cancellationToken)
		{

			// Create a temporary holding solution that will hold all the forms having the current field within
			using (var solution = await CreateHoldingSolutionAsync(crm, cancellationToken))
			{
				foreach (var form in dependencies)
				{
					await solution.AddComponentAsync(form.dependentcomponentobjectid, ComponentType.SystemForm, cancellationToken).ConfigureAwait(false);
				}

				// now I need to download the solution, extract the customizations.xml file, modify it, and re-upload it
				using (var solutionContent = await solution.DownloadAsync(cancellationToken))
				{
					var tempDir = CreateTempFolder();
					try
					{
						var fileName = Path.Combine(tempDir, $"{solution}_original.zip");
						await solutionContent.SaveToAsync(fileName).ConfigureAwait(false);

						solutionContent.UpdateEntryXml("customizations.xml", doc =>
						{
							var formList = doc.XPathSelectElements("./ImportExportXml/Entities/Entity/FormXml/forms/systemform")?.ToArray() ?? [];

							var i = 0;
							foreach (var form in formList)
							{
								i++;
								UpdateFormXml(i, formList.Length, form, attribute);
							}
							return true;
						});

						var newZipBytes = solutionContent.ToArray();

						await solution.UploadAndPublishAsync(newZipBytes, cancellationToken, attribute.EntityLogicalName).ConfigureAwait(false);
					}
					finally
					{
						Directory.Delete(tempDir, true);
					}
				}
			}
		}

		private void UpdateFormXml(int i, int formCount, XElement formXml, AttributeMetadata attribute)
		{
			var formName = formXml.XPathSelectElement("./LocalizedNames/LocalizedName")?.Attribute("description")?.Value ?? "unnamed";

			output.Write($"Updating systemform {i}/{formCount} {formName}...");

			var controlNodes = formXml.XPathSelectElements($"./form/tabs/tab/columns/column/sections/section/rows/row/cell/control[@datafieldname=\"{attribute.LogicalName}\"]")?.ToArray() ?? [];
			foreach (var controlNode in controlNodes)
			{
				var rowNode = controlNode.Parent?.Parent; // here we are on the row node
				rowNode?.Remove();
			}
			controlNodes = formXml.XPathSelectElements($"./form/header/rows/row/cell/control[@datafieldname=\"{attribute.LogicalName}\"]")?.ToArray() ?? [];
			foreach (var cellNode in controlNodes.Select(x => x.Parent))
			{
				cellNode?.Remove();
			}

			output.WriteLine("Done", ConsoleColor.Green);
		}




		private async Task<ITemporarySolution> CreateHoldingSolutionAsync(IOrganizationServiceAsync2 crm, CancellationToken cancellationToken)
		{
			var currentSolutionName = await organizationServiceRepository.GetCurrentDefaultSolutionAsync().ConfigureAwait(false);
			if (currentSolutionName == null)
			{
				throw new AttributeDeletionException("No solution name provided and no current solution name found in the settings.");
			}

			output.Write($"Creating temporary holding solution...");
			var currentSolution = await solutionRepository.GetByUniqueNameAsync(crm, currentSolutionName, cancellationToken).ConfigureAwait(false);
			if (currentSolution == null)
			{
				throw new AttributeDeletionException($"Solution {currentSolutionName} not found");
			}

			var solution = await solutionRepository.CreateTemporarySolutionAsync(crm, currentSolution.publisherid, cancellationToken).ConfigureAwait(false);
			output.WriteLine("Done", ConsoleColor.Green);


			return solution;
		}

		public static string CreateTempFolder()
		{
			var temp = Path.Combine(Path.GetTempPath(), "PACX-" + DateTimeOffset.Now.Ticks);
			Directory.CreateDirectory(temp);
			return temp;
		}
	}
}
