# Exit Codes

`pacx` uses stable process exit codes for automation.

| Code | Name | Meaning |
| ---: | --- | --- |
| 0 | Success | Command completed successfully. |
| 1 | UsageError | Invalid command, option, argument, or invocation shape. |
| 2 | AuthError | Authentication or profile selection failed. |
| 3 | ApiError | Dataverse, Power Platform, or service API returned a failure. |
| 4 | ValidationError | Command options parsed but failed validation rules. |
| 5 | NetworkError | HTTP, timeout, cancellation, or connectivity failure. |
| 64 | InternalError | Unexpected PACX implementation/runtime error. |
| 130 | Cancelled | User cancelled with Ctrl+C. |

Use these codes in CI scripts instead of parsing console text.
