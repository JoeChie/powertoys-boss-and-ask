# Publishing / 发布流程

Repository: https://github.com/JoeChie/powertoys-boss-and-ask

1. Build against the intended PowerToys runtime, run contract and isolated installer tests, and check portable configuration.
2. Update bilingual documentation and versioned release URLs. Package seven independent ZIPs using fresh staging/output directories.
3. Run `tests/validate_release.py` against that output. Review Git's staged file list. Never include private drafts, accounts, logs, third-party app binaries or PowerToys runtime DLLs.
4. Commit with a public GitHub noreply address. Publish source and a preview release containing seven ZIPs plus SHA256SUMS.txt.
5. Verify repository visibility, each attached asset and download link. Record automated checks separately from target-machine manual tests.

发布前重新编译、测试、检查中英文链接与包内容；用新的打包目录生成七个独立 ZIP 和校验文件。不得上传账号、草稿、日志或第三方应用本体。发布后验证仓库可见性和每个下载链接；预览版不把自动测试说成目标机器实测。
