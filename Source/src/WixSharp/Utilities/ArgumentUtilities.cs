using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WixSharp.Utilities
{
    static class ArgumentUtilities
    {
        public static string GetArgumentValue(string[] possiblePreffixes)
        {
            foreach (var preffix in possiblePreffixes)
            {
                string arg = Environment.GetCommandLineArgs().Where(x => x.StartsWith(preffix, true)).LastOrDefault() ?? "";

                if (arg.IsNotEmpty())
                {
                    return arg.Substring(preffix.Length);
                }
            }

            return string.Empty;
        }
    }
}
