using System;
using System.IO;
using Wox.Plugin;
internal static class ContractTests {
    private static void Assert(bool value,string label){if(!value)throw new Exception(label);}
    [STAThread] private static int Main(string[] args){
        try{
            string dir=Path.GetDirectoryName(typeof(ContractTests).Assembly.Location);
            string config=Path.Combine(dir,"settings.ini");
            File.WriteAllText(config,"# test\nHotkey=Z\nBackupDrafts=false\nAppId=test.app!App\nAppPath=\nDiagnostics=false\n");
            CommunityConfig.Initialize(args[0],args[1],69);
            Assert(CommunityConfig.Key==90,"Configurable hotkey");
            Assert(!CommunityConfig.BackupDrafts,"Opt-out draft backup");
            Assert(CommunityConfig.AppId=="test.app!App","Configured package identity");
            File.WriteAllText(config,"Hotkey=NotAKey\n");CommunityConfig.Initialize(args[0],args[1],69);
            Assert(CommunityConfig.Key==0,"Invalid hotkey disables registration");
            Assert(CommunityConfig.BackupDrafts,"Missing setting uses fresh defaults");
            var main=new Main();
            if(args[0].StartsWith("Ask")){
                Assert(main.Query(new Query{Search="",RawQuery="",ActionKeyword=""}).Count==0,"Empty query has no result");
                string text="北京大学面积 \"a&b\" + 日本語 😀";
                foreach(string keyword in new[]{"",args[2]}){
                    string raw=keyword.Length==0?text:keyword+" "+text;
                    var result=main.Query(new Query{Search=text,RawQuery=raw,ActionKeyword=keyword});
                    Assert(result.Count==1,"One Ask result");
                    Assert(result[0].QueryTextDisplay==raw,"Tab and Shift+Tab preserve raw query");
                    Assert(result[0].SubTitle==text,"Question body preserved");
                    Assert(result[0].Action!=null,"Draft action available");
                }
                if(args[0]=="AskWorkBuddy"){
                    var longResult=main.Query(new Query{Search=new string('a',8001),RawQuery="long",ActionKeyword="wb"});
                    Assert(longResult.Count==1&&longResult[0].Action==null,"Reject oversized URI draft rather than truncating");
                }
            }
            Assert(global::Main.PluginID.Length==32,"Plugin ID format");
            Console.WriteLine("PASS "+args[0]+": config and query contracts; no apps launched or messages sent.");return 0;
        }catch(Exception e){Console.Error.WriteLine(e.Message);return 1;}
    }
}
