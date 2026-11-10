using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wox.Plugin;

public sealed class Main : IPlugin
{
    public static string PluginID { get { return "CC37CBCF580C5E01AE100E1F896166E1"; } }
    public string Name { get { return "Codex Desktop"; } }
    public string Description { get { return "Draft a question in Codex Desktop"; } }
    public void Init(PluginInitContext context) { CommunityConfig.Initialize("AskCodex", "ChatGPT", 0); }

    public List<Result> Query(Query query)
    {
        var results = new List<Result>();
        var question = query.Search;
        if (String.IsNullOrWhiteSpace(question))
            return results;

        results.Add(new Result {
            Title = CommunityConfig.Localized("Ask Codex", "问问 Codex"),
            SubTitle = question,
            QueryTextDisplay = query.RawQuery,
            IcoPath = "icon.png",
            Action = _ => {
                bool coldStart = !IsDesktopRunning();
                Process.Start(new ProcessStartInfo(
                    "codex://new?prompt=" + Uri.EscapeDataString(question)) {
                    UseShellExecute = true
                });
                if (coldStart)
                {
                    Process.Start(new ProcessStartInfo("explorer.exe",
                        @"shell:AppsFolder\" + CommunityConfig.AppId) {
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                }
                return true;
            }
        });
        return results;
    }
    private static bool IsDesktopRunning()
    {
        var processes = Process.GetProcessesByName("ChatGPT");
        foreach (var process in processes)
            process.Dispose();
        return processes.Length != 0;
    }
}
