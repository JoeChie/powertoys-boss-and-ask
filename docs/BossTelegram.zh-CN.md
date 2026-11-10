# Telegram 老板键

预览版 0.1.0 · Windows 11 x64 · PowerToys 0.102.3101.0

按 **Alt+X**：前台可操作时收起；后台／被遮挡时恢复并前置；退出后启动。需要先安装目标应用，不增加独立常驻助手。



[ZIP](https://github.com/JoeChie/powertoys-boss-and-ask/releases/download/v0.1.0/BossTelegram-0.1.0-win-x64.zip) · [Project / 完整说明](https://github.com/JoeChie/powertoys-boss-and-ask)

先退出 PowerToys，解压 ZIP。运行包内 `Install.ps1`，或将 `BossTelegram` 文件夹复制到 `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`。检查 `settings.ini`，重启 PowerToys 并启用插件。

若开始菜单发现失败，请把 `AppPath` 填成已安装应用的真实 EXE 路径。

```powershell
.\Install.ps1
# Restore windows and exit PowerToys before uninstalling.
.\Uninstall.ps1 -Plugin BossTelegram
```

请先停用旧试用版或占用相同热键的其他映射。完整项目说明含冲突排查、本地草稿隐私、适用边界、构建和验证方式。
