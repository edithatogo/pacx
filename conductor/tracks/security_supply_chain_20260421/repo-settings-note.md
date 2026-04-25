# Repo Settings Note

These items are intentionally left for GitHub repository settings because they cannot be enforced from the working tree alone.

## Security

- Enable private vulnerability reporting in the repository `Security` tab.
- Keep Dependabot alerts enabled.
- Keep Dependabot security updates enabled.
- Leave Dependabot version-update PRs disabled.
- Keep notification preferences on security alerts only.

## Branch protection

- Require CI checks on `master`.
- Require at least one review before merge.
- Disallow force pushes.
- Disallow direct pushes.
- Prefer linear history.
- Prefer signed commits.

## Ownership

- Keep `CODEOWNERS` active for the command, workflow, and conductor paths.
- Review the default owner entries if the maintainer list changes.
