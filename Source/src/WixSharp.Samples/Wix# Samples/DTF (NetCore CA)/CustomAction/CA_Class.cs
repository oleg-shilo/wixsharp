using System.Runtime.InteropServices;
using WixToolset.Dtf.WindowsInstaller;

namespace CustomAction;

public class Class1
{
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

    [UnmanagedCallersOnly(EntryPoint = "CustomActionCore")]
    public static uint CustomActionCore(IntPtr handle)
    {
        using Session session = Session.FromHandle(handle, false);

        MessageBox(IntPtr.Zero, "Hello from .NET Core!", "WixSharp", 0);
        session.Log("CustomActionCore invoked");

        return 0;
    }
}