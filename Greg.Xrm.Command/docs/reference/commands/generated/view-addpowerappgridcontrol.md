# view addPowerappGridControl

(Preview) Adds a Power Apps grid control to a given view.

## Usage

```powershell
pacx view addPowerappGridControl
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--table` | t | string? | False | The name of the table that contains the view. Required only if the view name is not unique in the system. |
| `--type` | q | QueryType1 | False | The type of query. |
| `--force` | f | bool | False | Force the update of the control if a custom control is already set on the view. |
| `--accessibleLabel` | al | string? | False | The accessible label for the grid control. |
| `--enableEditing` | ee | bool | False | Enable editing functionality in the grid control. |
| `--disableChildItemsEditing` | dcie | bool | False | Disable editing of child items in the grid control. |
| `--enableFiltering` | ef | bool | False | Enable filtering functionality in the grid control. |
| `--enableSorting` | es | bool | False | Enable sorting functionality in the grid control. |
| `--enableGrouping` | eg | bool | False | Enable grouping functionality in the grid control. |
| `--enableAggregation` | ea | bool | False | Enable aggregation functionality in the grid control. |
| `--enableColumnMoving` | ecm | bool | False | Enable column moving functionality in the grid control. |
| `--enableMultipleSelection` | ems | bool | False | Enable multiple selection functionality in the grid control. |
| `--enableRangeSelection` | ers | bool | False | Enable range selection functionality in the grid control. |
| `--enableJumpBar` | ejb | bool | False | Enable jump bar functionality in the grid control. |
| `--enablePagination` | ep | bool | False | Enable pagination functionality in the grid control. |
| `--enableDropdownColor` | edc | bool | False | Enable dropdown color functionality in the grid control. |
| `--enableStatusIcons` | esi | bool | False | Enable status icons functionality in the grid control. |
| `--enableTypeIcons` | eti | bool | False | Enable type icons functionality in the grid control. |
| `--navigationTypesAllowed` | nav | NavigationTypes | False | The types of navigation allowed in the grid control. |
| `--reflowBehavior` | ref | Reflow | False | The reflow behavior of the grid control. |
| `--showAvatar` | avatar | bool | False | Show avatar in the grid control. |
| `--numberOfListColumns` | cols | int | False | The number of list columns in the grid control. |
| `--contextualFilters` | cf | bool | False | Enable contextual lookup column filters in the grid control. |
| `--lookupFilterBeginsWith` | lfbw | bool | False | Filter lookup suggestions from their starting letter. |
| `--useFirstColumnForLookupEdits` | ufcfle | bool | False | Use first column for lookup edits in the grid control. |
| `--customizerControl` | cc | string? | False | The full name of the grid customizer control. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Views/AddPowerAppGridControlCommand.cs`
