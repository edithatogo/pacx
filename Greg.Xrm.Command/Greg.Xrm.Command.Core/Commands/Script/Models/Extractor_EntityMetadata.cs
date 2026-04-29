using Microsoft.Xrm.Sdk.Metadata;

namespace Greg.Xrm.Command.Commands.Script.Models
{
	public class Extractor_EntityMetadata
	{
		public string SchemaName { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
		public string PluralName { get; set; } = string.Empty;
		public bool IsCustomEntity { get; set; }
		public List<AttributeMetadata> Fields { get; set; } = new();
		public List<Extractor_RelationshipMetadata> Relationships { get; set; } = new();
		public List<Extractor_OptionSetMetadata> LocalOptionSets { get; set; } = new();
		public string? PrimaryFieldName { get; set; }
		public string? PrimaryFieldSchemaName { get; set; }
		public string? PrimaryFieldDescription { get; set; }
		public int? PrimaryFieldMaxLength { get; set; }
		public string? PrimaryFieldAutoNumberFormat { get; set; }
		public string? PrimaryFieldRequiredLevel { get; set; }

		private static Extractor_EntityMetadata BuildEntityMetadata(EntityMetadata e, List<AttributeMetadata> fields, List<string> prefixes)
		{
			var entity = new Extractor_EntityMetadata
			{
				SchemaName = e.LogicalName,
				DisplayName = e.DisplayName?.UserLocalizedLabel?.Label ?? e.LogicalName,
				PluralName = e.DisplayCollectionName?.UserLocalizedLabel?.Label ?? e.LogicalName,
				IsCustomEntity = prefixes.Any(pre => e.LogicalName.StartsWith(pre)),
				Fields = new List<AttributeMetadata>()
			};
			var primaryField = e.Attributes?.FirstOrDefault(a => a.LogicalName == e.PrimaryNameAttribute);
			if (primaryField is StringAttributeMetadata strAttr)
			{
				entity.PrimaryFieldName = strAttr.DisplayName?.UserLocalizedLabel?.Label ?? strAttr.LogicalName;
				entity.PrimaryFieldSchemaName = strAttr.LogicalName;
				entity.PrimaryFieldDescription = strAttr.Description?.UserLocalizedLabel?.Label;
				entity.PrimaryFieldMaxLength = strAttr.MaxLength;
				entity.PrimaryFieldAutoNumberFormat = strAttr.AutoNumberFormat;
				entity.PrimaryFieldRequiredLevel = strAttr.RequiredLevel?.Value.ToString();
			}
			foreach (var a in fields)
			{
				entity.Fields.Add(a);
			}
			return entity;
		}

		public static List<Extractor_EntityMetadata> ExtractEntitiesByPrefix(IEnumerable<EntityMetadata> entityMetadataList, List<string> prefixes)
		{
			var entities = new List<Extractor_EntityMetadata>();
			foreach (var e in entityMetadataList)
			{
				var fields = (e.Attributes ?? Enumerable.Empty<AttributeMetadata>())
					.Where(a =>
						(a.AttributeType != AttributeTypeCode.Virtual || (a.AttributeType == AttributeTypeCode.Virtual && a.AttributeTypeName?.Value == "MultiSelectPicklistType"))
				).ToList();
				entities.Add(BuildEntityMetadata(e, fields, prefixes));
			}
			return entities;
		}

		public static List<Extractor_EntityMetadata> ExtractEntitiesBySolution(IEnumerable<EntityMetadata> entityMetadataList, List<Guid> entityIds, List<string> prefixes)
		{
			var entities = new List<Extractor_EntityMetadata>();
			foreach (var e in entityMetadataList.Where(e => entityIds.Contains(e.MetadataId.GetValueOrDefault())))
			{
				var fields = (e.Attributes ?? Enumerable.Empty<AttributeMetadata>())
					.Where(a =>
						(a.AttributeType != AttributeTypeCode.Virtual || (a.AttributeType == AttributeTypeCode.Virtual && a.AttributeTypeName?.Value == "MultiSelectPicklistType"))
				).ToList();
				entities.Add(BuildEntityMetadata(e, fields, prefixes));
			}
			return entities;
		}

		public static Extractor_EntityMetadata? ExtractEntityByName(IEnumerable<EntityMetadata> entityMetadataList, string tableName, List<string> prefixes)
		{
			var e = entityMetadataList.FirstOrDefault(e => e.LogicalName == tableName);
			if (e == null) return null;
			var fields = (e.Attributes ?? Enumerable.Empty<AttributeMetadata>())
				.Where(a =>
					(a.AttributeType != AttributeTypeCode.Virtual || (a.AttributeType == AttributeTypeCode.Virtual && a.AttributeTypeName?.Value == "MultiSelectPicklistType"))
			).ToList();
			return BuildEntityMetadata(e, fields, prefixes);
		}
	}
}
