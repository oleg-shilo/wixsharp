#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2015 Oleg Shilo

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
#endregion
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Defines the file driver installation approach.
    /// </summary>
    ///<example>The following is an example of installing <c>driver.sys</c> file.
    ///<code>
    /// var project = new Project("MyProduct",
    ///                   new Dir(@"%ProgramFiles%\My Company\My Device",
    ///                         new File("driver.sys",
    ///                                  new DriverInstaller
    ///                                  {
    ///                                      AddRemovePrograms = false,
    ///                                      DeleteFiles = false,
    ///                                      Legacy = true,
    ///                                      PlugAndPlayPrompt = false,
    ///                                      Sequence = 1,
    ///                                      Architecture = DriverArchitecture.x64
    ///                                  })));
    ///         ...
    ///project.BuildMsi();
    /// </code>
    /// </example>
    public partial class DriverInstaller : WixEntity
    {
        /// <summary>
        /// Specifies that the DIFxApp CustomActions should add an entry in the Add/Remove Programs Control Panel applet. The default is 'true'.
        /// </summary>
        public bool? AddRemovePrograms;

        /// <summary>
        /// If set to "true", configures DIFxApp to delete binary files that were copied to the system from the driver store when a driver package 
        /// was installed. If this attribute is set to "no" or not present, DIFxApp does not remove these files from a system. Note that configuring 
        /// DIFxApp to delete these files is controlled by the Flags entry value of the component that represents the driver package in the 
        /// MsiDriverPackages custom table. Setting DeleteFiles to "true" sets the corresponding bit in the Flags entry value. Setting DeleteFiles 
        /// to "false" clears the corresponding bit in the Flags entry value. If this attribute is not present, DIFxApp uses a default value of "false".
        /// </summary>
        public bool? DeleteFiles;

        /// <summary>
        /// If set to "true", configures DIFxApp to install unsigned driver packages and driver packages with missing files. For more information, 
        /// see "Installing Unsigned Driver Packages in Legacy Mode" earlier in this paper. If this attribute is set to "false" or not present, DIFxApp 
        /// will install only signed driver packages. Note that configuring DIFxApp to install unsigned drivers is controlled by the Flags entry value 
        /// of the component that represents the driver package in the MsiDriverPackages custom table. Setting Legacy to "true" sets the corresponding 
        /// bit in the Flags entry value. Setting Legacy to "false" clears the bit in the Flags entry value that configures DIFxApp to install unsigned 
        /// driver packages. If this attribute is not present, DIFxApp uses a default value of "false".
        /// </summary>
        public bool? ForceInstall;

        /// <summary>
        /// If set to "true", configures DIFxApp to install unsigned driver packages and driver packages with missing files. For more information, 
        /// see "Installing Unsigned Driver Packages in Legacy Mode" earlier in this paper. If this attribute is set to "false" or not present, 
        /// DIFxApp will install only signed driver packages. Note that configuring DIFxApp to install unsigned drivers is controlled by the Flags 
        /// entry value of the component that represents the driver package in the MsiDriverPackages custom table. Setting Legacy to "true" sets the 
        /// corresponding bit in the Flags entry value. Setting Legacy to "false" clears the bit in the Flags entry value that configures DIFxApp to 
        /// install unsigned driver packages. If this attribute is not present, DIFxApp uses a default value of "false"
        /// </summary>
        public bool? Legacy;

        /// <summary>
        /// Specifies that the DIFxApp CustomActions should prompt the user to connect the Plug and Play device if it is not connected. The default is 'true'.
        /// </summary>
        public bool? PlugAndPlayPrompt;

        /// <summary>
        /// Specifies an optional installation sequence number. DIFxApp CustomActions install the driver packages in an installation package in the order 
        /// of increasing sequence numbers. The same sequence number can be used for more than one driver; however, the order in which packages with the 
        /// same sequence number are actually installed cannot be determined.
        /// </summary>
        public int? Sequence;
        /// <summary>
        /// The architecture of the driver to be installed. Default value is 'x86'
        /// </summary>
        public DriverArchitecture Architecture = DriverArchitecture.x86;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XContainer[] ToXml()
        {
            var element = new XElement(WixExtension.Difx.ToXNamespace() + "Driver")
                                  .SetAttribute("AddRemovePrograms", AddRemovePrograms)
                                  .SetAttribute("DeleteFiles", DeleteFiles)
                                  .SetAttribute("ForceInstall", ForceInstall)
                                  .SetAttribute("Legacy", Legacy)
                                  .SetAttribute("PlugAndPlayPrompt", PlugAndPlayPrompt)
                                  .SetAttribute("Sequence", Sequence)
                                  .AddAttributes(this.Attributes); 

            return new[] { element };
        }
    }

    internal static class DriverInstallerCompiling
    {
        public static DriverInstaller Compile(this DriverInstaller driver, Project project, XElement component)
        {
            if (driver != null)
            {
                component.Add(driver.ToXml());
                project.IncludeWixExtension(WixExtension.Difx);
                project.LibFiles.Add(System.IO.Path.Combine(Compiler.WixLocation, "difxapp_{0}.wixlib".FormatWith(driver.Architecture)));
            }
            return driver;
        }
    }
}