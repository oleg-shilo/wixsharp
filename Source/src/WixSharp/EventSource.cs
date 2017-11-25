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
    ///<example>The following is an example of creating an event source "ROOT Builder".
    ///<code>
    ///var project =
    /// new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
    ///         new EventSource
    ///         {
    ///             Name = "ROOT Builder",
    ///             Log = "Application",
    ///             EventMessageFile = @"%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll"
    ///         },
    ///         ...
    ///
    /// project.BuildMsi();
    /// </code>
    /// </example>
    public class EventSource : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Name of the event source.
        /// </summary>
        [Xml]
        new public string Name;

        /// <summary>
        ///	Name of the event source's log.
        /// </summary>
        [Xml]
        public string Log;

        /// <summary>
        /// Creates an event source.
        /// </summary>
        ///<example>The following is an example of creating an event source "ROOT Boilder".
        ///<code>
        ///var project =
        /// new Project("MyProduct",
        ///     new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
        ///         new EventSource
        ///         {
        ///             Name = "ROOT Builder",
        ///             Log = "Application",
        ///             EventMessageFile = @"%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll"
        ///         },
        ///         ...
        ///
        /// project.BuildMsi();
        /// </code>
        /// </example>
        [Xml]
        public string EventMessageFile;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.IncludeWixExtension(WixExtension.Util);

            context.XParent
                   .FindFirst("Component")
                   .Add(this.ToXElement(WixExtension.Util, "EventSource"));
        }
    }
}