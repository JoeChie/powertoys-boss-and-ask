# PowerToys Boss & Ask

[English](README.md) · [简体中文](README.zh-CN.md)

七个可独立下载、按需安装的 **PowerToys Run 社区插件**：四个老板键，以及三个本地 AI 应用提问草稿入口。功能运行在 PowerToys Run 进程内部，不增加独立常驻助手或计划任务。

**预览版 0.1.0；Windows 11 x64；使用 PowerToys 0.102.3101.0 自带 .NET 10 编译。** 尚未验证其他版本、ARM64 或 Windows 10。它们不是命令面板扩展，也不会添加到 PowerToys 原生“实用程序”卡片列表。

## 按需下载

| 独立插件 | 快捷键／关键词 | 下载 |
|---|---|---|
| [Codex 老板键](docs/BossCodex.zh-CN.md) | `Alt+E` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossCodex-0.1.0-win-x64.zip) |
| [DeepSeek Harness 老板键](docs/BossDeepSeek.zh-CN.md) | `Alt+F` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossDeepSeek-0.1.0-win-x64.zip) |
| [Telegram 老板键](docs/BossTelegram.zh-CN.md) | `Alt+X` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossTelegram-0.1.0-win-x64.zip) |
| [手机连接老板键](docs/BossPhoneLink.zh-CN.md) | `Alt+C` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossPhoneLink-0.1.0-win-x64.zip) |
| [问问 Codex](docs/AskCodex.zh-CN.md) | `Alt+Space → gpt question` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskCodex-0.1.0-win-x64.zip) |
| [问问 WorkBuddy](docs/AskWorkBuddy.zh-CN.md) | `Alt+Space → wb question` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskWorkBuddy-0.1.0-win-x64.zip) |
| [问问 DeepSeek Harness](docs/AskDeepSeek.zh-CN.md) | `Alt+Space → ds question` | [ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskDeepSeek-0.1.0-win-x64.zip) |


[全部发布版本](https://github.com/JoeChie/powertoys-boss-and-ask/releases) · [SHA-256 校验值](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/SHA256SUMS.txt)

## 四个老板键

- 应用在前台、启用、限时响应检查通过且五个内部抽样点未被遮挡：收起窗口并移除任务栏按钮。
- 应用在后台、被其他程序挡住、已隐藏或最小化：恢复同一个窗口，尝试带到前台。
- 应用完全退出、找不到主窗口：启动应用。

松开字母键后触发，不必松开 Alt。Codex 恢复后在 Alt 松开时尝试聚焦唯一支持的聊天输入框；恢复期间主动切到其他窗口会取消旧聚焦请求。抽样检查不能证明所有界面区域都正常。

Codex、Telegram、DeepSeek 隐藏窗口；手机连接使用最小化并移除任务栏按钮。插件不会凭空创建应用托盘图标，托盘行为取决于应用本身。关闭／卸载插件或强制结束 PowerToys 前，先恢复隐藏窗口。

## 三个“问问”入口

按 **Alt+Space** 输入问题，选择“问问 Codex／WorkBuddy／DeepSeek Harness”并回车。也可输入 `gpt 问题`、`wb 问题`、`ds 问题` 直接定位。插件结果之间用 **Tab／Shift+Tab** 切换会保留原问题。

- **Codex**：使用 `codex://new?prompt=…` 创建问题草稿，并保留应用冷启动兜底。
- **WorkBuddy**：使用其内置 `workbuddy://task?action=start&prompt=…` 草稿接口。测试版本中的 `action=start` 只设置草稿，不发送；部分版本遇到现有草稿可能询问是否替换。最多 8000 字符，超长明确拒绝，不悄悄截断。
- **DeepSeek Harness**：测试版本没有对应草稿接口，因此进入新会话，定位唯一支持的编辑框后输入，并核对结果。复用未发送草稿时，默认先保存本地备份。仅支持单行问题；界面变更、多个候选输入框或焦点变化可能使操作安全中止。

插件不需要 API Key，但目标应用必须安装、登录，其正常订阅／用量规则仍然适用。**所有入口都只填草稿，由你确认发送。**

## 安装

1. 安装 PowerToys 并启用 **PowerToys Run**。先恢复隐藏窗口，再从托盘菜单完全退出 PowerToys。
2. 下载需要的 ZIP，对照 `SHA256SUMS.txt` 检查文件，再解压。
3. 将里面以插件命名的文件夹复制到 `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`；或者先查看包内 `Install.ps1`，再用 PowerShell 运行。安装脚本会拒绝在 Run 运行时安装、备份已有同 ID 插件、保留原配置；不会覆盖完整 PowerToys 设置。
4. 检查插件内 `settings.ini`。Telegram／DeepSeek Harness／WorkBuddy 的 `AppPath` 指向真实 EXE；安装器可从开始菜单匹配到的唯一快捷方式自动发现路径，失败时自行填写，不会扫描所有磁盘。Codex／手机连接通过 `AppId` 设置应用身份。
5. 重开 PowerToys，在“PowerToys Run → 插件”启用对应插件。三个提问入口若需要响应普通问题，开启“包含在全局结果中”。

例如，在解压后的目录运行：

```powershell
.\Install.ps1 -AppPath 'C:\Apps\WorkBuddy\WorkBuddy.exe'
```

Codex／手机连接通常不需要 `AppPath`。手动安装只复制插件文件夹，不复制外层包装目录。按系统原有策略运行自己审阅过的脚本，不需要修改全系统执行策略。

## 自定义、冲突与卸载

每个插件各有 `settings.ini`。`Hotkey=E` 表示 Alt+E，可改 A–Z 的一个字母；留空／无效值停用注册，改完重启 PowerToys。Run 中输入 `bosse`／`bossf`／`bossx`／`bossc` 可打开相应配置。这是配置文件入口，不是 PowerToys 原生快捷键选择器。

若旧 Boss、AutoHotkey 或键盘管理器已经占用相同热键，请先停用旧映射。冲突不会被强行覆盖，注册失败信息显示在插件结果和 `status.txt`。公开版七个 ID 与个人试用版不同；已有旧版时应停用／移除旧版，避免重复结果和抢键。

卸载前先恢复窗口并退出 PowerToys，再运行 `Uninstall.ps1 -Plugin AskWorkBuddy`，替换为实际模块名。脚本先备份，再仅删除对应插件；也可手动移除其目录。其他插件及全局 Windows 映射不受影响。

## 隐私与适用边界

发布包不包含账号数据库、令牌、聊天历史、个人设置和调试日志；中性图标为原创。插件自身没有网络客户端或遥测，目标应用可能按其正常流程联网。通过 URI 传递的问题在本机启动参数中可见，并交给已安装应用。

`status.txt` 保存最近状态。`Diagnostics=false` 默认不写额外循环日志。DeepSeek 的 `BackupDrafts=true` 会把被替换的旧草稿保存到插件 `draft-backups/` 目录；这是本地隐私文件，不应未经检查上传到公开 Issue。设为 `false` 可关闭备份，但被替换草稿将不再有备份。

低权限 PowerToys 无法保证把高权限应用带到前台，也不能操作安全桌面或修复客户端内部的滚动／输入法缺陷。不要为了绕过问题长期以管理员身份运行所有应用。DeepSeek 中英文输入框名称、Codex ProseMirror 控件都依赖应用版本；存在多个编辑器时可能跳过。本项目不修改目标应用归档，不改 Windows 全局键位。

## 构建与研究

```powershell
.\scripts\Build.ps1 -PowerToysPath "$env:LOCALAPPDATA\PowerToys"
.\scripts\Test.ps1
.\scripts\Package.ps1
python .\tests\validate_release.py
```

PowerToys 装在其他位置时，指向包含 `Wox.Plugin.dll` 的目录。构建使用 Windows 自带 .NET Framework C# 编译器和 PowerToys 安装目录里的运行时引用，不另装 SDK；输出插件实际运行于 PowerToys 的 .NET 运行时，不是 .NET Framework。`release/` 生成七个 ZIP 和校验文件。每次打包使用新的 `staging/`／输出目录，脚本会拒绝覆盖已有发布包。

[架构与验证记录](docs/ARCHITECTURE.md) · [参与贡献](CONTRIBUTING.md) · [安全反馈](SECURITY.md) · [更新记录](CHANGELOG.md)

个人试用版行为曾由作者手动验证；公共便携模块会进行编译、源码约定及打包检查，仍需在每台目标机器实测。本预览版不承诺广泛兼容，也不宣称已测得每个插件的内存占用。

## 参考与许可

参考了 [PowerToys 官方第三方插件指南](https://github.com/microsoft/PowerToys/blob/main/doc/thirdPartyRunPlugins.md)、[Run 插件开发说明](https://github.com/microsoft/PowerToys/blob/main/doc/devdocs/modules/launcher/plugins/overview.md)、[8LWXpg 插件模板](https://github.com/8LWXpg/PowerToysRun-PluginTemplate) 及 [ruslanlap Hotkeys 项目](https://github.com/ruslanlap/PowerToysRun-Hotkeys) 的说明和发布组织方式，未打包其源码。也不分发 PowerToys DLL、应用本体或第三方品牌图标。

本项目代码及原创图标使用 MIT 许可证。PowerToys、Codex、Telegram、WorkBuddy、DeepSeek 名称归各自权利人所有；这是独立社区项目，非官方版本或背书。
