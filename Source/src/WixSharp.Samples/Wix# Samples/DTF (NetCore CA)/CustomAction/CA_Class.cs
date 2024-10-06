using System.Reflection;
using System.Runtime.InteropServices;
using WixToolset.Dtf.WindowsInstaller;

// namespace CustomAction;

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

        // uncommenting next line will increate AOT compilation overhead, but it will work nevertheless
        // System.Windows.Forms.MessageBox.Show("Hello from .NET Core! (007)", "WixSharp.Core", 0);

        MessageBox(GetForegroundWindow(), "Hello from .NET Core!", "WixSharp", 0);

        // Assembly.LoadFrom();

        MessageBox(GetForegroundWindow(), typeof(Class1).Assembly.Location, "WixSharp", 0);
        // session.Log("CustomActionCore invoked");

        return (uint)ActionResult.UserExit;
    }
}