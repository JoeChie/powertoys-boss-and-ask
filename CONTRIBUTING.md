# Contributing / 参与贡献

Small, reproducible reports and focused pull requests are welcome. State your Windows build, PowerToys version, CPU architecture, destination app version and which independent module you use. Explain expected and actual behavior and whether the app was foreground, covered, hidden or closed.

请提供系统、PowerToys、目标应用版本、CPU 架构及具体模块，说明前台／被遮挡／隐藏／退出时哪一步失败。截图请先遮挡个人内容。

Never upload account stores, access tokens, raw chat histories, `draft-backups/` or an unreviewed diagnostic log. Questions in a reproduction should be harmless sample text, not real private material.

Build against the installed PowerToys runtime with `scripts/Build.ps1`, run `scripts/Test.ps1`, and run `tests/validate_release.py`. Document real-window checks separately from automated tests. Keep all Ask behavior draft-only: a change that introduces automatic sending is outside this project's scope.

Do not add fixed machine-specific paths, global settings-file replacements, background helper startup tasks, permanent topmost forcing, or synthetic Alt tricks. Keep the seven modules independently installable and preserve third-party license notices if new dependencies are introduced.
