using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Simplifies authoring for major upgrades, including support for preventing downgrades.
    /// </summary>
    public class MajorUpgrade
    {
        /// <summary>
        /// When set to "false" (the default), products with lower version numbers are blocked from installing when a product with a higher version
        /// is installed; the DowngradeErrorMessage attribute must also be specified.
        /// <para>
        /// When set to "true", any version can be installed over any other version.
        /// </para>
        /// </summary>
        public bool? AllowDowngrades;

        /// <summary>
        /// When set to "false" (the default), installing a product with the same version and upgrade code (but different product code) is allowed and
        /// treated by MSI as two products. When set to yes, WiX sets the msidbUpgradeAttributesVersionMaxInclusive attribute, which tells MSI to
        /// treat a product with the same version as a major upgrade.
        /// <para>
        /// This is useful when two product versions differ only in the fourth version field. MSI specifically ignores that field when comparing
        /// product versions, so two products that differ only in the fourth version field are the same product and need this attribute set to
        /// "true" to be detected.
        /// </para>
        /// <para>
        /// Note that because MSI ignores the fourth product version field, setting this attribute to yes also allows downgrades when the first three
        /// product version fields are identical. For example, product version 1.0.0.1 will "upgrade" 1.0.0.2998 because they're seen as the same
        /// version (1.0.0). That could reintroduce serious bugs so the safest choice is to change the first three version fields and omit this attribute
        ///  to get the default of no.
        /// </para>
        /// <para>
        /// This attribute cannot be "true" when AllowDowngrades is also "true" -- AllowDowngrades already allows two products with the same version
        /// number to upgrade each other.
        /// </para>
        /// </summary>
        public bool? AllowSameVersionUpgrades;

        /// <summary>
        /// When set to "true", products with higher version numbers are blocked from installing when a product with a lower version is installed;
        /// the UpgradeErrorMessage attribute must also be specified.
        /// <para>
        /// When set to "false" (the default), any version can be installed over any lower version.
        /// </para>
        /// </summary>
        public bool? Disallow;

        /// <summary>
        /// The message displayed if users try to install a product with a higher version number when a product with a lower version is installed.
        /// Used only when Disallow is "true".
        /// </summary>
        public string DisallowUpgradeErrorMessage;

        /// <summary>
        /// The message displayed if users try to install a product with a lower version number when a product with a higher version is installed.
        /// Used only when AllowDowngrades is "false" (the default).
        /// </summary>
        public string DowngradeErrorMessage;

        /// <summary>
        /// When set to "true", failures removing the installed product during the upgrade will be ignored.
        /// When set to "false" (the default), failures removing the installed product during the upgrade will be considered a failure and, depending on the
        /// scheduling, roll back the upgrade.
        /// </summary>
        public bool? IgnoreRemoveFailure;

        /// <summary>
        /// When set to "true" (the default), the MigrateFeatureStates standard action will set the feature states of the upgrade product to those of
        /// the installed product.
        /// <para>
        /// When set to "false", the installed features have no effect on the upgrade installation
        /// </para>
        /// </summary>
        public bool? MigrateFeatures;

        /// <summary>
        /// A formatted string that contains the list of features to remove from the installed product. The default is to remove all features. Note that
        /// if you use formatted property values that evaluate to an empty string, no features will be removed; only omitting this attribute defaults to
        /// removing all features.
        /// </summary>
        public string RemoveFeatures;

        /// <summary>
        /// Determines the scheduling of the RemoveExistingProducts standard action, which is when the installed product is removed. The default is
        /// "afterInstallValidate" which removes the installed product entirely before installing the upgrade product. It's slowest but gives
        /// the most flexibility in changing components and features in the upgrade product.
        /// </summary>
        public UpgradeSchedule? Schedule;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public virtual XContainer[] ToXml()
        {
            var result = new XElement("MajorUpgrade");

            result.SetAttribute("AllowDowngrades", AllowDowngrades)
                  .SetAttribute("AllowSameVersionUpgrades", AllowSameVersionUpgrades)
                  .SetAttribute("Disallow", Disallow)
                  .SetAttribute("DisallowUpgradeErrorMessage", DisallowUpgradeErrorMessage)
                  .SetAttribute("DowngradeErrorMessage", DowngradeErrorMessage)
                  .SetAttribute("IgnoreRemoveFailure", IgnoreRemoveFailure)
                  .SetAttribute("MigrateFeatures", MigrateFeatures)
                  .SetAttribute("RemoveFeatures", RemoveFeatures)
                  .SetAttribute("Schedule", Schedule);

            return new XContainer[] { result };
        }
    }

    /// <summary>
    /// Determines the scheduling of the RemoveExistingProducts standard action.
    /// </summary>
    public enum UpgradeSchedule
    {
        /// <summary>
        ///  (Default) Schedules RemoveExistingProducts after the InstallValidate standard action. This scheduling removes the installed product entirely before installing the upgrade product. It's slowest but gives the most flexibility in changing components and features in the upgrade product. Note that if the installation of the upgrade product fails, the machine will have neither version installed.
        /// </summary>
        afterInstallValidate,

        /// <summary>
        /// Schedules RemoveExistingProducts after the InstallInitialize standard action. This is similar to the afterInstallValidate scheduling, but if the installation of the upgrade product fails, Windows Installer also rolls back the removal of the installed product -- in other words, reinstalls it.
        /// </summary>
        afterInstallInitialize,

        /// <summary>
        /// Schedules RemoveExistingProducts between the InstallExecute and InstallFinalize standard actions. This scheduling installs the upgrade product "on top of" the installed product then lets RemoveExistingProducts uninstall any components that don't also exist in the upgrade product. Note that this scheduling requires strict adherence to the component rules because it relies on component reference counts to be accurate during installation of the upgrade product and removal of the installed product. For more information, see Bob Arnson's blog post "Paying for Upgrades" for details. If installation of the upgrade product fails, Windows Installer also rolls back the removal of the installed product -- in other words, reinstalls it.
        /// </summary>
        afterInstallExecute,

        /// <summary>
        /// Schedules RemoveExistingProducts between the InstallExecuteAgain and InstallFinalize standard actions. This is identical to the afterInstallExecute scheduling but after the InstallExecuteAgain standard action instead of InstallExecute.
        /// </summary>
        afterInstallExecuteAgain,

        /// <summary>
        /// Schedules RemoveExistingProducts after the InstallFinalize standard action. This is similar to the afterInstallExecute and afterInstallExecuteAgain schedulings but takes place outside the installation transaction so if installation of the upgrade product fails, Windows Installer does not roll back the removal of the installed product, so the machine will have both versions installed.
        /// </summary>
        afterInstallFinalize
    }
}