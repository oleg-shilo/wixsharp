using System;

[assembly: Xunit.TestFramework("WixSharp.Test.WixLocator", "WixSharp.Test")]

namespace WixSharp.Test
{
    // public class WixLocator
    // {
    //     static bool done = false;

    //     public WixLocator()
    //     {
    //         if (!done)
    //         {
    //             done = true;

    //             var asm_file = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;

    //             var wix_dir = asm_file.PathGetDirName()
    //                 .PathJoin(@"..\..\..\WixSharp.Samples\Wix_bin\bin")
    //                 .PathGetFullPath();

    //             Compiler.WixLocation = wix_dir;
    //         }
    //     }
    // }
}