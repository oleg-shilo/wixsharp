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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Defines WiX <c>Condition</c>. <c>Condition</c> is normally associated with <c>CustomActions</c> or WiX elements (e.g. <c>Shortcut</c>).
    /// <para>
    /// 		<see cref="Condition"/> is nothing else but an XML friendly string wrapper, containing
    /// some predefined (commonly used) condition values. You can either use one of the
    /// predefined condition values (static members) or define your by specifying full string representation of
    /// the required WiX condition when calling the constructor or static method <c>Create</c>.
    /// </para>
    /// </summary>
    /// <example>The following is an example of initializing the Shortcut.<see cref="Shortcut.Condition"/>
    /// with custom value <c>INSTALLDESKTOPSHORTCUT="yes"</c> and
    /// the InstalledFileAction.<see cref="T:WixSharp.InstalledFileAction.Condition"/> with perefined value <c>NOT_Installed</c>:
    ///   <code>
    /// var project =
    ///     new Project("My Product",
    ///                 ...
    ///                 new Dir(@"%Desktop%",
    ///                     new WixSharp.Shortcut("MyApp", "[INSTALL_DIR]MyApp.exe", "")
    ///                     {
    ///                         Condition = new Condition("INSTALLDESKTOPSHORTCUT=\"yes\"")
    ///                     }),
    ///                 new InstalledFileAction("MyApp.exe", "",
    ///                                         Return.check,
    ///                                         When.After,
    ///                                         Step.InstallFinalize,
    ///                                         Condition.NOT_Installed),
    ///                                         ...
    ///   </code>
    ///   </example>
    public class Condition : WixEntity
    {
        /// <summary>
        /// String value of WiX <c>Condition</c>.
        /// </summary>
        public string Value = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="value">The value of the WiX condition expression.</param>
        public Condition(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expectedValue">The expected value.</param>
        public Condition(string name, string expectedValue)
        {
            Value = $"{name}=\"{expectedValue}\"";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expectedValue">if set to <c>true</c> [expected value].</param>
        public Condition(string name, bool expectedValue)
        {
            Value = $"{name}=\"{expectedValue.ToYesNo()}\"";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expectedValue">The expected value.</param>
        public Condition(string name, int expectedValue)
        {
            Value = $"{name}=\"{expectedValue}\"";
        }

        /// <summary>
        ///  Returns the WiX <c>Condition</c> as a string.
        /// </summary>
        /// <returns>A string representing the condition.</returns>
        public override string ToString()
        {
            return Value.Replace("'", "\"");
        }

        /// <summary>
        /// Extracts the distinct names of properties from the condition string expression.
        /// </summary>
        /// <returns></returns>
        public string[] GetDistinctProperties()
        {
            //"NETFRAMEWORK30_SP_LEVEL and NOT NETFRAMEWORK30_SP_LEVEL='#0'"
            var text = this.ToString();
            string[] parts = text.Split("[]()!=><\t \n\r".ToCharArray());

            var props = parts.Where(x => x.IsNotEmpty() &&
                                        !x.SameAs("AND", true) &&
                                        !x.SameAs("NOT", true) &&
                                        !x.SameAs("OR", true) &&
                                        !x.StartsWith("\""))
                             .Distinct()
                             .ToArray();
            return props;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Condition"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(Condition obj)
        {
            return obj.ToString();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Condition"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Condition(string obj)
        {
            return new Condition(obj);
        }

        /// <summary>
        ///  Returns the WiX <c>Condition</c> as a <see cref="T:System.Xml.Linq.XCData"/>.
        ///  <remarks> Normally <c>Condition</c> is not designed to be parsed by the XML parser thus it should be embedded as CDATA</remarks>
        /// <code>
        /// &lt;Condition&gt;&lt;![CDATA[NETFRAMEWORK20="#0"]]&gt;&lt;/Condition&gt;
        /// </code>
        /// </summary>
        /// <returns>A CDATA string representing the condition.</returns>
        public XCData ToCData()
        {
            return new XCData(Value);
        }

        /// <summary>
        /// String representation of the <c>Custom_UI_Command = "back"</c> condition. This condition is triggered when user presses 'Back' button in the CLR Dialog.
        /// </summary>
        public readonly static Condition ClrDialog_BackPressed = new Condition(" Custom_UI_Command = \"back\" ");

        /// <summary>
        /// String representation of the <c>Custom_UI_Command = "next"</c> condition. This condition is triggered when user presses 'Next' button in the CLR Dialog.
        /// </summary>
        public readonly static Condition ClrDialog_NextPressed = new Condition(" Custom_UI_Command = \"next\" ");

        /// <summary>
        /// String representation of the <c>Custom_UI_Command = "abort"</c> condition. This condition is triggered when user presses 'Cancel' button in the CLR Dialog.
        /// </summary>
        public readonly static Condition ClrDialog_CancelPressed = new Condition(" Custom_UI_Command = \"abort\" ");

        /// <summary>
        /// String representation of the <c>NOT Installed</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition NOT_Installed = new Condition(" (NOT Installed) ");

        /// <summary>
        /// String representation of the <c>Installed</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition Installed = new Condition(" (Installed) ");

        /// <summary>
        /// String representation of "always true" condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition Always = new Condition(" (1) ");

        /// <summary>
        /// String representation of the <c>NOT (REMOVE="ALL")</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition NOT_BeingRemoved = new Condition(" (NOT (REMOVE=\"ALL\")) ");

        /// <summary>
        /// String representation of the <c>UILevel > 3</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition NOT_Silent = new Condition(" (UILevel > 3) ");

        /// <summary>
        /// String representation of the <c>UILevel &lt; 4</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition Silent = new Condition(" (UILevel < 4) ");

        /// <summary>
        /// String representation of the <c>REMOVE="ALL"</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition BeingUninstalled = new Condition(" (REMOVE=\"ALL\") ");

        /// <summary>
        /// Software is being removed and no newer version is being installed.
        /// </summary>
        public static readonly Condition BeingUninstalledAndNotBeingUpgraded = new Condition("(NOT UPGRADINGPRODUCTCODE) AND (REMOVE=\"ALL\")");

        /// <summary>
        /// The .NET2.0 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net20_Installed = new Condition(" (NETFRAMEWORK20='#1') ");

        /// <summary>
        /// The .NET3.5 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net35_Installed = new Condition(" (NETFRAMEWORK35='#1') ");

        /// <summary>
        /// The .NET4.5 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net45_Installed = new Condition(" (NETFRAMEWORK45 >= '#378389') ");

        /// <summary>
        /// The .NET4.5.1 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net451_Installed = new Condition(" (NETFRAMEWORK45 >= '#378675') ");

        /// <summary>
        /// The .NET4.5.2 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net452_Installed = new Condition(" (NETFRAMEWORK45 >= '#379893') ");

        /// <summary>
        /// The .NET4.6 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net46_Installed = new Condition(" (NETFRAMEWORK45 >= '#393295') ");

        /// <summary>
        /// The .NET4.6.1 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net461_Installed = new Condition(" (NETFRAMEWORK45 >= '#394254') ");

        /// <summary>
        /// The .NET4.6.2 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net462_Installed = new Condition(" (NETFRAMEWORK45 >= '#394802') ");

        /// <summary>
        /// The .NET4.7 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net47_Installed = new Condition(" (NETFRAMEWORK45 >= '#460798') ");

        /// <summary>
        /// The .NET4.7.1 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net471_Installed = new Condition(" (NETFRAMEWORK45 >= '#461308') ");

        /// <summary>
        /// The .NET4.7.2 installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net472_Installed = new Condition(" (NETFRAMEWORK45 >= '#461808') ");

        /// <summary>
        /// The .NET3.0 SP installed. This condition is to be used in Project.SetNetFxPrerequisite.
        /// </summary>
        public readonly static Condition Net30_SP_Installed = new Condition(" (NETFRAMEWORK30_SP_LEVEL and NOT NETFRAMEWORK30_SP_LEVEL='#0') ");

        /// <summary>
        /// Creates WiX <c>Condition</c> condition from the given string value.
        /// </summary>
        /// <param name="value">String value of the <c>Condition</c> to be created.</param>
        /// <returns>Instance of the <c>Condition</c></returns>
        /// <example>The following is an example of initializing the Shortcut.<see cref="Shortcut.Condition"/>
        /// with custom value <c>INSTALLDESKTOPSHORTCUT="yes"</c>:
        /// <code>
        ///     new Dir(@"%Desktop%",
        ///         new WixSharp.Shortcut("MyApp", "[INSTALL_DIR]MyApp.exe", "")
        ///         {
        ///            Condition = Condition.Create("INSTALLDESKTOPSHORTCUT=\"yes\"")
        ///         })
        ///
        /// </code>
        /// </example>
        public static Condition Create(string value) { return new Condition(value); }

        /// <summary>
        /// Returns a new <see cref="Condition"/> performing an AND between two conditions.
        /// </summary>
        /// <param name="a">The first <see cref="Condition"/></param>
        /// <param name="b">The second <see cref="Condition"/></param>
        /// <returns>The new AND'ed condition.</returns>
        public static Condition operator &(Condition a, Condition b)
        {
            return new Condition("(" + a + " AND " + b + ")");
        }

        /// <summary>
        /// Returns a new <see cref="Condition"/> performing an OR between two conditions.
        /// </summary>
        /// <param name="a">The first <see cref="Condition"/></param>
        /// <param name="b">The second <see cref="Condition"/></param>
        /// <returns>The new OR'ed condition.</returns>
        public static Condition operator |(Condition a, Condition b)
        {
            return new Condition("(" + a + " OR " + b + ")");
        }

        /// <summary>
        /// Returns a new <see cref="Condition"/> adding a logical NOT before the condition.
        /// </summary>
        /// <param name="x">The first <see cref="Condition"/> or text</param>
        /// <returns>The new NOT'ed condition.</returns>
        public static Condition NOT(string x)
        {
            return new Condition("(NOT " + x + ")");
        }

        //public Condition And(Condition condition)
        //{
        //    return Create("(" + this.ToString() + " AND " + condition.ToString() + ")");
        //}

        //public Condition Or(Condition condition)
        //{
        //    return Create("(" + this.ToString() + " OR " + condition.ToString() + ")");
        //}

        //public Condition Invert()
        //{
        //    return Create("NOT (" + this.ToString() + ")");
        //}
    }

    /// <summary>
    /// Specialized Condition for conditionally installing WiX Features.
    /// </summary>
    /// <remarks>
    /// Setting Attributes on FeatureCondition is ignored.
    /// </remarks>
    public class FeatureCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCondition"/> class.
        /// </summary>
        /// <param name="value">The value of the WiX condition expression.</param>
        /// <param name="level">The level value of the WiX condition.</param>
        public FeatureCondition(string value, int level)
            : base(value)
        {
            Level = level;
        }

        /// <summary>
        /// Allows modifying the level of a Feature based on the result of this condition.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <exception cref="NotImplementedException">Raised when getting or setting Attributes.</exception>
        public new Dictionary<string, string> Attributes
        {
            get { throw new NotImplementedException("Attributes is not a valid property for FeatureCondition"); }
            set { throw new NotImplementedException("Attributes is not a valid property for FeatureCondition"); }
        }
    }
}