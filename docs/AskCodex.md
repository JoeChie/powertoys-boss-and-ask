# Ask Codex

Preview 0.1.0 · Windows 11 x64 · PowerToys 0.102.3101.0

Press **Alt+Space**, type `gpt your question`, then press Enter. The question is prepared as a draft, never automatically submitted. Tab / Shift+Tab preserve the original query among the three Ask entries.



[ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskCodex-0.1.0-win-x64.zip) · [Project / 完整说明](https://github.com/JoeChie/powertoys-boss-and-ask)

Quit PowerToys. Extract the ZIP. Run the included `Install.ps1`, or copy the `AskCodex` folder into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`. Configure `settings.ini`, restart PowerToys, then enable the plugin.

```powershell
.\Install.ps1
# Restore windows and exit PowerToys before uninstalling.
.\Uninstall.ps1 -Plugin AskCodex
```

Old pilot plugins and other mappings using the same hotkey must be disabled first. See the project README for conflicts, local draft privacy, limits, building and verification.
