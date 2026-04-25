# Security Policy

PACX handles Dataverse environments and can touch production data, so security reports are treated as high priority.

## Reporting a vulnerability

Use GitHub private vulnerability reporting through the repository `Security` tab whenever possible. Do not open a public issue or pull request for a security problem.

Include, if you can:

- the affected command or workflow
- the PACX version or commit
- reproduction steps
- the expected impact
- whether production data or credentials could be exposed

We aim to acknowledge reports within 2 business days and work toward a coordinated disclosure window of up to 90 days, unless an earlier fix or a shorter disclosure window is required.

## Disclosure preferences

- Prefer private GitHub Security Advisories for all security reports.
- If you need an encrypted channel, request one in the advisory thread.
- No public disclosure before a fix is available unless the reporter and maintainers agree otherwise.

## Dependency updates

- Renovate is the primary version-update bot for this repository.
- Dependabot alerts and security updates are handled at the GitHub repository level only.
- There is intentionally no `.github/dependabot.yml` in this repository, so Dependabot does not create routine version-bump pull requests.
- Keep Dependabot notifications on security alerts only so routine version bumps stay out of your inbox.

## Supported releases

Security fixes are maintained for the current mainline release and the latest published version.

## Secret handling

- PACX does not persist Dataverse access tokens to its own on-disk cache.
- Access tokens are kept in memory for the lifetime of the process, and log output redacts bearer tokens before they are written.
