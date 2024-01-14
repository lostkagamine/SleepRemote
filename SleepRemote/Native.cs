using System.Runtime.InteropServices;

namespace SleepRemote;

public static class Native
{
    public enum SuspendMode
    {
        Sleep,
        Hibernate
    }

    public enum ShutdownMode
    {
        Shutdown,
        Reboot
    }
    
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    private struct TokPriv1Luid
    {
        public int Count;
        public long Luid;
        public int Attr;
    }
    
    [DllImport("powrprof.dll")]
    private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

    [DllImport("Advapi32.dll")]
    private static extern bool InitiateSystemShutdownA(
        string machineName,
        string message,
        uint timeout,
        bool forceAppsClosed,
        bool rebootAfterShutdown);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();
    
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(
        IntPtr h,
        int acc,
        ref IntPtr phtok);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool LookupPrivilegeValue(
        string? host,
        string name,
        ref long pluid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool AdjustTokenPrivileges(
        IntPtr htok,
        bool disall,
        ref TokPriv1Luid newst,
        int len,
        IntPtr prev,
        IntPtr relen);

    private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
    private const int TOKEN_QUERY = 0x08;
    private const int SE_PRIVILEGE_ENABLED = 0x02;
    private const string SE_SHUTDOWN_PRIVILEGE = "SeShutdownPrivilege";

    private static void GainShutdownPrivilege()
    {
        TokPriv1Luid tp;
        var process = GetCurrentProcess();
        var hToken = IntPtr.Zero;
        OpenProcessToken(process, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken);
        tp.Count = 1;
        tp.Luid = 0;
        tp.Attr = SE_PRIVILEGE_ENABLED;
        LookupPrivilegeValue(null, SE_SHUTDOWN_PRIVILEGE, ref tp.Luid);
        AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
    }
    
    public static void ShutdownSystem(ShutdownMode mode)
    {
        GainShutdownPrivilege();
        
        // It looks cleaner, shut up ReSharper. ~Homura
        // ReSharper disable once UseStringInterpolation
        var message = string.Format(
            "This computer has been requested to {0} by a trusted remote device.",
            mode == ShutdownMode.Reboot ? "reboot" : "shutdown");
        
        InitiateSystemShutdownA(
            "",
            message,
            20,
            true,
            mode == ShutdownMode.Reboot);
    }

    public static void SuspendSystem(SuspendMode mode)
    {
        SetSuspendState(mode == SuspendMode.Hibernate, false, false);
    }
}