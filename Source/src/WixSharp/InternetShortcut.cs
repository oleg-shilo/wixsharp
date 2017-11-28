using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using static WixSharp.SetupEventArgs;

namespace WixSharp
{
    /// <summary>
    /// Defines Event source for windows Event Log service.
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    ///<example>The following is an example of creating a shortcut to the Wix# website.
    ///<code>
    ///var project =
    /// new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
    ///        new InternetShortcut
    ///        {
    ///            Name = "My Online Documentation",
    ///            Target = "https://github.com/oleg-shilo/wixsharp"
    ///        },
    ///         ...
    ///
    /// project.BuildMsi();
    /// </code>
    /// </example>
    public class InternetShortcut : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Type of shortcut should be created.
        /// </summary>
        ///
        public enum ShortcutType
        {
            /// <summary>
            /// Creates .lnk files using IShellLinkW (default).
            /// </summary>
            link,

            /// <summary>
            /// Creates .url files using IUniformResourceLocatorW
            /// </summary>
            url
        }

        /// <summary>
        /// The name of the shortcut file, which is visible to the user. (The .lnk extension is added automatically and by default, is not shown to the user.)
        /// </summary>
        [Xml]
        new public string Name;

        /// <summary>
        ///	Unique identifier in your installation package for this Internet shortcut.
        /// </summary>
        [Xml]
        public new string Id;

        /// <summary>
        ///	URL that should be opened when the user selects the shortcut. Windows opens the URL in the appropriate handler for the protocol specified in the URL. Note that this is a formatted field, so you can use [#fileId] syntax to refer to a file being installed (using the file: protocol).
        /// </summary>
        [Xml]
        public string Target;

        /// <summary>
        /// Which type of shortcut should be created. This attribute's value must be one of the following:
        /// <para><c>url</c> - Creates .url files using IUniformResourceLocatorW.</para>
        /// <para><c>link</c> - Creates .lnk files using IShellLinkW (default). </para>
        /// </summary>
        [Xml]
        public ShortcutType? Type;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            this.Id = this.Id ?? base.Id; // ensure the auto-generated Id is pushed to the XML attribute
            context.Project.IncludeWixExtension(WixExtension.Util);

            context.XParent
                   .FindFirst("Component")
                   .Add(this.ToXElement(WixExtension.Util, "InternetShortcut"));
        }
    }
}