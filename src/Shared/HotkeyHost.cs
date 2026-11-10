using System;
using System.Runtime.InteropServices;
using System.Threading;

internal sealed class HotkeyHost : IDisposable
{
    [StructLayout(LayoutKind.Sequential)] private struct Message {public IntPtr window;public uint message;public UIntPtr wParam;public IntPtr lParam;public uint time;public int x,y;public uint reserved;}
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll",SetLastError=true)] private static extern bool RegisterHotKey(IntPtr window,int id,uint modifiers,uint key);
    [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr window,int id);
    [DllImport("user32.dll")] private static extern int GetMessage(out Message message,IntPtr window,uint min,uint max);
    [DllImport("user32.dll")] private static extern bool PeekMessage(out Message message,IntPtr window,uint min,uint max,uint flags);
    [DllImport("user32.dll")] private static extern bool PostThreadMessage(uint thread,uint message,UIntPtr wParam,IntPtr lParam);
    private uint threadId;
    private volatile bool disposed;
    private Thread worker;
    internal void Start(){worker=new Thread(Run){IsBackground=true,Name=CommunityConfig.Slug+" hotkey"};worker.SetApartmentState(ApartmentState.STA);worker.Start();}
    private void Run(){
        int id=CommunityConfig.DefaultKey+0x7100;bool registered=false;
        try{
            threadId=GetCurrentThreadId();Message message;PeekMessage(out message,IntPtr.Zero,0,0,0);
            int key=CommunityConfig.Key;
            if(disposed||key==0){CommunityConfig.Log("Disabled: Hotkey must be one letter A-Z.");return;}
            registered=RegisterHotKey(IntPtr.Zero,id,0x4001,(uint)key);
            CommunityConfig.Log(registered?"Registered Alt+"+(char)key:"Registration failed; Windows error="+Marshal.GetLastWin32Error()+". Check hotkey conflicts.");
            if(!registered||disposed)return;
            while(GetMessage(out message,IntPtr.Zero,0,0)>0){
                if(message.message==0x312){try{Boss.RunRegistered(key,CommunityConfig.ProcessName);}catch(Exception e){CommunityConfig.Log("Hotkey failed: "+e.GetType().Name);}}
            }
        }finally{if(registered)UnregisterHotKey(IntPtr.Zero,id);Boss.RestoreOwnedWindows();threadId=0;}
    }
    public void Dispose(){disposed=true;if(threadId!=0)PostThreadMessage(threadId,0x12,UIntPtr.Zero,IntPtr.Zero);if(worker!=null&&worker!=Thread.CurrentThread)worker.Join(1000);}
}
