using System.Runtime.InteropServices;
using WixToolset.Dtf.WindowsInstaller;

namespace CustomAction;

public class Class1
{
    [DllImport("user32.dll")]
    static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [UnmanagedCallersOnly(EntryPoint = "CustomActionCore")]
    public static uint CustomActionCore(IntPtr handle)
    {
        using Session session = Session.FromHandle(handle, false);

        MessageBox(GetForegroundWindow(), "Hello from .NET Core!", "WixSharp", 0);
        session.Log("CustomActionCore invoked");

        return (uint)ActionResult.Success;
    }
}