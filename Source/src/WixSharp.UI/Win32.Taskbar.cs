using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace WixSharp
{
    public static partial class Win32
    {
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);

        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int X, int Y);

        // [DllImport("user32.dll", SetLastError = true)]
        // public static extern IntPtr FindWindow(string className, string windowTitle);

        // [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        // internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        // internal static extern int GetWindowTextLength(IntPtr hWnd);

        delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

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
        internal static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        static string GetWindowClass(IntPtr hWnd)
        {
            var class_name = new StringBuilder(1024);
            var t = GetClassName(hWnd, class_name, class_name.Capacity);

            return class_name.ToString();
        }

        // [DllImport("user32.dll")]
        // internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // public static string GetWindowText(IntPtr wnd)
        // {
        //     int length = GetWindowTextLength(wnd);
        //     var sb = new StringBuilder(length + 1);
        //     GetWindowText(wnd, sb, sb.Capacity);
        //     return sb.ToString();
        // }

        // [StructLayout(LayoutKind.Sequential)]
        // public struct RECT
        // {
        //     public int Left;
        //     public int Top;
        //     public int Right;
        //     public int Bottom;
        // }

        static void FireMouseClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
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

        static IntPtr GetTaskbarWindow()
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

        static void StartMonitoringTaskbarForUAC()
        {
            Win32.RECT? taskbarLastItemRectangle = null;
#pragma warning disable 8321
            bool ChechForUACTaskbarItem()
            {
                Bitmap image = Win32.GetWindowBitmap(Win32.GetTaskbarWindow());

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

            ThreadPool.QueueUserWorkItem(x =>
            {
                bool done = false;
                while (!done)
                {
                    // if (ChechForUACTaskbarItem())
                    //     break;
                    Thread.Sleep(1000);
                    Win32.SetForegroundWindow(Win32.GetTaskbarWindow());
                }
            });
        }

        static int count = 0;

        static void KeepTaskbarFocused()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                bool done = false;
                while (!done)
                {
                    Thread.Sleep(1000);
                    count++;
                    if (count < 4)
                    {
                        Win32.SetActiveWindow(Win32.GetTaskbarWindow());
                        // Win32.SetForegroundWindow(Win32.GetTaskbarWindow());
                    }
                }
            });
        }
    }
}