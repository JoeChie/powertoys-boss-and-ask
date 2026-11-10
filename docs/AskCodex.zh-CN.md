# 问问 Codex

预览版 0.1.0 · Windows 11 x64 · PowerToys 0.102.3101.0

按 **Alt+Space** 输入 `gpt 你的问题` 后回车：仅填入草稿，不自动发送；在三个提问入口之间 Tab／Shift+Tab 切换保留原文。



[ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/AskCodex-0.1.0-win-x64.zip) · [Project / 完整说明](https://github.com/JoeChie/powertoys-boss-and-ask)

先退出 PowerToys，解压 ZIP。运行包内 `Install.ps1`，或将 `AskCodex` 文件夹复制到 `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`。检查 `settings.ini`，重启 PowerToys 并启用插件。

```powershell
.\Install.ps1
# Restore windows and exit PowerToys before uninstalling.
.\Uninstall.ps1 -Plugin AskCodex
```

请先停用旧试用版或占用相同热键的其他映射。完整项目说明含冲突排查、本地草稿隐私、适用边界、构建和验证方式。
