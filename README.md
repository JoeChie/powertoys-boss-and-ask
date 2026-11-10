# PowerToys Boss & Ask

[English](README.md) · [简体中文](README.zh-CN.md)

Seven independent **PowerToys Run** community plugins: four application boss keys and three local AI question-draft launchers. Install only the modules you need. The plugins run inside `PowerToys.PowerLauncher.exe`; they do not add a separate resident helper or scheduled task.

**Preview release 0.1.0 · Windows 11 x64 · built against PowerToys 0.102.3101.0 / bundled .NET 10.** Other PowerToys versions, ARM64 and Windows 10 have not been validated. This is not a Command Palette extension or a built-in PowerToys Utilities card.

## Choose a download

| Package | Shortcut / keyword | Download |
|---|---|---|
| [Codex boss key](docs/BossCodex.md) | `Alt+E` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossCodex-0.1.0-win-x64.zip) |
| [DeepSeek Harness boss key](docs/BossDeepSeek.md) | `Alt+F` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossDeepSeek-0.1.0-win-x64.zip) |
| [Telegram boss key](docs/BossTelegram.md) | `Alt+X` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossTelegram-0.1.0-win-x64.zip) |
| [Phone Link boss key](docs/BossPhoneLink.md) | `Alt+C` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossPhoneLink-0.1.0-win-x64.zip) |
| [Ask Codex](docs/AskCodex.md) | `Alt+Space → gpt question` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskCodex-0.1.0-win-x64.zip) |
| [Ask WorkBuddy](docs/AskWorkBuddy.md) | `Alt+Space → wb question` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskWorkBuddy-0.1.0-win-x64.zip) |
| [Ask DeepSeek Harness](docs/AskDeepSeek.md) | `Alt+Space → ds question` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskDeepSeek-0.1.0-win-x64.zip) |


[All releases](https://github.com/JoeChie/powertoys-boss-and-ask/releases) · [Checksums](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/SHA256SUMS.txt)

## What the boss keys do

1. If the target is the foreground window, enabled, responsive to a bounded probe, and not covered at five sampled interior points: hide it and remove its taskbar button.
2. If it is behind another app, hidden or minimized: restore the same window and try to bring it to the foreground.
3. If it has no main window: start the installed application.

The action is triggered after the letter is released; you do not have to release Alt. Codex tries to focus its single supported message editor after Alt is released. Foreground recovery respects a user switching elsewhere during recovery. Foreground eligibility and occlusion checks are best-effort Windows behavior, not proof that every part of an app is healthy.

Codex / Telegram / DeepSeek use hidden-window behavior; Phone Link uses minimize plus taskbar removal. A new tray icon is **not** created by these plugins. An app's own tray behavior remains app-dependent. Restore windows before disabling, uninstalling or force-terminating PowerToys.

## What Ask does

Press **Alt+Space**, type a question, choose **Ask Codex**, **Ask WorkBuddy** or **Ask DeepSeek Harness**, then press Enter. Dedicated keywords are `gpt`, `wb`, and `ds`. Global search can be enabled per plugin. **Tab / Shift+Tab preserve the full original query** when switching these results.

- **Codex:** uses `codex://new?prompt=…`, with packaged-app cold-start fallback.
- **WorkBuddy:** uses its installed native `workbuddy://task?action=start&prompt=…` draft-prefill handler. Despite its protocol parameter name, the tested handler sets draft blocks; it does not submit a message. It may ask before replacing a user draft in some app versions. Maximum 8,000 characters; oversized questions are rejected rather than truncated.
- **DeepSeek Harness:** has no equivalent draft URI in the tested build. It opens a new conversation, focuses the unique supported editor, inputs text, and checks the resulting value. Reused unsent drafts are backed up locally before replacement by default. Only single-line questions are supported. UI changes can break this integration; ambiguous controls or focus changes abort it.

No API key is needed by these plugins. You still need the destination app installed and signed in, and its normal subscription or usage rules apply. **Nothing is automatically sent by these plugins.** Sending remains your action in the target app.

## Install

1. Install PowerToys and enable **PowerToys Run**. Restore any hidden target windows, then quit PowerToys from its tray menu.
2. Download one ZIP from the table, check its SHA-256 against `SHA256SUMS.txt`, and extract it.
3. Either copy its named plugin folder into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`, or inspect and run the included `Install.ps1` in PowerShell. The script refuses to install while Run is running, backs up an existing same-ID installation, preserves its configuration, and never replaces the whole PowerToys settings file.
4. Check the plugin's `settings.ini`. `AppPath` must point to the installed Telegram / DeepSeek Harness / WorkBuddy executable when auto-detection is insufficient. `AppId` configures Codex / Phone Link's packaged identity. The installer can discover an unambiguous matching Start-menu shortcut; it does not recursively search your drives.
5. Restart PowerToys. Enable the plugin in **PowerToys → PowerToys Run → Plugins**. Enable **Include in global result** for Ask plugins if you want ordinary questions to show all three.

Example from an extracted ZIP:

```powershell
.\Install.ps1 -AppPath 'C:\Apps\WorkBuddy\WorkBuddy.exe'
```

AppPath is optional for Codex and Phone Link. You may use a different absolute path. For manual installation, copy only the module folder, not the outer ZIP wrapper. Allow only scripts you have reviewed under your normal Windows policy; no system-wide execution-policy change is required.

## Configuration, conflicts and uninstall

`settings.ini` is per plugin. `Hotkey=E` means Alt+E; an empty or invalid value disables registration. Change it to another letter A–Z and restart PowerToys. Enter `bosse`, `bossf`, `bossx` or `bossc` in Run to open that module's settings. These are simple configuration files, not native PowerToys shortcut-picker controls.

Disable old Boss / AutoHotkey / Keyboard Manager mappings using the same keys before enabling a new boss module. Registration failures are shown in the module's Run result and `status.txt`; a conflict is not silently stolen. The seven public IDs differ from the original personal pilot plugins, so disabling/removing the old pilot is essential to avoid duplicate results and hotkey collisions.

Before uninstalling, restore the app, quit PowerToys, and run `Uninstall.ps1 -Plugin AskWorkBuddy` (substitute the module name). This backs up the directory before removing only that module. Or remove its directory manually while PowerToys is closed. Existing global Windows mappings and unrelated plugins are untouched.

## Privacy and limitations

No account databases, tokens, chat history, personal configurations or diagnostic logs are distributed. The neutral package icons are original assets. The plugin itself has no network client or telemetry; the destination apps may connect to their own services. URI question text is visible locally in a process launch/URI and is handed to the installed app.

`status.txt` contains the last operation status. `Diagnostics=false` disables the additional rolling diagnostic log by default. DeepSeek's `BackupDrafts=true` stores replaced draft text in that module's `draft-backups/` folder. These are local private files: do not attach them to public issues without reviewing them. Set `BackupDrafts=false` to opt out, understanding that reused drafts will then be replaced without a backup.

Plugins cannot reliably foreground higher-privilege applications from a lower-privilege PowerToys process, interact with the secure desktop, or fix app-internal scroll/IME bugs. Avoid permanently elevating applications just to work around this. DeepSeek's English/Chinese editor names and Codex's ProseMirror editor are version-sensitive. Multiple editors can cause a safe skip. The package does not patch app archives or alter Windows key mappings.

## Build and research

```powershell
.\scripts\Build.ps1 -PowerToysPath "$env:LOCALAPPDATA\PowerToys"
.\scripts\Test.ps1
.\scripts\Package.ps1
python .\tests\validate_release.py
```

Provide the actual PowerToys installation directory containing `Wox.Plugin.dll` if it is different. Build uses Windows' .NET Framework C# compiler with the installed PowerToys runtime references; a separate .NET SDK is not required. The resulting plugins target that bundled runtime, **not .NET Framework**. Seven ZIPs and checksums are written to `release/`. Use a fresh `staging/` and release output for each packaging run; the script refuses to overwrite release artifacts.

[Architecture and testing](docs/ARCHITECTURE.md) · [Contributing](CONTRIBUTING.md) · [Security](SECURITY.md) · [Release notes](CHANGELOG.md)

The private pilot behavior was manually tested by its author before extraction. Public portable modules have build, source-contract and package checks; they must be rechecked on each target machine. This preview does not claim broad compatibility or measured per-plugin memory figures.

## References and license

Documentation and packaging structure were informed by the [official third-party Run plugin guide](https://github.com/microsoft/PowerToys/blob/main/doc/thirdPartyRunPlugins.md), [PowerToys Run plugin overview](https://github.com/microsoft/PowerToys/blob/main/doc/devdocs/modules/launcher/plugins/overview.md), [8LWXpg's plugin template](https://github.com/8LWXpg/PowerToysRun-PluginTemplate), and [ruslanlap's Hotkeys plugin](https://github.com/ruslanlap/PowerToysRun-Hotkeys). Their source code is not vendored here. `Wox.Plugin.dll`, application binaries and third-party brand logos are not redistributed.

MIT license for this project's code and original assets. PowerToys, Codex, Telegram, WorkBuddy and DeepSeek names belong to their respective owners. This is an independent community project, not an official release or endorsement.
