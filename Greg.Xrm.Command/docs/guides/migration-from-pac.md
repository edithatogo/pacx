# Migration From `pac`

PACX complements the Microsoft Power Platform CLI. Prefer `pac` for first-party platform operations and use PACX where this repository adds higher-level automation, validation, reporting, or developer workflow support.

## Mapping Guidance

| Scenario | `pac` starting point | PACX area |
| --- | --- | --- |
| Authentication | `pac auth` | `pacx auth` |
| Environment inventory | `pac env` | `pacx env` |
| Dataverse metadata automation | `pac solution`, `pac data` | `pacx table`, `pacx column`, `pacx solution` |
| Custom connector validation | Power Platform maker tools | `pacx connector validate` |
| AI Builder model operations | Maker portal | `pacx ai model` |

Keep existing `pac` scripts where they already cover the workflow cleanly. Migrate to PACX when you need repository-specific validation, richer output, or orchestration behavior.
