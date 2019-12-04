using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using WindowsInstaller;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using io = System.IO;

public class Script
{
    static public void Main()
    {
        var product =
            new ManagedProject("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        product.Language = "en-US,de-DE,ru-RU";
        product.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");

        product.PreserveTempFiles = true;
        product.BuildLocalizedMsi();

        // bootstrapper is yet to be added
    }
}