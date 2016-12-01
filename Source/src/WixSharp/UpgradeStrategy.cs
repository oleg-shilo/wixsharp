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
#endregion
namespace WixSharp
{
    /// <summary>
    /// This class defines the Major Upgrade scenario.
    /// </summary>
    ///
    ///<example>The following is an example of building product MSI with auto uninstalling of any older version of the product
    ///and preventing downgrading.
    ///<code>
    /// var project = new Project("My Product",
    ///                   ...
    ///
    /// project.MajorUpgradeStrategy =  new MajorUpgradeStrategy
    ///                                 {
    ///                                     UpgradeVersions = VersionRange.OlderThanThis,
    ///                                     PreventDowngradingVersions = VersionRange.NewerThanThis,
    ///                                     NewerProductInstalledErrorMessage = "Newer version already installed"
    ///                                 };
    /// Compiler.BuildMsi(project);
    /// </code>
    /// or the same scenario but using predefined <c>MajorUpgradeStrategy.Default</c> strategy.
    ///<code>
    /// var project = new Project("My Product",
    ///                   ...
    ///
    /// project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class MajorUpgradeStrategy
    {
        /// <summary>
        /// Range of versions which should be automatically uninstalled during the installation of the product MSI being built.
        /// <para>Typically these are all versions older then the version being installed. </para>
        /// </summary>
        public VersionRange UpgradeVersions;
        /// <summary>
        /// Range of versions, which should not be automatically uninstalled during the installation of the product MSI being built.
        /// <para>Typically these are all versions newer then the version being installed (downgrade scenario). </para>
        /// </summary>
        public VersionRange PreventDowngradingVersions;
        /// <summary>
        /// Step, which determines when the RemoveExistingProducts standard action is to be performed.
        /// <para>The default value is <c>InstallFinalize</c>. </para>
        /// </summary>
        public Step RemoveExistingProductAfter = Step.InstallFinalize;
        /// <summary>
        /// Error message to be displayed if setup is aborted because of restricted downgrade attempt (see <see cref="PreventDowngradingVersions"/>).
        /// </summary>
        public string NewerProductInstalledErrorMessage;
        /// <summary>
        /// Default <see cref="MajorUpgradeStrategy"/>. Ensures that all older versions of the product are automatically uninstalled and if
        /// a newer version is detected, <c>"Newer version already installed"</c> message is displayed.
        /// </summary>
        public static MajorUpgradeStrategy Default = new MajorUpgradeStrategy
        {
            UpgradeVersions = VersionRange.OlderThanThis,
            PreventDowngradingVersions = VersionRange.NewerThanThis,
            NewerProductInstalledErrorMessage = "Newer version already installed",
        };
    }
}


