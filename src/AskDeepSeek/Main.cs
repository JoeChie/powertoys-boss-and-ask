using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using Wox.Plugin;

public sealed class Main : IPlugin
{
    public static string PluginID { get { return "A33F7CB55DFE5776829004F953003F4A"; } }
    public string Name { get { return "Ask DeepSeek Harness"; } }
    public string Description { get { return "在 DeepSeek Harness 桌面端的新对话中填写问题草稿，不自动发送"; } }
    private static int busy;
    private static string root;
    public void Init(PluginInitContext context) { root=Path.GetDirectoryName(typeof(Main).Assembly.Location); CommunityConfig.Initialize("AskDeepSeek", "DeepSeek Harness", 0); }
    public List<Result> Query(Query query)
    {
        var list=new List<Result>(); string question=query.Search;
        if(String.IsNullOrWhiteSpace(question))return list;
        list.Add(new Result { Title=CommunityConfig.Localized("Ask DeepSeek Harness", "问问 DeepSeek Harness"), SubTitle=question, QueryTextDisplay=query.RawQuery, IcoPath="icon.png", Action=_=>{
            if(Interlocked.CompareExchange(ref busy,1,0)!=0)return false;
            try {
                var exe=CommunityConfig.ResolveExe();
                using(Process.Start(new ProcessStartInfo(exe){UseShellExecute=true, WorkingDirectory=Path.GetDirectoryName(exe)})){}
                var worker=new Thread(()=>Draft(question));worker.IsBackground=true;worker.SetApartmentState(ApartmentState.STA);worker.Start();
                return true;
            } catch(Exception e){Log("Launch failed: "+e.GetType().Name);Interlocked.Exchange(ref busy,0);return false;}
        }});return list;
    }
    private delegate bool Visitor(IntPtr hwnd,IntPtr ignored);
    [DllImport("user32.dll")] private static extern bool EnumWindows(Visitor visitor,IntPtr ignored);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd,out uint pid);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] private static extern int GetClassName(IntPtr hwnd,StringBuilder name,int size);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] private static extern int GetWindowText(IntPtr hwnd,StringBuilder name,int size);
    [DllImport("user32.dll")] private static extern bool ShowWindowAsync(IntPtr hwnd,int command);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool IsIconic(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
    [StructLayout(LayoutKind.Sequential)] private struct Keyboard { public ushort key, scan; public uint flags, time; public UIntPtr extra; }
    [StructLayout(LayoutKind.Explicit)] private struct InputData { [FieldOffset(0)] public Keyboard keyboard; [FieldOffset(0)] public Mouse mouse; }
    [StructLayout(LayoutKind.Sequential)] private struct Mouse { public int x,y; public uint data,flags,time; public UIntPtr extra; }
    [StructLayout(LayoutKind.Sequential)] private struct Input { public uint type; public InputData data; }
    [DllImport("user32.dll",SetLastError=true)] private static extern uint SendInput(uint count,Input[] inputs,int size);
    private static void TypeDraft(IntPtr hwnd,AutomationElement app,AutomationElement edit,string question)
    {
        // Wait for Run's Enter/Alt/Ctrl release; never inject a submit key.
        for(int i=0;i<100;i++){
            if((GetAsyncKeyState(13)&0x8000)==0&&(GetAsyncKeyState(18)&0x8000)==0&&(GetAsyncKeyState(17)&0x8000)==0&&(GetAsyncKeyState(16)&0x8000)==0)break;
            if(i==99)throw new InvalidOperationException("Modifiers still held");
            Thread.Sleep(20);
        }
        SetForegroundWindow(hwnd);
        var document=app.FindFirst(TreeScope.Descendants,new PropertyCondition(AutomationElement.ControlTypeProperty,ControlType.Document));
        if(document!=null)try{document.SetFocus();}catch{}
        edit.SetFocus();Thread.Sleep(100);
        if(GetForegroundWindow()!=hwnd||!Automation.Compare(AutomationElement.FocusedElement,edit))throw new InvalidOperationException("Composer focus unavailable");
        var value=(ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
        string previous=value.Current.Value;
        if(!String.IsNullOrEmpty(previous)){
            // DeepSeek reuses its unsent new-session page. Keep the draft before replacing it.
            if(!(edit.Current.Name.StartsWith("描述你想要构建的内容")||edit.Current.Name.StartsWith("Describe what you want to build",StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("New conversation not confirmed; existing draft preserved");
            if(CommunityConfig.BackupDrafts){string backups=Path.Combine(root,"draft-backups");Directory.CreateDirectory(backups);
            File.WriteAllText(Path.Combine(backups,DateTime.Now.ToString("yyyyMMdd-HHmmss-fff")+".txt"),previous);}
        }
        var inputs=new List<Input>();
        if(!String.IsNullOrEmpty(previous)){
            inputs.Add(new Input{type=1,data=new InputData{keyboard=new Keyboard{key=17}}});
            inputs.Add(new Input{type=1,data=new InputData{keyboard=new Keyboard{key=65}}});
            inputs.Add(new Input{type=1,data=new InputData{keyboard=new Keyboard{key=65,flags=2}}});
            inputs.Add(new Input{type=1,data=new InputData{keyboard=new Keyboard{key=17,flags=2}}});
        }
        foreach(char ch in question){
            if(ch=='\r'||ch=='\n')throw new InvalidOperationException("Multiline query not supported");
            inputs.Add(new Input{type=1,data=new InputData{keyboard=new Keyboard{scan=ch,flags=4}}});
            inputs.Add(new Input{type=1,data=new InputData{keyboard=new Keyboard{scan=ch,flags=6}}});
        }
        if(GetForegroundWindow()!=hwnd||!Automation.Compare(AutomationElement.FocusedElement,edit)||value.Current.Value!=previous)throw new InvalidOperationException("Focus or draft changed");
        if(SendInput((uint)inputs.Count,inputs.ToArray(),Marshal.SizeOf(typeof(Input)))!=inputs.Count)throw new InvalidOperationException("Text input interrupted");
        for(int i=0;i<20;i++){if(value.Current.Value==question)return;Thread.Sleep(100);}
        throw new InvalidOperationException("Draft verification failed");
    }
    private static IntPtr FindWindow()
    {
        IntPtr found=IntPtr.Zero;
        EnumWindows((hwnd,unused)=>{uint pid;GetWindowThreadProcessId(hwnd,out pid);
            try {using(var p=Process.GetProcessById((int)pid)){
                if(!p.ProcessName.Equals("DeepSeek Harness",StringComparison.OrdinalIgnoreCase))return true;
                var cls=new StringBuilder(256);GetClassName(hwnd,cls,cls.Capacity);
                var title=new StringBuilder(256);GetWindowText(hwnd,title,title.Capacity);
                if(cls.ToString().StartsWith("Chrome_WidgetWin_")&&title.ToString().EndsWith("DeepSeek Harness",StringComparison.OrdinalIgnoreCase)){found=hwnd;return false;}
            }}catch{}return true;},IntPtr.Zero);return found;
    }
    private static bool Named(string name,string[] names){foreach(string s in names)if(name.Equals(s,StringComparison.OrdinalIgnoreCase))return true;return false;}
    private static AutomationElement NewChat(AutomationElement app)
    {
        var elements=app.FindAll(TreeScope.Descendants,new PropertyCondition(AutomationElement.ControlTypeProperty,ControlType.Button));
        foreach(AutomationElement e in elements){if(!e.Current.IsOffscreen&&e.Current.IsEnabled&&Named(e.Current.Name,new[]{"新建会话","New conversation","New chat"}))return e;}
        return null;
    }
    private static void Invoke(AutomationElement e){object p;if(!e.TryGetCurrentPattern(InvokePattern.Pattern,out p))throw new InvalidOperationException("Action unavailable");((InvokePattern)p).Invoke();}
    private static AutomationElement Composer(AutomationElement app)
    {
        var edits=app.FindAll(TreeScope.Descendants,new PropertyCondition(AutomationElement.ControlTypeProperty,ControlType.Edit));
        AutomationElement match=null;
        foreach(AutomationElement e in edits){if(e.Current.IsOffscreen||!e.Current.IsEnabled||!(e.Current.Name.StartsWith("描述你想要构建的内容")||e.Current.Name.StartsWith("发消息或创建任务")||e.Current.Name.StartsWith("Describe what you want to build",StringComparison.OrdinalIgnoreCase)||e.Current.Name.StartsWith("Send a message")||e.Current.ClassName.IndexOf("ProseMirror",StringComparison.OrdinalIgnoreCase)>=0))continue;
            object p;if(!e.TryGetCurrentPattern(ValuePattern.Pattern,out p)||((ValuePattern)p).Current.IsReadOnly)continue;
            if(match!=null)throw new InvalidOperationException("Ambiguous composer");match=e;
        }return match;
    }
    private static void Draft(string question)
    {
        try {
            IntPtr hwnd=IntPtr.Zero;for(int i=0;i<140;i++){hwnd=FindWindow();if(hwnd!=IntPtr.Zero)break;Thread.Sleep(250);}
            if(hwnd==IntPtr.Zero)throw new TimeoutException("Window unavailable");
            ShowWindowAsync(hwnd,IsIconic(hwnd)?9:5);SetForegroundWindow(hwnd);
            AutomationElement app=AutomationElement.FromHandle(hwnd),link=null;
            for(int i=0;i<140;i++){
                link=NewChat(app);if(link!=null)break;
                Thread.Sleep(250);
            }
            if(link==null)throw new TimeoutException("New chat unavailable");
            Invoke(link);Thread.Sleep(1000);
            AutomationElement edit=null;for(int i=0;i<100;i++){edit=Composer(app);if(edit!=null)break;Thread.Sleep(250);}
            if(edit==null)throw new TimeoutException("Composer unavailable");
            TypeDraft(hwnd,app,edit,question);
            Log("Draft ready; not sent");
        }catch(Exception e){Log("Draft failed: "+e.GetType().Name+" "+e.Message);}
        finally{Interlocked.Exchange(ref busy,0);}
    }
    private static void Log(string text){CommunityConfig.Log(text);}
}
