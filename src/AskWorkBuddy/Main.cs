using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wox.Plugin;

public sealed class Main : IPlugin
{
    public static string PluginID { get { return "31B07D1B56BC5E3B8CF299AE0BDD8AD0"; } }
    public string Name { get { return "Ask WorkBuddy"; } }
    public string Description { get { return "通过 WorkBuddy 内置接口填写问题草稿，不自动发送"; } }
    private static string root;
    public void Init(PluginInitContext context) { root=Path.GetDirectoryName(typeof(Main).Assembly.Location); CommunityConfig.Initialize("AskWorkBuddy", "WorkBuddy", 0); }
    public List<Result> Query(Query query)
    {
        var list=new List<Result>(); string question=query.Search;
        if(String.IsNullOrWhiteSpace(question))return list;
        if(question.Length>8000){list.Add(new Result{Title="问题超过 WorkBuddy 接口的 8000 字限制",SubTitle="请缩短问题后重试",QueryTextDisplay=query.RawQuery,IcoPath="icon.png"});return list;}
        list.Add(new Result{
            Title=CommunityConfig.Localized("Ask WorkBuddy", "问问 WorkBuddy"),SubTitle=question,QueryTextDisplay=query.RawQuery,IcoPath="icon.png",
            Action=_=>{
                try{
                    // The installed WorkBuddy parser's action=start publishes a task-prefill intent.
                    // Its consumer sets input blocks only; it never invokes sendPrompt.
                    var url="workbuddy://task?action=start&prompt="+Uri.EscapeDataString(question)+"&requestId="+Guid.NewGuid().ToString("N");
                    var exe=CommunityConfig.ResolveExe();
                    using(Process.Start(new ProcessStartInfo(exe,"\""+url+"\""){UseShellExecute=false,WorkingDirectory=Path.GetDirectoryName(exe)})){}
                    Log("Native draft handoff requested; not sent");return true;
                }catch(Exception e){Log("Handoff failed: "+e.GetType().Name+" "+e.Message);return false;}
            }
        });return list;
    }
    private static void Log(string text){CommunityConfig.Log(text);}
}
