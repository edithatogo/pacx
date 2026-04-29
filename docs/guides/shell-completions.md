# Shell Completions

`pacx completions <shell>` prints completion scripts generated from the registered command metadata.

## PowerShell

Load for the current session:

```powershell
pacx completions pwsh | Out-String | Invoke-Expression
```

Persist by adding that line to `$PROFILE`.

## Bash

```bash
pacx completions bash > ~/.local/share/bash-completion/completions/pacx
```

Restart the shell or source the generated file.

## Zsh

```zsh
mkdir -p ~/.zfunc
pacx completions zsh > ~/.zfunc/_pacx
```

Add `fpath=(~/.zfunc $fpath)` and `autoload -Uz compinit && compinit` to `~/.zshrc` if not already configured.

## Fish

```fish
pacx completions fish > ~/.config/fish/completions/pacx.fish
```
