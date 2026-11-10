# Ask DeepSeek Harness

Preview 0.1.0 · Windows 11 x64 · PowerToys 0.102.3101.0

Press **Alt+Space**, type `ds your question`, then press Enter. The question is prepared as a draft, never automatically submitted. Tab / Shift+Tab preserve the original query among the three Ask entries.

Single-line questions only. UI Automation plus Unicode input. Replaced unsent drafts are saved locally in `draft-backups/` unless `BackupDrafts=false`. Stop if the editor is ambiguous or focus changes.

[ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskDeepSeek-0.1.0-win-x64.zip) · [Project / 完整说明](https://github.com/JoeChie/powertoys-boss-and-ask)

Quit PowerToys. Extract the ZIP. Run the included `Install.ps1`, or copy the `AskDeepSeek` folder into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`. Configure `settings.ini`, restart PowerToys, then enable the plugin.

Set `AppPath` to your installed executable if Start-menu discovery does not find it.

```powershell
.\Install.ps1
# Restore windows and exit PowerToys before uninstalling.
.\Uninstall.ps1 -Plugin AskDeepSeek
```

Old pilot plugins and other mappings using the same hotkey must be disabled first. See the project README for conflicts, local draft privacy, limits, building and verification.
