using System;
using System.Collections.Generic;
using Wox.Plugin;
public sealed class Main : IPlugin, IDisposable {
    public static string PluginID { get { return "6DAF71D9147B5FEB984046EA5DC90FA7"; } }
    public string Name { get { return "Codex boss key"; } }
    public string Description { get { return "Foreground: hide. Background: restore. Closed: launch."; } }
    private HotkeyHost host;
    public void Init(PluginInitContext context) { CommunityConfig.Initialize("BossCodex","ChatGPT",69);host=new HotkeyHost();host.Start(); }
    public List<Result> Query(Query query) { return new List<Result>{new Result{Title=CommunityConfig.Localized("Codex boss key settings","Codex 老板键设置"),SubTitle=CommunityConfig.Status,QueryTextDisplay=query.RawQuery,IcoPath="icon.png",Action=_=>{CommunityConfig.OpenSettings();return true;}}}; }
    public void Dispose(){if(host!=null)host.Dispose();}
}
