# column create

(DEPRECATED. Use specialized `column add <type>` commands instead) Creates a new column on a given Dataverse table

## Usage

```powershell
pacx column create
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--solution` | s | string? | False | The name of the unmanaged solution to which you want to add this attribute. |
| `--schemaName` | sn | string? | False | The schema name of the attribute.\nIf not specified, is deducted from the display name |
| `--description` | d | string? | False | The description of the attribute. |
| `--type` | at | SupportedAttributeType | False | The type of the attribute. |
| `--stringFormat` | sf | StringFormat | False | The format of the string attribute (default: Text). |
| `--memoFormat` | mf | MemoFormatName1 | False | The format of the memo attribute (default: Text). |
| `--intFormat` | if | IntegerFormat | False | For whole number type columns indicates the integer format for the column.(default: None) |
| `--requiredLevel` | r | AttributeRequiredLevel | False | The required level of the attribute. |
| `--len` | l | int? | False | The maximum length for string attribute. |
| `--autoNumber` | an | string? | False | In case of autonumber field, the autonumber format to apply. |
| `--audit` | a | bool | False | Indicates whether the attribute is enabled for auditing (default: true). |
| `--options` | o | string? | False | The list of options for the attribute, as a single string separated by comma (,) or semicolon (;) or pipe.\nYou can pass also values separating using syntax \ |
| `--globalOptionSetName` | gon | string? | False | For Picklist type columns that must be tied to a global option set,\nprovides the name of the global option set. |
| `--multiselect` | m | bool | False | Indicates whether the attribute is a multi-select picklist (default: false). |
| `--min` | min | double? | False | For number type columns indicates the minimum value for the column. |
| `--max` | max | double? | False | For number type columns indicates the maximum value for the column. |
| `--precision` | p | int? | False | For money or decimal type columns indicates the precision for the column. |
| `--precisionSource` | ps | int? | False | For money type columns indicates if precision should be taken from:\n(0) the precision property,\n(1) the `Organization.PricingDecimalPrecision` attribute or\n(2) the `TransactionCurrency.CurrencyPrecision` property of the transaction currency that is associated the current record.\n |
| `--imeMode` | ime | ImeMode | False | For number type columns indicates the input method editor (IME) mode for the column. |
| `--dateTimeBehavior` | dtb | DateTimeBehavior1 | False | For DateTime type columns indicates the DateTimeBehavior of the column. |
| `--dateTimeFormat` | dtf | DateTimeFormat | False | For DateTime type columns indicates the DateTimeFormat of the column. |
| `--trueLabel` | tl | string? | False | For Boolean type columns that represents the Label to be associated to the \ |
| `--falseLabel` | fl | string? | False | For  Boolean type columns that represents the Label to be associated to the \ |
| `--defaultValue` | dv | string? | False | For Picklist type columns indicates the default value for the column. You can provide the name or the value. If not provided, is automatically evaluated by the system. |
| `--maxSizeInKB` | maxKb | int? | False | For File or Image type columns indicates the maximum size in KB for the column. Do not provide a value if you want to stay with the default (32Mb for file columns, 10Mb for image columns). The value must be lower than 10485760 (1Gb) for file columns, and lower than 30720 (30Mb) for image columns . |
| `--storeOnlyThumbnailImage` | thumb | bool? | False | For Image type columns indicates if the column stores only thumbnail-sized images. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/CreateCommand.cs`

