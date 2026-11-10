# Telegram boss key

Preview 0.1.0 · Windows 11 x64 · PowerToys 0.102.3101.0

Press **Alt+X**. Foreground/interactable → hide; background/covered → restore and foreground; closed → launch. The application must already be installed. No separate resident helper is created.



[ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossTelegram-0.1.0-win-x64.zip) · [Project / 完整说明](https://github.com/JoeChie/powertoys-boss-and-ask)

Quit PowerToys. Extract the ZIP. Run the included `Install.ps1`, or copy the `BossTelegram` folder into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`. Configure `settings.ini`, restart PowerToys, then enable the plugin.

Set `AppPath` to your installed executable if Start-menu discovery does not find it.

```powershell
.\Install.ps1
# Restore windows and exit PowerToys before uninstalling.
.\Uninstall.ps1 -Plugin BossTelegram
```

Old pilot plugins and other mappings using the same hotkey must be disabled first. See the project README for conflicts, local draft privacy, limits, building and verification.
