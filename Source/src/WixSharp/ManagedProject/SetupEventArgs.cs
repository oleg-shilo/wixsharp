using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    ///
    /// </summary>
    public class SetupEventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public enum SetupMode
        {
            /// <summary>
            /// The installing mode
            /// </summary>
            Installing,

            /// <summary>
            /// The modifying mode
            /// </summary>
            Modifying,

            /// <summary>
            /// The uninstalling mode
            /// </summary>
            Uninstalling,

            /// <summary>
            /// The repairing mode
            /// </summary>
            Reparing,

            /// <summary>
            /// The unknown mode
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public Session Session { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public ActionResult Result { get; set; }

        /// <summary>
        /// Gets name of the product being installed
        /// </summary>
        public string ProductName
        {
            get
            {
                var value = Session?.Property("ProductName") ?? "";
                return value.IsEmpty() ? Data["ProductName"] : value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Authored UI and wizard dialog boxes suppressed.
        /// </summary>
        /// <value>
        /// <c>true</c> if UI is suppressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsUISupressed { get { return UILevel <= 4; } }

        /// <summary>
        /// Gets the UIlevel.
        /// <para>UILevel > 4 lead to displaying modal dialogs. See https://msdn.microsoft.com/en-us/library/aa369487(v=vs.85).aspx. </para>
        /// </summary>
        /// <value>
        /// The UI level.
        /// </value>
        public int UILevel { get { return Data["UILevel"].ToInt(-1); } }

        /// <summary>
        /// The managed UI main window object. It is a main System.Windows.Forms.Form window of the standard Wix# embedded UI.
        /// <para>This member is only populated when it is handled by the <see cref="T:WixSharp.ManagedProject.UILoaded"/> event handler.
        /// It has the default <c>null</c> value for all other events.</para>
        /// </summary>
        [Obsolete(message: "This member name is obsolete use `ManagedUI` instead")]
        public IShellView ManagedUIShell { get { return ManagedUI; } set { ManagedUI = value; } }

        /// <summary>
        /// The managed UI main window object. It is a main System.Windows.Forms.Form window of the standard Wix# embedded UI.
        /// <para>This member is only populated when it is handled by the <see cref="T:WixSharp.ManagedProject.UILoaded"/> event handler.
        /// It has the default <c>null</c> value for all other events.</para>
        /// </summary>
        public IShellView ManagedUI = null;

        /// <summary>
        /// Gets a value indicating whether the event handler is executed from the elevated context.
        /// </summary>
        /// <value>
        /// <c>true</c> if the execution context is elevated; otherwise, <c>false</c>.
        /// </value>
        public bool IsElevated { get { return WindowsIdentity.GetCurrent().IsAdmin(); } }

        /// <summary>
        /// Gets a value indicating whether the product is installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product is installed; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalled { get { return Data["Installed"].IsNotEmpty(); } }

        /// <summary>
        /// Gets a value indicating whether the product is being installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if installing; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalling { get { return !IsInstalled && !Data["REMOVE"].SameAs("ALL", ignoreCase: true); } }

        /// <summary>
        /// Gets a value indicating whether the installed product is being modified.
        /// </summary>
        /// <value>
        /// <c>true</c> if modifying; otherwise, <c>false</c>.
        /// </value>
        public bool IsModifying { get { return IsInstalled && !Data["REINSTALL"].SameAs("ALL", ignoreCase: true); } }

        /// <summary>
        /// Gets a value indicating whether the installed product is being upgraded.
        /// <para>
        /// This property relies on "UPGRADINGPRODUCTCODE" MSI property, which is not set by MSI until previous version is uninstalled. Thus it may not be the
        /// most practical way of detecting upgrades. Use AppSearch.GetProductVersionFromUpgradeCode as a more reliable alternative.
        /// </para>
        /// </summary>
        /// <value>
        /// <c>true</c> if modifying; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpgrading { get { return IsModifying && UpgradingProductCode.IsNotEmpty(); } }

        /// <summary>
        /// Gets a value indicating whether the installed product is being repaired.
        /// </summary>
        /// <value>
        /// <c>true</c> if repairing; otherwise, <c>false</c>.
        /// </value>
        public bool IsRepairing { get { return IsInstalled && Data["REINSTALL"].SameAs("ALL", ignoreCase: true); } }

        /// <summary>
        /// Gets a value indicating whether the installed product is being uninstalled.
        /// </summary>
        /// <value>
        /// <c>true</c> if uninstalling; otherwise, <c>false</c>.
        /// </value>
        public bool IsUninstalling { get { return Data["REMOVE"].SameAs("ALL", ignoreCase: true); } }

        /// <summary>
        /// Gets the action name from the standard Change/Modify dialog. The action is stored in <c>MODIFY_ACTION</c>
        /// session property. And it can be one of these predefined values: Change, Repair or Remove.
        /// <para>Note this value is set only by ManagedUI Maintenance dialog.</para>
        /// </summary>
        /// <value>The modify action.</value>
        public string ModifyAction { get { return Session.Property("MODIFY_ACTION"); } }

        /// <summary>
        /// Gets the msi file location.
        /// </summary>
        /// <value>
        /// The msi file.
        /// </value>
        public string MsiFile { get { return Data["MsiFile"] ?? Session.Property("OriginalDatabase"); } } // Data may not be initializaed it 

        /// <summary>
        /// Gets the setup mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public SetupMode Mode
        {
            get
            {
                if (IsInstalling) return SetupMode.Installing;
                if (IsModifying) return SetupMode.Modifying;
                if (IsUninstalling) return SetupMode.Uninstalling;
                if (IsRepairing) return SetupMode.Reparing;
                return SetupMode.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the install directory.
        /// </summary>
        /// <value>
        /// The install dir.
        /// </value>
        public string InstallDir
        {
            get { return Session.Property("INSTALLDIR"); }
            set { Session["INSTALLDIR"] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this session is started via ManagedUI.
        /// </summary>
        /// <value>
        /// <c>true</c> if this session is a managed UI session; otherwise, <c>false</c>.
        /// </value>
        public bool IsManagedUISession
        {
            //depending on what stage of the MSI session IsManagedUISession call is made some of the properties
            //may or may not be available
            get
            {
                return Session.Property("WIXSHARP_MANAGED_UI").IsNotEmpty() ||
                       Session.Property("WIXSHARP_MANAGED_UI_HANDLE").IsNotEmpty() ||
                       Data.ContainsKey("WIXSHARP_MANAGED_UI_HANDLE");
            }
        }

        /// <summary>
        /// Gets the ManagedUI window handle.
        /// </summary>
        /// <value>The window handle.</value>
        public IntPtr ManagedUIHandle
        {
            get
            {
                int handle;
                string value = Session.Property("WIXSHARP_MANAGED_UI_HANDLE");
                if (value.IsEmpty() && Data.ContainsKey("WIXSHARP_MANAGED_UI_HANDLE"))
                    value = Data["WIXSHARP_MANAGED_UI_HANDLE"];
                int.TryParse(value, out handle);
                return new IntPtr(handle);
            }
        }

        /// <summary>
        /// Gets the upgrading product code. Note this property is normally set by MSI for the
        /// uninstall session being triggered by upgrading to a higher version.
        /// </summary>
        /// <value>The upgrading product code.</value>
        public string UpgradingProductCode
        {
            get { return Data["UPGRADINGPRODUCTCODE"]; }
        }

        /// <summary>
        /// Gets the product code.
        /// </summary>
        /// <value>
        /// The product code.
        /// </value>
        public string ProductCode
        {
            get { return Data["ProductCode"]; }
        }

        /// <summary>
        /// Gets the upgrade code.
        /// </summary>
        /// <value>
        /// The upgrade code.
        /// </value>
        public string UpgradeCode
        {
            get { return Data["UpgradeCode"]; }
        }

        //doesn't work if called from session
        //public Version GetCurrentlyInstalledVersion()
        //{
        //    Version result = null;
        //    try
        //    {
        //        result = AppSearch.GetProductVersionFromUpgradeCode(Data["UpgradeCode"]);
        //    }
        //    catch { }
        //    return result;
        //}

        /// <summary>
        /// Gets or sets the Data.
        /// <para>Data is a free form data storage for custom user defined and Wix# generated settings. In a way it's an
        /// alternative to the MSI session properties. The Data interface is identical to the Session properties -
        /// string dictionary. Though a very important difference is that Data (as opposite to session) does maintain
        /// the all entries during the whole MSI session. Even for the deferred action.</para>
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public AppData Data { get; set; }

        //public ResourcesData UIText { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupEventArgs"/> class.
        /// </summary>
        public SetupEventArgs()
        {
            Data = new AppData();
        }

        /// <summary>
        /// Saves the user data.
        /// </summary>
        internal void SaveData()
        {
            if (this.Session.IsActive())
                this.Session["WIXSHARP_RUNTIME_DATA"] = Data.ToString();
        }

        /// <summary>
        ///Class that encapsulated parsing of the CustomActionData content
        /// </summary>
        public class AppData : Dictionary<string, string>
        {
            /// <summary>
            /// Initializes from string.
            /// </summary>
            /// <param name="data">The data.</param>
            public AppData InitFrom(string data)
            {
                this.Clear();
                this.MergeReplace(data);
                return this;
            }

            //public AppData MergeReplace(string data)
            //{
            //    return (AppData) this.MergeReplace(data);
            //}

            /// <summary>
            /// Initializes from dictionary.
            /// </summary>
            /// <param name="data">The data.</param>
            public AppData InitFrom(Dictionary<string, string> data)
            {
                this.Clear();
                foreach (var item in data)
                    this.Add(item.Key, item.Value);
                return this;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return this.Serialize();
            }

            /// <summary>
            /// Gets or sets the value associated with the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns></returns>
            public new string this[string key]
            {
                get
                {
                    return base.ContainsKey(key) ? base[key] : null;
                }
                set
                {
                    base[key] = value;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                "\nInstallDir=" + InstallDir +
                "\nMsiFile=" + MsiFile +
                "\nUILevel=" + UILevel +
                "\nMode=" + Mode +
                "\nIsElevated=" + IsElevated +
                "\nIsInstalled=" + IsInstalled +
                "\n" +
                "\nIsInstalling=" + IsInstalling +
                "\nIsUninstalling=" + IsUninstalling +
                "\nIsReparing=" + IsRepairing +
                //"\nIsUpgrading=" + IsUpgrading + //not reliable
                "\nIsModifying=" + IsModifying +
                "\nModifyAction=" + ModifyAction +
                "\nProductCode=" + ProductCode +
                "\nUpgradeCode=" + UpgradeCode +
                "\nUpgradingProductCode=" + UpgradingProductCode +
                "\nIsManagedUISession=" + IsManagedUISession +
                "\nManagedUIHandle=" + ManagedUIHandle +
                "\n" +
                "\n" +
                "\np_Installed=" + Data["Installed"] +
                "\np_REINSTALL=" + Data["REINSTALL"] +
                "\np_UPGRADINGPRODUCTCODE=" + Data["UPGRADINGPRODUCTCODE"]
                ;
        }
    }
}