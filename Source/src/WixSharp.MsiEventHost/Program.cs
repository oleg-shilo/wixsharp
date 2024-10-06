using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

namespace MsiEventHost
{
    internal static class Program
    {
        static string GetArg(this string[] args, string name)
        {
            return args.FirstOrDefault(arg => arg.StartsWith(name))?.Split(new[] { ':' }, 2)?.LastOrDefault();
        }

        static int Main(string[] args)
        {
            // MessageBox.Show("MsiEventHost");
            // Debug.Assert(false, "MsiEventHost");

            // Note, the EventHost is always executed with "runas".
            // The proper Windows elevation scenario requires the executable to be compiled with the elevation request
            // embedded in the exe manifest. However in this case Windows Defender may block the execution of the exe.
            // Even though the file is signed, executed from the elevated context.
            // And yet it is happy to allow the execution of the exe if it is executed with "runas".
            // Yes that crazy!!!!
            // This is the reason why `requireAdministrator` is commented out in the manifest file.

            // CLI: -event:{eventName} -session:{sessionFile} -log:{logFile}
            var eventName = args.GetArg("-event:");
            var sessionFile = args.GetArg("-session:");
            var logFile = args.GetArg("-log:");

            try
            {
                Session session = DisconnectedSession.Create();
                session.DeserializeAndUpdateFrom(System.IO.File.ReadAllText(sessionFile));
                return (int)ManagedProject.InvokeClientHandlersInternally(session, eventName, null);
            }
            catch (Exception e)
            {
                if (logFile.IsNotEmpty())
                    System.IO.File.WriteAllText(logFile, e.ToString());
                return (int)ActionResult.Failure;
            }
        }
    }
}