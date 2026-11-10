using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Automation;

internal static class Boss
{
    private static string Marker { get { return "PowerToysCommunity.Boss." + CommunityConfig.ProcessName; } }
    private static int focusVersion;
    private static readonly object logGuard = new object();
    private delegate bool WindowVisitor(IntPtr window, IntPtr unused);

    [DllImport("user32.dll")] private static extern bool EnumWindows(WindowVisitor visitor, IntPtr unused);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window, out uint pid);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr window, StringBuilder title, int capacity);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetProp(IntPtr window, string name);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetProp(IntPtr window, string name, IntPtr value);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr RemoveProp(IntPtr window, string name);
    [DllImport("user32.dll")] private static extern bool IsWindow(IntPtr window);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr window, StringBuilder name, int size);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr window, out Rect bounds);
    private struct Rect { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")] private static extern bool IsIconic(IntPtr window);
    [DllImport("user32.dll")] private static extern bool IsWindowEnabled(IntPtr window);
    [DllImport("user32.dll")] private static extern IntPtr SendMessageTimeout(IntPtr window, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, out IntPtr result);
    [StructLayout(LayoutKind.Sequential)] private struct ScreenPoint { public int x, y; }
    [DllImport("user32.dll")] private static extern IntPtr WindowFromPoint(ScreenPoint point);
    [DllImport("user32.dll")] private static extern IntPtr GetAncestor(IntPtr window, uint flags);
    [DllImport("user32.dll")] private static extern bool ShowWindowAsync(IntPtr window, int command);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr window, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
    [DllImport("user32.dll")] private static extern bool GetLastInputInfo(ref LastInputInfo info);
    [StructLayout(LayoutKind.Sequential)] private struct LastInputInfo { public uint size, time; }


    [ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
    private class TaskbarList { }

    [ComImport, Guid("56FDF342-FD6D-11d0-958A-006097C9A090"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITaskbarList
    {
        void HrInit();
        void AddTab(IntPtr window);
        void DeleteTab(IntPtr window);
        void ActivateTab(IntPtr window);
        void SetActiveAlt(IntPtr window);
    }

    private static void SetTaskbarButton(IntPtr window, bool show)
    {
        var taskbar = (ITaskbarList)new TaskbarList();
        try
        {
            taskbar.HrInit();
            if (show)
                taskbar.AddTab(window);
            else
                taskbar.DeleteTab(window);
        }
        finally
        {
            Marshal.ReleaseComObject(taskbar);
        }
    }

    [STAThread]
    private static void Main(string[] args)
    {
        int key = 0x43;
        if (args.Length > 0 && (!int.TryParse(args[0], out key) || key < 65 || key > 90)) return;
        string processName = args.Length > 1 ? args[1] : "PhoneExperienceHost";
        using (var guard = new Mutex(false, @"Local\PowerToysCommunity.Boss." + CommunityConfig.ProcessName))
        {
            if (!guard.WaitOne(0))
                return;
            try
            {
                for (int i = 0; i < 2000 && GetAsyncKeyState(key) < 0; i++)
                    Thread.Sleep(5);
                if (GetAsyncKeyState(key) >= 0)
                {
                    Interlocked.Increment(ref focusVersion);
                    ToggleWindow(processName);
                }
            }
            finally
            {
                guard.ReleaseMutex();
            }
        }
    }

    internal static void RunRegistered(int key, string processName = "ChatGPT")
    {
        Main(new[] { key.ToString(), processName });
    }

    private static void ToggleWindow(string processName)
    {
        IntPtr target = IntPtr.Zero;
        long best = -1;
        bool isPhoneLink = processName.Equals("PhoneExperienceHost", StringComparison.OrdinalIgnoreCase);
        bool isCodex = processName.Equals("ChatGPT", StringComparison.OrdinalIgnoreCase);
        bool isTelegram = processName.Equals("Telegram", StringComparison.OrdinalIgnoreCase);
        bool isGemini = processName.Equals("Gemini", StringComparison.OrdinalIgnoreCase);
        bool isDeepSeek = processName.Equals("DeepSeek Harness", StringComparison.OrdinalIgnoreCase);
        bool smartToggle = isCodex || isTelegram || isGemini || isDeepSeek || isPhoneLink;
        var foreground = GetForegroundWindow();
        int session = Process.GetCurrentProcess().SessionId;
        foreach (var process in Process.GetProcessesByName(processName))
        {
            using (process)
            {
                if (process.SessionId != session || process.HasExited) continue;
                var id = (uint)process.Id;
                EnumWindows(delegate(IntPtr window, IntPtr unused) {
                    uint owner;
                    GetWindowThreadProcessId(window, out owner);
                    if (owner != id) return true;
                    if (smartToggle && window == foreground && IsWindowVisible(window))
                    {
                        var activeClass = new StringBuilder(256);
                        GetClassName(window, activeClass, activeClass.Capacity);
                        var activeTitle = new StringBuilder(256);
                        GetWindowText(window, activeTitle, activeTitle.Capacity);
                        bool valid = (isCodex || isGemini || isDeepSeek) ? activeClass.ToString().StartsWith("Chrome_WidgetWin_") :
                            isTelegram ? activeClass.ToString().StartsWith("Qt") : activeTitle.ToString() == "手机连接" || activeTitle.ToString() == "Phone Link";
                        if (valid) { best = long.MaxValue; target = window; return false; }
                    }
                    if (GetProp(window, Marker) != IntPtr.Zero)
                    {
                        best = smartToggle ? long.MaxValue - 1 : long.MaxValue;
                        target = window;
                        return smartToggle;
                    }
                    var title = new StringBuilder(256);
                    GetWindowText(window, title, title.Capacity);
                    if (title.Length == 0) return true;
                    if (isPhoneLink && title.ToString() != "手机连接" && title.ToString() != "Phone Link") return true;
                    if (isCodex || isTelegram || isGemini || isDeepSeek)
                    {
                        var windowClass = new StringBuilder(256);
                        GetClassName(window, windowClass, windowClass.Capacity);
                        if (!windowClass.ToString().StartsWith((isCodex || isGemini || isDeepSeek) ? "Chrome_WidgetWin_" : "Qt")) return true;
                    }
                    else if (!IsWindowVisible(window) && !IsIconic(window)) return true;

                    Rect bounds;
                    GetWindowRect(window, out bounds);
                    long score = (long)Math.Max(0, bounds.Right - bounds.Left) * Math.Max(0, bounds.Bottom - bounds.Top);
                    if (IsIconic(window)) score = 2000000;
                    if (IsWindowVisible(window) || IsIconic(window)) score += 100000000;
                    if (window == foreground) score += 200000000;
                    if (score > best)
                    {
                        best = score;
                        target = window;
                    }
                    return true;
                }, IntPtr.Zero);
                if (best == long.MaxValue) break;
            }
        }

        string appId = (isCodex || isPhoneLink) ? CommunityConfig.AppId : null;
        bool hideToTray = isCodex || isTelegram || isGemini || isDeepSeek;
        bool marked = target != IntPtr.Zero && GetProp(target, Marker) != IntPtr.Zero;
        if (target == IntPtr.Zero || (hideToTray && !marked && !IsWindowVisible(target)))
        {
            if (appId != null)
                ActivatePackagedApp(appId);
            else {
                string exe=CommunityConfig.ResolveExe();
                using(Process.Start(new ProcessStartInfo(exe){UseShellExecute=true,WorkingDirectory=Path.GetDirectoryName(exe)})){}
            }
            Log(processName + " activation requested; previous window=" + target);
            if (isCodex) FocusLaunchedCodex();
            return;
        }
        bool hide = !marked && IsWindowVisible(target) && !IsIconic(target);
        if (smartToggle) hide = hide && IsForegroundInteractive(target);
        if (hide)
        {
            if (!SetProp(target, Marker, new IntPtr(1)))
            {
                Log("Could not mark window");
                return;
            }
            bool hidden = ShowWindowAsync(target, hideToTray ? 0 : 6);
            if (hidden)
                SetTaskbarButton(target, false);
            if (!hidden)
                RemoveProp(target, Marker);
            Log("Hide=" + hidden + " window=" + target);
            return;
        }

        if (marked)
        {
            SetTaskbarButton(target, true);
        }
        bool requested = ShowWindowAsync(target, IsIconic(target) ? 9 : 5);
        bool shown = false;
        for (int attempt = 0; requested && attempt < 20 && IsWindow(target); attempt++)
        {
            shown = IsWindowVisible(target) && !IsIconic(target);
            if (shown) break;
            Thread.Sleep(25);
        }
        if (shown)
            RemoveProp(target, Marker);
        bool focused = shown && FocusRestoredWindow(target, foreground);
        if ((isPhoneLink || isCodex) && shown && !focused && IsWindow(target))
        {
            ActivatePackagedApp(appId);
            // Ask the packaged app to activate itself only after restoring the existing HWND.
            // This is a fallback; do not treat posting the request as proof of foreground success.
            for (int attempt = 0; attempt < 20 && IsWindow(target); attempt++)
            {
                if (GetForegroundWindow() == target) break;
                Thread.Sleep(50);
            }
            bool active = GetForegroundWindow() == target;
            bool composer = isCodex && active && FocusMainChat(target);
            Log(processName + " Show=True shellFallback=True foreground=" + active + " composerQueued=" + composer + " window=" + target + " actualForeground=" + GetForegroundWindow());
            return;
        }
        bool composerFocused = isCodex && focused && FocusMainChat(target);
        Log(processName + " Show=" + shown + " foreground=" + focused + " composerQueued=" + composerFocused + " window=" + target + " actualForeground=" + GetForegroundWindow());
    }

    private static bool IsForegroundInteractive(IntPtr window)
    {
        if (GetForegroundWindow() != window || !IsWindowEnabled(window)) return false;
        IntPtr result;
        if (SendMessageTimeout(window, 0, IntPtr.Zero, IntPtr.Zero, 0x22, 120, out result) == IntPtr.Zero) return false;
        Rect bounds;
        if (!GetWindowRect(window, out bounds)) return false;
        int width = bounds.Right - bounds.Left, height = bounds.Bottom - bounds.Top;
        if (width < 1 || height < 1) return false;
        // Sample interior points so a non-activating overlay also prevents hiding.
        int[] xs = { 50, 20, 80, 20, 80 }, ys = { 50, 20, 20, 80, 80 };
        for (int i = 0; i < xs.Length; i++) {
            var hit = WindowFromPoint(new ScreenPoint { x = bounds.Left + width * xs[i] / 100, y = bounds.Top + height * ys[i] / 100 });
            if (hit == IntPtr.Zero || GetAncestor(hit, 2) != window) return false;
        }
        return GetForegroundWindow() == window;
    }

    private static bool FocusMainChat(IntPtr window)
    {
        int version = Interlocked.CompareExchange(ref focusVersion, 0, 0);
        // Keep the real hotkey message loop responsive while Alt is held.
        return ThreadPool.QueueUserWorkItem(delegate {
            for (int attempt = 0; attempt < 2000; attempt++)
            {
                if (version != Interlocked.CompareExchange(ref focusVersion, 0, 0) || !IsWindow(window) || !IsWindowVisible(window) || GetForegroundWindow() != window) return;
                if (GetAsyncKeyState(0x12) >= 0 && GetAsyncKeyState(0x45) >= 0)
                {
                    // Async key state may change before the restored renderer has processed
                    // the real Alt-up/focus messages. Allow a quiet settling interval.
                    var input = new LastInputInfo { size = (uint)Marshal.SizeOf(typeof(LastInputInfo)) };
                    bool tracked = GetLastInputInfo(ref input);
                    uint releasedAt = input.time;
                    Thread.Sleep(100);
                    if (version != Interlocked.CompareExchange(ref focusVersion, 0, 0) || GetForegroundWindow() != window || !IsWindowVisible(window) || GetAsyncKeyState(0x12) < 0) return;
                    if (tracked && GetLastInputInfo(ref input) && input.time != releasedAt)
                    {
                        Log("ChatGPT composer focus cancelled: new user input during settling");
                        return;
                    }
                    bool sent = SendFocusMainChat(window);
                    Log("ChatGPT AltReleased=True settleMs=100 composerFocus=" + sent + " window=" + window);
                    return;
                }
                Thread.Sleep(5);
            }
            Log("ChatGPT composer focus cancelled: Alt still held at timeout");
        });
    }

    private static bool SendFocusMainChat(IntPtr window)
    {
        int version = Interlocked.CompareExchange(ref focusVersion, 0, 0);
        try {
            for (int attempt = 0; attempt < 5; attempt++) {
                if (GetForegroundWindow() != window || GetAsyncKeyState(0x12) < 0 || version != Interlocked.CompareExchange(ref focusVersion, 0, 0)) return false;
                var root = AutomationElement.FromHandle(window);
                var condition = new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                    new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                    new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                    new PropertyCondition(AutomationElement.IsKeyboardFocusableProperty, true));
                var candidates = root.FindAll(TreeScope.Descendants, condition);
                var matching = new System.Collections.Generic.List<AutomationElement>();
                foreach (AutomationElement candidate in candidates)
                    if (Array.IndexOf(candidate.Current.ClassName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), "ProseMirror") >= 0) matching.Add(candidate);
                var editors = matching;
                if (editors.Count == 1) {
                    if (GetForegroundWindow() != window || version != Interlocked.CompareExchange(ref focusVersion, 0, 0)) return false;
                    // A mode switch can leave DOM focus on the editor while its WebContents
                    // is not the active keyboard surface. Activate that document first.
                    var ancestor = TreeWalker.ControlViewWalker.GetParent(editors[0]);
                    for (int depth = 0; ancestor != null && depth < 15; depth++) {
                        if (ancestor.Current.ControlType == ControlType.Document) { if (ancestor.Current.IsKeyboardFocusable) ancestor.SetFocus(); break; }
                        ancestor = TreeWalker.ControlViewWalker.GetParent(ancestor);
                    }
                    if (GetForegroundWindow() != window || version != Interlocked.CompareExchange(ref focusVersion, 0, 0)) return false;
                    editors[0].SetFocus();
                    for (int verify = 0; verify < 5 && !editors[0].Current.HasKeyboardFocus; verify++) Thread.Sleep(20);
                    bool focused = editors[0].Current.HasKeyboardFocus;
                    Log("ChatGPT directComposerFocus=" + focused + " window=" + window);
                    return focused;
                }
                if (editors.Count > 1) { Log("ChatGPT directComposerFocus skipped: multiple editors"); return false; }
                Thread.Sleep(60);
            }
            Log("ChatGPT directComposerFocus skipped: composer not exposed");
        } catch (Exception error) { Log("ChatGPT directComposerFocus error=" + error.GetType().Name); }
        return false;
    }

    private static void FocusLaunchedCodex()
    {
        for (int attempt = 0; attempt < 60; attempt++)
        {
            IntPtr window = GetForegroundWindow(); uint pid;
            GetWindowThreadProcessId(window, out pid);
            try {
                using (var process = Process.GetProcessById((int)pid)) {
                    if (process.ProcessName.Equals("ChatGPT", StringComparison.OrdinalIgnoreCase) && process.SessionId == Process.GetCurrentProcess().SessionId) {
                        Thread.Sleep(350);
                        bool sent = FocusMainChat(window);
                        Log("ChatGPT launch composerFocus=" + sent + " window=" + window);
                        return;
                    }
                }
            } catch (ArgumentException) { }
            Thread.Sleep(100);
        }
        Log("ChatGPT launch composer focus pending; app did not become foreground in observation window");
    }

    private static bool FocusRestoredWindow(IntPtr target, IntPtr previousForeground)
    {
        // Both show and foreground changes can be queued to the application's UI thread.
        // Never join input queues, synthesize Alt, or leave the window permanently topmost.
        for (int attempt = 0; attempt < 12 && IsWindow(target); attempt++)
        {
            var current = GetForegroundWindow();
            if (current == target) return true;
            // If the user deliberately moved to another window during restoration, stop.
            if (current != IntPtr.Zero && current != previousForeground) return false;
            if (attempt == 2)
                SetWindowPos(target, IntPtr.Zero, 0, 0, 0, 0, 0x4003); // ASYNCWINDOWPOS | NOMOVE | NOSIZE, ordinary TOP
            SetForegroundWindow(target);
            Thread.Sleep(40);
        }
        return GetForegroundWindow() == target;
    }

    private static void ActivatePackagedApp(string appId)
    {
        using (Process.Start(new ProcessStartInfo("explorer.exe",
            @"shell:AppsFolder\" + appId) {
            UseShellExecute = true
        })) { }
    }

    internal static void RestoreOwnedWindows() {
        Interlocked.Increment(ref focusVersion);
        EnumWindows((window,unused)=>{
            if(GetProp(window,Marker)!=IntPtr.Zero){
                ShowWindowAsync(window,IsIconic(window)?9:5);
                try{SetTaskbarButton(window,true);}catch{}
                RemoveProp(window,Marker);
            }return true;
        },IntPtr.Zero);
    }
    private static void Log(string message){CommunityConfig.Log(message);}
}
