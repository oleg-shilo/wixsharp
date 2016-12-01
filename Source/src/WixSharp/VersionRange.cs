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

namespace WixSharp
{
    /// <summary>
    /// This class represents a range of product versions. It is designed to be used for
    /// Major/Minor upgrade scenarios in conjunction with Upgrade strategy classes (e.g. <see cref="MajorUpgradeStrategy"/>).
    /// <para>Version is represented as a string to allow for predefined value <c>%this%</c>, which means
    /// "version of the product MSI being built".</para>
    /// <para>This class includes predefined ranges for common upgrade scenarios.</para>
    /// </summary>
    public class VersionRange
    {
        /// <summary>
        /// Minimum value of the Version range.
        /// <para>If <c>"%this%"</c> is used the Minimum value will set to the version of the product MSI being built.</para>
        /// </summary>
        public string Minimum;

        /// <summary>
        /// Maximum value of the Version range.
        /// <para>If <c>"%this%"</c> is used the Maximum value will set to the version of the product MSI being built.</para>
        /// </summary>
        public string Maximum;

        /// <summary>
        /// Indicates if version range includes <see cref="Maximum"/> value.
        /// </summary>
        public bool? IncludeMaximum;

        /// <summary>
        /// Indicates if version range includes <see cref="Minimum"/> value.
        /// </summary>
        public bool? IncludeMinimum;

        /// <summary>
        /// Set to true to migrate feature states from upgraded products by enabling the logic in the MigrateFeatureStates action
        /// </summary>
        public bool? MigrateFeatures;

        /// <summary>
        /// Set to <c>true</c> to detect products and applications but do not uninstall.
        /// </summary>
        public bool? OnlyDetect; 

        /// <summary>
        /// Predefined "open ended" range between version of the product MSI being built (inclusive) and any newer version.
        /// </summary>
        static public VersionRange ThisAndNewer = new VersionRange
        {
            Minimum = "%this%",
            IncludeMaximum = true,
        };

        /// <summary>
        /// Predefined "open ended" range between version of the product MSI being built (exclusive) and any newer version.
        /// </summary>
        static public VersionRange NewerThanThis = new VersionRange
        {
            Minimum = "%this%",
            IncludeMinimum = false,
        };

        /// <summary>
        /// Predefined range of versions between <c>0.0.0.0</c> (inclusive) and the version of the product MSI being built (exclusive).
        /// </summary>
        static public VersionRange OlderThanThis = new VersionRange
        {
            Minimum = "0.0.0.0",
            Maximum = "%this%",
            IncludeMinimum = true,
            IncludeMaximum = false,
        };

        /// <summary>
        /// Predefined range of versions between <c>0.0.0.0</c> (inclusive) and the version of the product MSI being built (inclusive).
        /// </summary>
        static public VersionRange ThisAndOlder = new VersionRange
        {
            Minimum = "0.0.0.0",
            Maximum = "%this%",
            IncludeMinimum = true,
            IncludeMaximum = true,
        };
    }
}