using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

internal static class CommunityConfig
{
    private static readonly Dictionary<string,string> values=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
    internal static string Root, ProcessName, Slug;
    internal static int DefaultKey;
    internal static string Status="Starting";
    internal static void Initialize(string slug,string process,int key){
        values.Clear();
        Root=Path.GetDirectoryName(typeof(CommunityConfig).Assembly.Location);Slug=slug;ProcessName=process;DefaultKey=key;
        string file=Path.Combine(Root,"settings.ini");
        if(File.Exists(file))foreach(string raw in File.ReadAllLines(file)){
            string line=raw.Trim();if(line.Length==0||line.StartsWith("#")||line.StartsWith(";"))continue;
            int index=line.IndexOf('=');if(index>0)values[line.Substring(0,index).Trim()]=line.Substring(index+1).Trim();
        }
    }
    internal static string Get(string key,string fallback){string v;return values.TryGetValue(key,out v)?v:fallback;}
    internal static int Key {get{string s=Get("Hotkey",DefaultKey==0?"":((char)DefaultKey).ToString()).ToUpperInvariant();return s.Length==1&&s[0]>='A'&&s[0]<='Z'?s[0]:0;}}
    internal static bool BackupDrafts {get{return !Get("BackupDrafts","true").Equals("false",StringComparison.OrdinalIgnoreCase);}}
    internal static string AppId {get{return Get("AppId",ProcessName=="ChatGPT"?"OpenAI.Codex_2p2nqsd0c76g0!App":ProcessName=="PhoneExperienceHost"?"Microsoft.YourPhone_8wekyb3d8bbwe!App":"");}}
    internal static string Localized(string english,string chinese){return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName=="zh"?chinese:english;}
    internal static string ResolveExe(){
        string configured=Environment.ExpandEnvironmentVariables(Get("AppPath",""));
        if(configured.Length>0){if(File.Exists(configured)&&Path.IsPathRooted(configured))return configured;throw new FileNotFoundException("Set an existing absolute AppPath in settings.ini.");}
        foreach(var process in Process.GetProcessesByName(ProcessName))using(process){try{var path=process.MainModule.FileName;if(File.Exists(path))return path;}catch{}}
        string[] bases={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)};
        string[] relative=ProcessName=="Telegram"?new[]{@"Telegram Desktop\Telegram.exe"}:ProcessName=="WorkBuddy"?new[]{@"Programs\WorkBuddy\WorkBuddy.exe",@"WorkBuddy\WorkBuddy.exe"}:new[]{@"DeepSeek Harness\DeepSeek Harness.exe",@"Programs\DeepSeek Harness\DeepSeek Harness.exe"};
        foreach(string folder in bases)foreach(string part in relative){string path=Path.Combine(folder,part);if(File.Exists(path))return path;}
        throw new FileNotFoundException("Application not found. Set AppPath in this plugin's settings.ini.");
    }
    internal static void OpenSettings(){using(Process.Start(new ProcessStartInfo("notepad.exe","\""+Path.Combine(Root,"settings.ini")+"\""){UseShellExecute=true})){} }
    internal static void Log(string message){
        Status=message;
        try{
            // No question text, account data or clipboard contents are included.
            string line=DateTime.Now.ToString("s")+" "+message;
            File.WriteAllText(Path.Combine(Root,"status.txt"),line);
            if(Get("Diagnostics","false").Equals("true",StringComparison.OrdinalIgnoreCase)){
                string file=Path.Combine(Root,"diagnostics.log");
                if(File.Exists(file)&&new FileInfo(file).Length>131072)File.WriteAllText(file,"");
                File.AppendAllText(file,line+Environment.NewLine);
            }
        }catch{}
    }
}
