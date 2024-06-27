// Ignore Spelling: Deconstruct

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WixToolset.Dtf.WindowsInstaller;

#pragma warning disable CA1416 // Validate platform compatibility

namespace WixSharp
{
    /// <summary>
    /// A utility for creating disconnected MSI session.
    /// </summary>
    public static class DisconnectedSession
    {
        /// <summary>
        /// Creates the instance of the disconnected Session.
        /// </summary>
        /// <returns></returns>
        public static Session Create()
        {
            var constr = typeof(Session).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();

            var constrArgs = new object[] { (IntPtr)1, false };

            var result = constr.Invoke(constrArgs);

            return (Session)result;
        }
    }

    static class ReflectionExtensions
    {
        public static object Call(this MethodInfo method, params object[] args)
        {
            object instance = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);
            return method.Invoke(instance, args);
        }
    }

    class WixItems : WixObject
    {
        public IEnumerable<WixObject> Items;

        public WixItems(IEnumerable<WixObject> items)
        {
            Items = items;
        }
    }

    /// <summary>
    ///
    /// </summary>
    [SuppressUnmanagedCodeSecurity, SecurityCritical]
    public static class Native
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

        /// <summary>
        /// Displays the message box. This method is native and has no dependency on WinForms or WPF. Thus it is very
        /// useful when you need to show message box to the user from the AOT compiled assembly.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        public static int MessageBox(string message, string title = "") => MessageBox(GetForegroundWindow(), message, title, 0);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GetUserPreferredUILanguages(uint dwFlags, out uint pulNumLanguages, char[] pwszLanguagesBuffer, ref uint pcchLanguagesBuffer);

        /// <summary>
        /// Gets the preferred ISO two-letter UI languages.
        /// <para>On Windows Vista and later, the user UI language is the first language in the user preferred UI languages list.</para>
        /// <para>Source: https://github.com/MicrosoftDocs/win32/blob/docs/desktop-src/Intl/user-interface-language-management.md)</para>
        /// </summary>
        /// <returns>The list of preferred ISO two-letter UI languages</returns>
        public static string[] GetPreferredIsoTwoLetterUILanguages()
        {
            const uint MUI_LANGUAGE_NAME = 0x8; // Use ISO language (culture) name convention

            uint languagesCount, languagesBufferSize = 0;

            if (Native.GetUserPreferredUILanguages(MUI_LANGUAGE_NAME, out languagesCount, null, ref languagesBufferSize))
            {
                char[] languagesBuffer = new char[languagesBufferSize];
                if (Native.GetUserPreferredUILanguages(MUI_LANGUAGE_NAME, out languagesCount, languagesBuffer, ref languagesBufferSize))
                {
                    List<string> result = new List<string>((int)languagesCount);
                    string[] languages = new string(languagesBuffer, 0, (int)languagesBufferSize - 2).Split('\0');
                    // Console.WriteLine("GetUserPreferredUILanguages returns " + languages.Length + " languages:");
                    foreach (string language in languages)
                    {
                        //
                        // Register as ISO two letter language when format is xx-xx.
                        //
                        if (language.Length == 5 && language[2] == '-')
                        {
                            result.Add(language.Substring(0, 2));
                        }
                    }

                    return result.ToArray();
                }
                else
                {
                    Console.WriteLine("GetUserPreferredUILanguages(2) error: " + Marshal.GetLastWin32Error());

                    return null;
                }
            }
            else
            {
                Console.WriteLine("GetUserPreferredUILanguages(1) error: " + Marshal.GetLastWin32Error());

                return null;
            }
        }
    }
}