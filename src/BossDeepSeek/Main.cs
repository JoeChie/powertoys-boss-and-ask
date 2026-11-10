using System;
using System.Collections.Generic;
using Wox.Plugin;
public sealed class Main : IPlugin, IDisposable {
    public static string PluginID { get { return "0DF931DD43F65D8588A285CA6E3B5195"; } }
    public string Name { get { return "DeepSeek Harness boss key"; } }
    public string Description { get { return "Foreground: hide. Background: restore. Closed: launch."; } }
    private HotkeyHost host;
    public void Init(PluginInitContext context) { CommunityConfig.Initialize("BossDeepSeek","DeepSeek Harness",70);host=new HotkeyHost();host.Start(); }
    public List<Result> Query(Query query) { return new List<Result>{new Result{Title=CommunityConfig.Localized("DeepSeek Harness boss key settings","DeepSeek Harness 老板键设置"),SubTitle=CommunityConfig.Status,QueryTextDisplay=query.RawQuery,IcoPath="icon.png",Action=_=>{CommunityConfig.OpenSettings();return true;}}}; }
    public void Dispose(){if(host!=null)host.Dispose();}
}
