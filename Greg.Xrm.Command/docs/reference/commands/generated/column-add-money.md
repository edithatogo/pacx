# column add money

Creates a money column.

## Usage

```powershell
pacx column add money
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--min` | min | double? | False | For number type columns indicates the minimum value for the column. |
| `--max` | max | double? | False | For number type columns indicates the maximum value for the column. |
| `--precision` | p | int? | False | For money or decimal type columns indicates the precision for the column. |
| `--precisionSource` | ps | int? | False | Indicates if precision should be taken from:\n(0) the precision property,\n(1) the `Organization.PricingDecimalPrecision` attribute or\n(2) the `TransactionCurrency.CurrencyPrecision` property of the transaction currency that is associated the current record.\n |
| `--imeMode` | ime | ImeMode | False | Indicates the input method editor (IME) mode for the column. |

## Source

`Greg.Xrm.Command/Greg.Xrm.Command.Core/Commands/Column/Create/CreateMoneyCommand.cs`
