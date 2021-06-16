using System;
using System.Linq;
using System.Text;

namespace WixSharp.Msiexec
{
    internal static class MsiexecLogCommand
    {
        internal static string Generate(string logFilePath, MsiexecLogSwitches flags = MsiexecLogSwitches.None)
        {
            if (logFilePath == null)
            {
                return string.Empty;
            }
            
            if (logFilePath.Trim().Length == 0)
            {
                return string.Empty;
            }

            var commandBuilder = new StringBuilder(" /L");

            if (flags != MsiexecLogSwitches.None)
            {
                ProcessLogSwitches(commandBuilder, flags);
            }

            return commandBuilder.Append($" \"{logFilePath}\"").ToString();
        }

        private static void ProcessLogSwitches(StringBuilder commandBuilder, MsiexecLogSwitches flags)
        {
            var allPossibleSwitches = Enum.GetValues(typeof(MsiexecLogSwitches)).Cast<MsiexecLogSwitches>();

            foreach (var val in allPossibleSwitches)
            {
                if (flags.HasFlag(val))
                {
                    commandBuilder.Append(val.GetDescription());
                }
            }
        }
    }
}