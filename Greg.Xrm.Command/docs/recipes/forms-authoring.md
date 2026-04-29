# Forms Authoring

Use the authoring commands to create and manage a source-controlled Microsoft Forms manifest.

```powershell
pacx forms create -o forms.json -t "Customer survey"
pacx forms update -f forms.json --published true
pacx forms publish -f forms.json --published false
pacx forms delete -f forms.json --force
pacx forms inspect -f forms.json
pacx forms question list -f forms.json
pacx forms question add -f forms.json -t "What is your name?"
pacx forms question update -f forms.json --id question_1 -t "Updated question"
pacx forms question delete -f forms.json --id question_1 --force
pacx forms section list -f forms.json
pacx forms section add -f forms.json -t "Section 1"
pacx forms section update -f forms.json --id section_1 --order 1
pacx forms section delete -f forms.json --id section_1 --force
```

These commands operate on the local manifest file. They do not call the live Microsoft Forms API yet.
