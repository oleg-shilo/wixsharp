using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using WixSharp;
using WixSharp.Controls;

namespace WixSharp.CommonTasks
{
    /// <summary>
    /// UAC prompt revealer. This is a work around for the MSI limitation/problem with EmbeddedUI UAC prompt.
    /// <para> The symptom of the problem is the UAC prompt not being displayed during the elevation but rather
    /// minimized on the taskbar. It's caused by the fact the all background applications (including MSI runtime)
    /// supposed to register the main window for UAC prompt. And, MSI does not doe the registration for EmbeddedUI.
    /// </para>
    /// <para>Call <c>UACRevealer.Enter</c> just before triggering UAC (staring the actual install). This will
    /// "steal" the focus from the MSI EmbeddedUI window. This in turn will bring UAC prompt to foreground.</para>
    /// <para>Call <c>UACRevealer.Exit</c> just after UAC prompt has been closed to dispose UACRevealer.
    /// </para>
    /// <para> See "Use the HWND Property to Be Acknowledged as a Foreground Application" section at
    /// https://msdn.microsoft.com/en-us/library/bb756922.aspx
    /// </para>
    /// </summary>
    public static class UACRevealer
    {
        /// <summary>
        /// Activates UACRevealer
        /// </summary>
        public static void Enter()
        {
            // https://superuser.com/questions/89008/vista-admin-user-dialog-hidden
            // https://msdn.microsoft.com/en-us/library/bb756922.aspx

            // Issue #151: UAC prompt appears minimized on TaskBar.
            // MSI is a background process (service) thus it is responsible for passing its
            // main window handle to the UAC. It does it correctly for its own UI but not for
            // the embedded one.
            // A simple work around allows to fix this behaver without begging MS for fixing MSI.
            // It seems that just having a foreground process main window (e.g. notepad.exe)
            // with the focus is enough to trick the UAC into believing that UAC prompt needs to
            // be displayed. Tested on Win10.
            //
            // Though the trick is not 100% reliable and may not work on all OS versions.

            // Issue #301: Managed UI: UAC prompt is always in background
            // Feb 2018 the trick stopped working on Win10 (presumably after win update)
            // Change of tactics. now instead of notepad instance let's set focus to task bar.
            // It works perfect with SetForegroundWindow(taskbar) but it also triggers taksbar item
            // preview. Thus the approach is to simulate mouse click at the end of the taskbar rect
            //
            // Win32.StartMonitoringTaskbarForUAC(); works quite OK too but it's too hacky

            if (Enabled)
                try
                {
                    approach_3();
                }
                catch { }
        }

        static void approach_1()   // doesn't longer work
        {
            var exe = "notepad.exe";
            UAC_revealer = System.Diagnostics.Process.Start(exe);
            UAC_revealer.WaitForInputIdle(2000);
            if (UAC_revealer.MainWindowHandle != IntPtr.Zero)
            {
                Win32.MoveWindow(UAC_revealer.MainWindowHandle, 0, 0, 30, 30, true);
            }
        }

        static void approach_2() // works great but only with taskbar that has an empty spot at the end
        {
            Win32.SetFocusOnTaskbar();

            ThreadPool.QueueUserWorkItem(x =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    Win32.SetFocusOnTaskbar();
                }
            });
        }

        static void approach_3() // works great but pops up the taskbar item preview. Nevertheless it is the safest
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                Thread.Sleep(3000);
                Win32.SetForegroundWindow(Win32.GetTaskbarWindow());
            });
        }

        /// <summary>
        /// Deactivates UACRevealer
        /// </summary>
        public static void Exit()
        {
            if (UAC_revealer != null)
            {
                try { UAC_revealer.Kill(); }
                catch { }
                UAC_revealer = null;
            }
        }

        /// <summary>
        /// Enables UACRevealer support. This flag is required for enabling UAC prompt work around for
        /// the WixSharp built-in EmbeddedUI (ManagedUI).
        /// </summary>
        public static bool Enabled = false;

        /// <summary>
        /// The warning text to be displayed in the ProgressDialog. By default it prompts "Please wait for UAC prompt to appear.
        /// If it appears minimized then activate it from the taskbar.".
        /// </summary>
        public static string WarningText = "";

        static System.Diagnostics.Process UAC_revealer;
    }

    /// <summary>
    ///
    /// </summary>
    public static class Uac
    {
        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnabled()
        {
            return Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                "EnableInstallerDetection",
                -1).Equals(1);
        }
    }

    static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("User32.dll")]
        internal static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        static string GetWindowClass(IntPtr hWnd)
        {
            var class_name = new StringBuilder(1024);
            var t = GetClassName(hWnd, class_name, class_name.Capacity);

            return class_name.ToString();
        }

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetActiveWindow(IntPtr hWnd);

        delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        internal static IntPtr GetTaskbarWindow()
        {
            var hwndTrayWnd = FindWindow("Shell_TrayWnd", null);
            IntPtr taskbar = IntPtr.Zero;

            EnumChildWindows(hwndTrayWnd, delegate (IntPtr hWnd, IntPtr param)
            {
                var name = GetWindowClass(hWnd);
                // Debug.WriteLine(name);
                if (name == "MSTaskListWClass")
                {
                    taskbar = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return taskbar;
        }

        internal static void SetFocusOnTaskbar()
        {
            var taskbar = GetTaskbarWindow();

            RECT rect;
            GetWindowRect(taskbar, out rect);

            POINT pos = GetCursorPos();

            if (rect.IsHorizontal())
                FireMouseClick(rect.Right - 10, rect.Top + 10);
            else
                FireMouseClick(rect.Left + 10, rect.Bottom - 10);

            SetCursorPos(pos);
        }

        static RECT GetTaskbarRect()
        {
            IntPtr taskbar = GetTaskbarWindow();
            RECT r;
            GetWindowRect(taskbar, out r);
            return r;
        }

        static int ContentHEnd(this Bitmap image)
        {
            int half_hight = image.Height / 2;

            Color? curent_color = null;
            int test_point = image.Width - 1;

            for (; test_point >= 0; test_point--)
            {
                Color new_color = image.GetPixel(test_point, half_hight);
                if (curent_color.HasValue && new_color != curent_color)
                    break;
                curent_color = new_color;
            }
            return test_point;
        }

        static Bitmap HCrop(this Bitmap image, int left, int right)
        {
            var section_rect = new Rectangle(left,
                                             0,
                                             image.Width - left - right,
                                             image.Height);

            return image.Clone(section_rect, image.PixelFormat);
        }

        static Bitmap GetHSquareSection(this Bitmap image, int index, int? sectionWidth = null)
        {
            var section_rect = new Rectangle(sectionWidth ?? image.Height * index,
                                             0,
                                             image.Height,
                                             image.Height);

            return image.Clone(section_rect, image.PixelFormat);
        }

        static void StartMonitoringTaskbarForUAC()
        {
            RECT? taskbarLastItemRectangle = null;

#pragma warning disable CS8321 // Local function is declared but never used
            bool CheckForUACTaskbarItem()
#pragma warning restore CS8321 // Local function is declared but never used
            {
                Bitmap image = GetWindowBitmap(Win32.GetTaskbarWindow());

                var rect = Win32.GetTaskbarRect();

                int item_size = image.Height; // it's roughly square

                int item_right = image.ContentHEnd();
                int item_left_offset = item_right - item_size;
                int item_right_offset = image.Width - item_right;

                image = image.HCrop(item_left_offset, item_right_offset);

                rect.Left += item_left_offset;
                rect.Right -= item_right_offset;

                if (taskbarLastItemRectangle.HasValue)
                {
                    if (Math.Abs(rect.Left - taskbarLastItemRectangle.Value.Left) >= item_size)
                    {
                        // new item has appeared on the taskbar

                        var half_size = item_size / 2;

                        // MessageBox.Show("Please activate the UAC prompt on the taskbar.");
                        Win32.FireMouseClick(rect.Left + half_size, rect.Top + half_size);

                        return true;
                    }
                }

                taskbarLastItemRectangle = rect;
                return false;
            }
        }

        [Flags]
        enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        static void FireMouseClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        static void SetCursorPos(POINT p)
        {
            SetCursorPos(p.x, p.y);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        static POINT GetCursorPos()
        {
            POINT p;
            GetCursorPos(out p);
            return p;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        static bool IsHorizontal(this RECT r)
        {
            return (r.Bottom - r.Top) < (r.Right - r.Left);
        }

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("user32.dll")]
        static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        static Bitmap GetWindowBitmap(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);

            Bitmap bmp = new Bitmap(rc.Right - rc.Left, rc.Bottom - rc.Top, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap;
            try
            {
                hdcBitmap = gfxBmp.GetHdc();
            }
            catch
            {
                return null;
            }
            bool succeeded = PrintWindow(hwnd, hdcBitmap, 0);
            gfxBmp.ReleaseHdc(hdcBitmap);
            if (!succeeded)
            {
                gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, bmp.Size));
            }
            IntPtr hRgn = CreateRectRgn(0, 0, 0, 0);
            GetWindowRgn(hwnd, hRgn);
            Region region = Region.FromHrgn(hRgn);//err here once
            if (!region.IsEmpty(gfxBmp))
            {
                gfxBmp.ExcludeClip(region);
                gfxBmp.Clear(Color.Transparent);
            }
            gfxBmp.Dispose();
            return bmp;
        }
    }
}