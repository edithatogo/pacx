# Dataverse Skill Pack Workflow

Use this workflow when you need to prepare, validate, or inspect a Dataverse environment before making changes.

```powershell
pacx auth whoami
pacx org info
pacx solution list
pacx validate all
```

If you are working on schema or connector changes, add targeted validation:

```powershell
pacx connector validate --file connector.json --strict
pacx package validate --file package.json
pacx table exportMetadata --table account
```

Use the pack as a checklist, not as a shortcut around review. It is meant to make the common path repeatable and to leave room for stricter environment-specific rules.
