# Architecture / 架构与验证

## Modules

Each module has a unique `plugin.json` ID, one production DLL, an original icon and a per-module `settings.ini`. Four boss modules compile the same shared window controller into their own assemblies. They register their own Alt+letter hotkey on a background Win32 message thread inside PowerToys Run. They do not start an additional resident executable.

每个模块有独立 ID、DLL、原创图标和配置。四个老板键复用共享控制源码，但各自注册热键、各自启停；没有独立常驻 EXE。

- `Shared/Boss.cs`: window ranking, same-window restoration, foreground checks, taskbar integration and optional Codex focus after physical Alt release. No permanent topmost window, `AttachThreadInput`, synthetic Alt activation or keyboard-layout changes.
- `Shared/HotkeyHost.cs`: `RegisterHotKey(MOD_ALT | MOD_NOREPEAT)` and message loop. Unregisters on disposal; attempts to restore windows it hid.
- `Shared/CommunityConfig.cs`: portable configuration and executable discovery. A running process path or known default folders can be used; an explicit `AppPath` takes priority. Start-menu discovery is performed by the optional installer, not an ongoing drive scan.
- `AskCodex`: native draft URI and packaged-app cold start.
- `AskWorkBuddy`: native task draft-prefill URI. The tested parser's `action=start` does not send a prompt. New application versions must be verified again.
- `AskDeepSeek`: UI Automation with strict process/window/editor selection and one batch of Unicode input after focus and modifier checks. No submit key is injected. Draft replacement is permitted only in the recognized new-conversation composer, with local backups by default.

三个提问插件均显式设置 `QueryTextDisplay=query.RawQuery`，避免 Tab／Shift+Tab 用结果名称覆盖原问题。公开版不包含日语助手、Windows 键位映射、旧常驻程序或个人 Boss 设置窗体。

## Compatibility

Initial artifacts were compiled against PowerToys 0.102.3101.0's installed runtime on Windows 11 x64. The compiler is .NET Framework's `csc.exe`, but `/nostdlib` and explicit runtime references produce a plugin for the bundled PowerToys runtime. The test-only shim builds a separate executable for configuration/query contracts; it is never shipped as a plugin dependency.

公共版剥离了个人路径并提供通用安装器。与个人试用版相比，新增了可移植配置、独立 ID、退出时尝试恢复隐藏窗口和发布检查；它不是已在所有电脑实测的二进制复制品。

## Automated checks

`scripts/Test.ps1` checks empty/global/keyword questions, original-query preservation including quotes, ampersands, Japanese and emoji, hotkey configuration, reset-to-default behavior, backup opt-out and WorkBuddy's length limit. It parses PowerShell scripts for syntax errors. It does not invoke hotkeys, launch target apps or send messages.

`tests/validate_release.py` checks seven unique IDs, production DLL manifest names, portable default settings, package allowlists, and SHA-256 checksums. Public source is scanned for obvious machine-specific paths and credential patterns. These checks are not a complete security audit.

## Manual acceptance matrix for each machine

| Check / 检查 | Expected / 预期 |
|---|---|
| Boss: foreground then press key | Window hides; taskbar button removed |
| Boss: browser/QQ/WeChat covers target | Same target returns to foreground and accepts input |
| Boss: hidden or minimized | Same window and current conversation restored |
| Boss: app completely exited | Installed app starts |
| Codex: release Alt after restoration | One supported composer receives focus; no extra letter |
| Ask: ordinary query and keyword | Correct local app receives an unsent draft |
| Ask: second distinct question without sending first | New question appears; app draft protection may apply |
| Ask: Tab then Shift+Tab | Original text survives switching results |
| Ask: cold start | App opens and receives draft |
| Disable / uninstall | Hotkey released; restore hidden windows first |
| Conflict | Registration failure reported, existing mapping not overwritten |

The personal pilot passed its author's real-window checks in this chat. Public portable artifacts receive automated checks here; target-machine smoke testing remains required. First-letter IME behavior, taskbar-induced scroll jumps and application UI health are not fixed or guaranteed by this project.
