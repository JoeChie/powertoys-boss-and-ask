using System;
using System.Collections.Generic;
using Wox.Plugin;
public sealed class Main : IPlugin, IDisposable {
    public static string PluginID { get { return "3520A8176A365687B8E172FF479C3A08"; } }
    public string Name { get { return "Telegram boss key"; } }
    public string Description { get { return "Foreground: hide. Background: restore. Closed: launch."; } }
    private HotkeyHost host;
    public void Init(PluginInitContext context) { CommunityConfig.Initialize("BossTelegram","Telegram",88);host=new HotkeyHost();host.Start(); }
    public List<Result> Query(Query query) { return new List<Result>{new Result{Title=CommunityConfig.Localized("Telegram boss key settings","Telegram 老板键设置"),SubTitle=CommunityConfig.Status,QueryTextDisplay=query.RawQuery,IcoPath="icon.png",Action=_=>{CommunityConfig.OpenSettings();return true;}}}; }
    public void Dispose(){if(host!=null)host.Dispose();}
}
