using System.Diagnostics;
using System.Windows.Automation;
using static WindowAutomation;

class Program
{
    static void print(string text) => Console.WriteLine(text);

    static void Main(string[] args)
    {
        // Debugger.Launch();
        // ui_wpf_OnMsiLaunch(@"D:\dev\wixsharp4\Source\src\WixSharp.Test\bin\Debug\MyProduct.msi"); return;

        if (args.Length != 2)
        {
            Console.WriteLine("No enough arguments provided.");
            return;
        }

        var msi = args.Last();

        if (args[0] == "ui-wpf.OnMsiLaunch") ui_wpf_OnMsiLaunch(msi);
    }

    static void ui_wpf_OnMsiLaunch(string msi)
    {
        Process.Start("msiexec", $"/i \"{msi}\"");

        WindowAutomation

            .WaitFor(() => FindWindowByPartialTitle("MyProduct"))
            .Click("Next")
            .Print("Clicked Next on Welcome screen")

            .WaitFor(x => x.VisibleTextAnywhere("End-User"))
            .Click("I accept the terms in the License Agreement")
            .Click("Next")
            .Print("Clicked Next on License Agreement screen")

            .WaitFor(x => x.VisibleTextAnywhere("Choose Setup Type"))
            .Click("Complete")
            .Print("Clicked Next on Setup Type screen")

            .WaitFor(x => x.VisibleTextAnywhere("was interrupted"))
            .Click("Finish")
            .Print("Clicked Next on Exit screen");
    }
}