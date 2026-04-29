# Advanced Microsoft Forms

Use the advanced Forms commands for branching, analytics, templates, sharing, and Customer Voice workflows.

```powershell
pacx forms branching export --form-id <form-id>
pacx forms analytics summary --form-id <form-id>
pacx forms analytics timeseries --form-id <form-id> --bucket week
pacx forms template create --from-form <form-id> --scope org
pacx forms share --form-id <form-id> --with-group <group-id> --role collaborate
pacx forms cv list
```
