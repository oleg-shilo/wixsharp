using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    //https://msdn.microsoft.com/en-us/library/aa368032.aspx

    /// <summary>
    /// Defines product properties as they appear in 'Add/Remove Programs' of Control Panel.
    /// <example>
    /// <code>
    ///  var project = new Project("MyProduct",
    ///                    new Dir(@"%ProgramFiles%\My Company\My Product",
    ///                    ...
    /// project.ControlPanelInfo.Comments = "Simple test msi";
    /// project.ControlPanelInfo.Readme = "https://github.com/oleg-shilo/wixsharp/manual";
    /// project.ControlPanelInfo.HelpLink = "https://github.com/oleg-shilo/wixsharp/support";
    /// project.ControlPanelInfo.HelpTelephone = "111-222-333-444";
    /// project.ControlPanelInfo.UrlInfoAbout = "https://github.com/oleg-shilo/wixsharp/About";
    /// project.ControlPanelInfo.UrlUpdateInfo = "https://github.com/oleg-shilo/wixsharp/update";
    /// project.ControlPanelInfo.ProductIcon = "app_icon.ico";
    /// project.ControlPanelInfo.Contact = "Product owner";
    /// project.ControlPanelInfo.Manufacturer = "My Company";
    /// project.ControlPanelInfo.InstallLocation = "[INSTALLDIR]";
    /// project.ControlPanelInfo.NoModify = true;
    /// project.ControlPanelInfo.NoRepair = true,
    /// project.ControlPanelInfo.NoRemove = true,
    /// project.ControlPanelInfo.SystemComponent = true, //if set will not be shown in Control Panel
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public partial class ProductInfo : WixEntity
    {
        /// <summary>
        /// Provides Comments for the Add/Remove Programs in the Control Panel.
        /// </summary>
        [ArpPropertyAttribute("ARPCOMMENTS")]
        public string Comments { get; set; }

        /// <summary>
        /// Provides the Contact for Add/Remove Programs in the Control Panel.
        /// </summary>
        [ArpPropertyAttribute("ARPCONTACT")]
        public string Contact { get; set; }

        /// <summary>
        /// Product manufacturer name
        /// </summary>
        public string Manufacturer = Environment.UserName;

        /// <summary>
        /// Fully qualified path to the application's primary folder.
        /// </summary>
        [ArpPropertyAttribute("ARPINSTALLLOCATION", SetAsAction = true)]
        public string InstallLocation { get; set; }

        /// <summary>
        /// Internet address, or URL, for technical support.
        /// </summary>
        [ArpPropertyAttribute("ARPHELPLINK")]
        public string HelpLink { get; set; }

        /// <summary>
        /// Technical support phone numbers.
        /// </summary>
        [ArpPropertyAttribute("ARPHELPTELEPHONE")]
        public string HelpTelephone { get; set; }

        /// <summary>
        /// Prevents display of a Change button for the product in Add/Remove Programs in the Control Panel. Note  This only affects the display in the ARP. The Windows Installer is still capable of repairing, installing-on-demand, and uninstalling applications through a command line or the programming interface.
        /// </summary>
        [ArpPropertyAttribute("ARPNOMODIFY", SetAsAction = true)]
        public bool? NoModify { get; set; }

        /// <summary>
        /// Prevents display of a Remove button for the product in the Add/Remove Programs in the Control Panel. The product can still be removed by selecting the Change button if the installation package has been authored with a user interface that provides product removal as an option. Note  This only affects the display in the ARP. The Windows Installer is still capable of repairing, installing-on-demand, and uninstalling applications through a command line or the programming interface.
        /// </summary>
        [ArpPropertyAttribute("ARPNOREMOVE")]
        public bool? NoRemove { get; set; }

        /// <summary>
        /// Disables the Repair button in the Add/Remove Programs in the Control Panel. Note  This only affects the display in the ARP. The Windows Installer is still capable of repairing, installing-on-demand, and uninstalling applications through a command line or the programming interface.
        /// </summary>
        [ArpPropertyAttribute("ARPNOREPAIR")]
        public bool? NoRepair { get; set; }

        /// <summary>
        /// Identifies the icon displayed in Add/Remove Programs. If this property is not defined, Add/Remove Programs specifies the display icon.
        /// </summary>
        [ArpPropertyAttribute("ARPPRODUCTICON")]
        public string ProductIcon { get; set; }

        /// <summary>
        /// Provides the ReadMe for Add/Remove Programs in Control Panel.
        /// </summary>
        [ArpPropertyAttribute("ARPREADME")]
        public string Readme { get; set; }

        /// <summary>
        /// Prevents display of the application in the Programs List of the Add/Remove Programs in the Control Panel. Note  This only affects the display in the ARP. The Windows Installer is still capable of repairing, installing-on-demand, and uninstalling applications through a command line or the programming interface.
        /// </summary>
        [ArpPropertyAttribute("ARPSYSTEMCOMPONENT")]
        public bool? SystemComponent { get; set; }

        /// <summary>
        /// URL for application's home page.
        /// </summary>
        [ArpPropertyAttribute("ARPURLINFOABOUT")]
        public string UrlInfoAbout { get; set; }

        /// <summary>
        /// URL for application update information.
        /// </summary>
        [ArpPropertyAttribute("ARPURLUPDATEINFO")]
        public string UrlUpdateInfo { get; set; }

        internal void AddMembersTo(Project project)
        {
            var properties = new List<Property>();
            var actions = new List<Action>();

            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                object value = prop.GetValue(this, new object[0]);

                if (value != null)
                {
                    var attr = (ArpPropertyAttribute)prop.GetCustomAttributes(typeof(ArpPropertyAttribute), false).FirstOrDefault();
                    if (attr != null)
                    {
                        bool propertyExists = project.Properties.Any(a => a.Name == attr.Name);
                        bool actionExists = project.Actions.OfType<SetPropertyAction>().Any(a => a.PropName == attr.Name);

                        if (attr.Name == "ARPPRODUCTICON")
                        {
                            if (!propertyExists)
                            {
                                properties.Add(new Property(attr.Name, "app_icon.ico"));
                                project.Add(new IconFile(new Id("app_icon.ico"), value.ToString()));
                            }
                        }
                        else
                        {
                            if (attr.SetAsAction)
                            {
                                if (!actionExists)
                                    actions.Add(new SetPropertyAction(new Id("Set_" + attr.Name), attr.Name, value.ToString())
                                    {
                                        Step = Step.CostFinalize
                                    });
                            }
                            else
                            {
                                if (!propertyExists)
                                    properties.Add(new Property(attr.Name, value.ToString()));
                            }
                        }
                    }
                }
            }

            project.AddProperties(properties.ToArray());
            project.AddActions(actions.ToArray());
        }

        class ArpPropertyAttribute : Attribute
        {
            public bool SetAsAction = false;
            public string Name;

            public ArpPropertyAttribute(string name)
            {
                this.Name = name;
            }
        }
    }
}