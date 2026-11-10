# v0.1.0 — PowerToys Boss & Ask / 七个独立插件预览版

Windows 11 x64 · built against PowerToys 0.102.3101.0 / bundled .NET 10. Other versions and ARM64 are unverified.

## Downloads / 按需下载

| Asset / 安装包 | Purpose / 用途 |
|---|---|
| BossCodex-0.1.0-win-x64.zip | Alt+E · Codex |
| BossDeepSeek-0.1.0-win-x64.zip | Alt+F · DeepSeek Harness |
| BossTelegram-0.1.0-win-x64.zip | Alt+X · Telegram |
| BossPhoneLink-0.1.0-win-x64.zip | Alt+C · Phone Link / 手机连接 |
| AskCodex-0.1.0-win-x64.zip | Alt+Space → gpt question / 问题 |
| AskWorkBuddy-0.1.0-win-x64.zip | Alt+Space → wb question / 问题 |
| AskDeepSeek-0.1.0-win-x64.zip | Alt+Space → ds question / 问题 |

Each ZIP includes one independent plugin, bilingual guides, MIT license and optional installation/uninstallation scripts. `SHA256SUMS.txt` contains SHA-256 checksums. The automatic GitHub source archives contain the research source; choose the named ZIP assets above for ready-built plugins.

每个 ZIP 只包含一个插件，附中英文说明、MIT 许可证和安装／卸载脚本。需要直接安装时下载上述命名 ZIP；GitHub 自动生成的 Source code 压缩包是源码。先核对校验值。

## Behavior / 功能

Boss: foreground and interactable → hide; covered/background → restore the same window and bring forward; closed → start the app. These plugins run inside PowerToys Run; no separate resident helper is installed. Tray availability depends on the destination app; Phone Link uses minimize plus taskbar removal.

老板键按实际窗口状态决定收起／前置，退出后可启动应用；运行于 PowerToys Run 内部，没有独立常驻助手。托盘图标取决于应用本身；手机连接采用最小化并移除任务栏按钮。

Ask: choose a question result and press Enter to fill an **unsent draft**. Tab / Shift+Tab keep the original query. WorkBuddy and Codex use native URI delivery; DeepSeek uses version-sensitive UI Automation and Unicode input. WorkBuddy accepts up to 8,000 characters. DeepSeek supports single-line questions and keeps replaced draft backups locally by default.

三个提问入口都只填草稿，不自动发送；Tab／Shift+Tab 保留原问题。DeepSeek 默认备份被替换的旧草稿，请勿把本地 `draft-backups/` 上传公开 Issue。

## Installation / 安装

Restore hidden windows and exit PowerToys. Extract one ZIP, inspect `Install.ps1`, then run it; or copy its named module folder into `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`. Check `settings.ini` for application paths / packaged app identities / hotkeys. Restart PowerToys and enable the module under PowerToys Run → Plugins.

先恢复隐藏窗口，再退出 PowerToys。解压并按包内说明安装；检查 `AppPath`／`AppId`／`Hotkey` 后重启 PowerToys 并启用对应插件。已有个人旧版 Boss 或其他同键映射时，请先停用，避免冲突。

## Validation / 验证范围

Seven production builds, configuration/query contract tests, isolated installation/reinstall/backup/uninstall tests, PowerShell syntax, archive allowlists and SHA-256 checks passed locally. Original pilot behavior was manually tested before portable extraction; these public portable modules still need real-window checks on each target machine. Artifacts are unsigned. This is an independent community preview, not an official PowerToys utility.

[English guide](https://github.com/JoeChie/powertoys-boss-and-ask/blob/main/README.md) · [中文说明](https://github.com/JoeChie/powertoys-boss-and-ask/blob/main/README.zh-CN.md)
