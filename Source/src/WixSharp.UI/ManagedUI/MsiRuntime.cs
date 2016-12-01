using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using io = System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using sys = System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace WixSharp
{
    /*http://www.codeproject.com/Articles/132918/Creating-Custom-Action-for-WIX-Written-in-Managed
     * Expected to be like this:
     *        Property Name                 Install     Uninstall   Repair      Modify      Upgrade
     *        --------------------------------------------------------------------------------------
     *        Installed                     False       True        False       True        True    
     *        REINSTALL                     True        False       True        False       False     
     *        UPGRADINGPRODUCTCODE          True        False       True        True        True      
     *        
     * Though in the reality it is like this:
     *        Property Name                 Install     Uninstall   Repair      Modify      Upgrade
     *        ---------------------------------------------------------------------------------------------------
     *        Installed                     <null>      00:00:00    00:00:00    00:00:00    <not tested yet>    
     *        REINSTALL                     <null>      <null>      All         <null>      <not tested yet>       
     *        UPGRADINGPRODUCTCODE          <null>      <null>      <null>      <null>      <not tested yet>
     *      
     * */



    /// <summary>
    /// Represents MSI runtime context. This class is to be used by ManagedUI dialogs to interact with the MSI session. 
    /// </summary>
    public class MsiRuntime
    {
        /// <summary>
        /// Starts the execution of the MSI installation.
        /// </summary>
        public System.Action StartExecute;

        /// <summary>
        /// Cancels the execution of the MSI installation, which is already started (progress is displayed).
        /// </summary>
        public System.Action CancelExecute;
        
        /// <summary>
        /// The session object.
        /// </summary>
        public Session Session;

        //DOESN'T work reliably. For example if no InstallDir dialog is displayed the MSI session does not have "INSTALLDIR" property initialized.
        //The other properties (e.g. UI Level) are even never available at all.
        //It looks like Session is initialized/updated correctly for the 'custom actions' session but not for the 'Embedded UI' session.  
        //In fact because of these problems a session object can no longer be considered as a single connection point between all MSI runtime modules. 
        internal void CaptureSessionData()
        {
            try
            {
                if (Session.IsActive())
                {
                    Data["INSTALLDIR"] = Session["INSTALLDIR"];
                    Data["Installed"] = Session["Installed"];
                    Data["REMOVE"] = Session["REMOVE"];
                    Data["REINSTALL"] = Session["REINSTALL"];
                    Data["UPGRADINGPRODUCTCODE"] = Session["UPGRADINGPRODUCTCODE"];
                    Data["UILevel"] = Session["UILevel"];
                    Data["WIXSHARP_MANAGED_UI_HANDLE"] = Session["WIXSHARP_MANAGED_UI_HANDLE"];
                }
            }
            catch { }
        }
        /// <summary>
        /// Repository of the session properties to be captured and transfered to the deferred CAs.for 
        /// </summary>
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        /// <summary>
        /// Localization map. 
        /// </summary>
        public ResourcesData UIText = new ResourcesData();

        internal void FetchInstallDir()
        {
            string installDirProperty = this.Session.Property("WixSharp_UI_INSTALLDIR");
            string dir = this.Session.Property(installDirProperty);
            if (dir.IsNotEmpty())
                InstallDir = dir; //user entered INSTALLDIR
            else
                InstallDir = this.Session.GetDirectoryPath(installDirProperty); //default INSTALLDIR
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsiRuntime"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public MsiRuntime(Session session)
        {
            this.Session = session;
            try
            {
                var bytes = Session.TryReadBinary("WixSharp_UIText");
                UIText.InitFromWxl(bytes);

                ProductName = Session.Property("ProductName");
                ProductCode = Session.Property("ProductCode");
                ProductVersion = session.Property("ProductVersion");

                FetchInstallDir();

                //it is important to preserve some product properties for localization as at the end of setup the session object will no longer be available
                UIText["ProductName"] = ProductName;
                UIText["ProductCode"] = ProductCode;
                UIText["ProductVersion"] = ProductVersion;

                //ensure Wix# strings are added if not already present
                if (!UIText.ContainsKey("ViewLog"))
                    UIText["ViewLog"] = "View Log";
            }
            catch { }
        }

        /// <summary>
        /// Gets the bitmap from the MSI embedded resources ('Binary' table).
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Bitmap GetMsiBitmap(string name)
        {
            try
            {
                byte[] data = Session.ReadBinary(name);
                using (Stream s = new MemoryStream(data))
                    return (Bitmap) Bitmap.FromStream(s);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Localizes the specified text.
        /// <para>The localization is performed according two possible scenarios. The method will return the match form the MSI embedded localization file. 
        /// However if it cannot find the match the method will try to find the and return the match in the MSI session properties.</para>
        /// <para>This method is mainly used by 'LocalizeWith' extension for a single step localization of WinForm controls.</para>
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public string Localize(string text)
        {
            if (UIText.ContainsKey(text))
                return UIText[text];

            try
            {
                string result = Session.Property(text);
                if (result.IsEmpty())
                    return text;
                else
                    return result;
            }
            catch { }

            return text;
        }

        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName;
        /// <summary>
        /// The directory the product is to be installed. This field will contain a valid path only after the MSI execution started.
        /// </summary>
        public string InstallDir { get; internal set; }
        /// <summary>
        /// The product code
        /// </summary>
        public string ProductCode;
        /// <summary>
        /// The product version
        /// </summary>
        public string ProductVersion;
    }

    /// <summary>
    /// Localization map. It is nothing else but a specialized version of a generic string-to-string Dictionary.
    /// </summary>
    public class ResourcesData : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes from WiX localization data (*.wxl).
        /// </summary>
        /// <param name="wxlData">The WXL file bytes.</param>
        /// <returns></returns>
        public void InitFromWxl(byte[] wxlData)
        {
            this.Clear();
            if (wxlData != null && wxlData.Any())
            {
                XDocument doc;

                var tempXmlFile = System.IO.Path.GetTempFileName();
                try
                {
                    System.IO.File.WriteAllBytes(tempXmlFile, wxlData);
                    doc = XDocument.Load(tempXmlFile); //important to use Load as it will take care about encoding magic first bites of wxlData, that came from the localization file  
                }
                catch
                {
                    throw new Exception("The localization XML data is in invalid format.");
                }
                finally
                {
                    System.IO.File.Delete(tempXmlFile);
                }

                var data = doc.Descendants()
                              .Where(x => x.Name.LocalName == "String")
                              .ToDictionary(x => x.Attribute("Id").Value, x => x.Value);

                foreach (var item in data)
                    this.Add(item.Key, item.Value);
            }
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

}