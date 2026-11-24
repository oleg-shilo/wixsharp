// Ignore Spelling: Aot pfx Wix Dirs Clr Fx Wxl Dialogs Exe Unelevate

#region Licence...

/*
The MIT License (MIT)
Copyright (c) 2014 Oleg Shilo
Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.Controls;
using WixSharp.Nsis;
using WixSharp.UI;
using WixSharp.Utilities;
using WixToolset.Dtf.WindowsInstaller;
using IO = System.IO;

namespace WixSharp.CommonTasks
{

    /// <summary>
    /// Provides utilities for recording method invocations and querying or clearing them by caller.
    /// It is used to assist with the automated testing. It is a light weight form of mocking for testing purposes. 
    /// It is not intended to be used in production code.
    /// </summary>
    public static class InvokeRegister
    {
        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public static bool Enabled = false;
        /// <summary>
        /// Stores invocation log entries keyed by the registered command name.
        /// </summary>
        public static Dictionary<string, List<string>> InvokeLog = new Dictionary<string, List<string>>();

        /// <summary>
        /// Records the current call stack under the specified method name.
        /// </summary>
        /// <param name="methodName">The name associated with the call stack. Defaults to the calling member's name.</param>
        public static void Record([CallerMemberName] string methodName = null)
        {
            if (Enabled)
            {
                lock (InvokeLog)
                {
                    if (!InvokeLog.ContainsKey(methodName))
                        InvokeLog[methodName] = new List<string>();
                    InvokeLog[methodName].Add(GetCallStack());
                }
            }
        }
        /// <summary>
        /// Determines whether the specified method has been invoked.
        /// </summary>
        /// <param name="methodName">The name of the method to check.</param>
        /// <returns>true if the method has been invoked; otherwise, false.</returns>
        public static bool WasInvoked(string methodName)
        {
            lock (InvokeLog)
            {
                return InvokeLog.ContainsKey(methodName) && InvokeLog[methodName].Count > 0;
            }
        }

        /// <summary>
        /// Determines whether the specified method was invoked from the specified caller method.
        /// </summary>
        /// <param name="methodName">The name of the method to check.</param>
        /// <param name="callerMethod">The name of the caller method to search for in the recorded invocation stacks.</param>
        /// <returns>true if the invocation log contains the specified method and a recorded stack contains the caller method;
        /// otherwise, false.</returns>
        public static bool WasInvokedFrom(string methodName, string callerMethod)
        {
            lock (InvokeLog)
            {
                return InvokeLog.ContainsKey(methodName) && InvokeLog[methodName].Count > 0
                    && InvokeLog[methodName].Any(stack => stack.Contains(callerMethod));
            }
        }
        /// <summary>
        /// Removes invocation log entries for the specified method that contain the caller method.
        /// </summary>
        /// <param name="methodName">The method name whose invocation log entries are filtered.</param>
        /// <param name="callerMethod">The text to search for in each invocation stack.</param>
        /// <returns>true if the invocation log contains an entry for methodName; otherwise, false.</returns>
        public static bool Clear(string methodName, string callerMethod)
        {
            lock (InvokeLog)
            {
                if (InvokeLog.ContainsKey(methodName))
                {
                    InvokeLog[methodName] = InvokeLog[methodName]
                        .Where(stack => !stack.Contains(callerMethod))
                        .ToList();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the call stack frames that include source-location information.
        /// </summary>
        /// <returns>The selected call stack frames, joined by newline characters.</returns>
        public static string GetCallStack()
        {
            var stackTraceString = Environment.StackTrace
                .GetLines()
                .Where(x => x.Contains(" in "))
                .Select(x =>
                {
                    var index = x.IndexOf(" in ");
                    return x.Substring(0, index).Replace("at", "");
                })
                .JoinBy("\n");
            return stackTraceString;
        }
    }
}