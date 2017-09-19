﻿using System;
using WixSharp;

namespace $safeprojectname$
{
    class Program
    {
        static void Main()
        {
            DON'T FORGET to add NuGet package "WixSharp".

            var project = new Project("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File("Program.cs")));

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }
}