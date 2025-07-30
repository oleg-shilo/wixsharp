using System;
using System.Collections.Generic;
using System.Linq;
using WixToolset.Dtf.WindowsInstaller;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// Equivalent of Microsoft.Deployment.WindowsInstaller.FeatureInfo which is read-only and doesn't work anyway (at least in WiX v3.9)
    /// </summary>
    public class FeatureItem
    {
        /// <summary>
        /// The name of the Feature
        /// </summary>
        public string Name;

        /// <summary>
        /// The name of the parent Feature
        /// </summary>
        public string ParentName;

        /// <summary>
        /// The title of the Feature
        /// </summary>
        public string Title;

        /// <summary>
        /// The description of the Feature
        /// </summary>
        public string Description;

        /// <summary>
        /// The view of the Feature. Typically a TreeNode
        /// </summary>
        [Obsolete("This field has been renamed into ViewModel. Use the new field instead.", error: false)]
        public object View { set => ViewModel = value; get => ViewModel; }

        /// <summary>
        /// The View or ViewModel of the Feature. Typically a TreeNode (view in WinForms) or Node (ViewModel in WPF)
        /// </summary>
        public object ViewModel;

        /// <summary>
        /// The parent FeatureItem
        /// </summary>
        public FeatureItem Parent;

        /// <summary>
        /// The requested state. Defines the InstallState of the feature to be achieved as the result of the MSI execution.
        /// </summary>
        public InstallState RequestedState;

        /// <summary>
        /// The current state. Defines the InstallState of the feature before the MSI execution.
        /// </summary>
        public InstallState CurrentState;

        /// <summary>
        /// Defines how the feature should be displayed in the feature tree.
        /// </summary>
        public FeatureAttributes Attributes;

        /// <summary>
        /// Determines the initial display of this feature in the feature tree.
        /// </summary>
        public FeatureDisplay Display;

        /// <summary>
        /// Determines the order and initial display of this feature in the feature tree. It is a raw value of the
        /// `Display` attribute of the `Feature` WiX element.
        /// </summary>
        public int RawDisplay;

        /// <summary>
        /// Gets a value indicating whether the feature is allowed to be "absent".
        /// </summary>
        /// <value>
        ///   <c>true</c> if "disallow absent"; otherwise, <c>false</c>.
        /// </value>
        public bool DisallowAbsent
        {
            get { return FeatureAttributes.UIDisallowAbsent.PresentIn(Attributes); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureItem"/> class.
        /// </summary>
        public FeatureItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureItem"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        public FeatureItem(Session session, string name)
        {
            // Debug.Assert(false);

            //
            // Get default install level and feature condition (if any).
            //
            if (!int.TryParse(session["INSTALLLEVEL"], out var installLevel)) installLevel = 1; // MSI default

            Dictionary<string, object> conditionRow = null;

            if (session.Database.IsTablePersistent("Condition"))
            {
                var condition = session.OpenView("select * from Condition where Feature_ = '" + name + "'");
                conditionRow = condition.FirstOrDefault();
            }

            var data = session.OpenView("select * from Feature where Feature = '" + name + "'");
            Dictionary<string, object> row = data.FirstOrDefault();

            if (row != null)
            {
                Name = name;
                ParentName = (string)row["Feature_Parent"];
                Title = (string)row["Title"];
                Description = (string)row["Description"];

                RawDisplay = (int)row["Display"];
                Display = RawDisplay.MapToFeatureDisplay();

                //
                // Set defaultState according to feature and install levels, then evaluate
                // and adjust state according to feature condition.
                //
                var defaultState = (Convert.ToInt32(row["Level"]) <= installLevel) ? InstallState.Local : InstallState.Absent;
                if (session.IsInstalling()
                    && conditionRow?["Condition"] != null
                    && session.EvaluateCondition(conditionRow["Condition"].ToString())  // If condition is true...
                {
                    // ...set state according to condition level.
                    defaultState = (Convert.ToInt32(conditionRow["Level"]) <= installLevel)
                       ? InstallState.Local 
                       : InstallState.Absent;
                }

                CurrentState = DetectFeatureState(session, name);
                RequestedState = session.IsInstalled() ? CurrentState : defaultState;

                Attributes = (FeatureAttributes)row["Attributes"];
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
            return Title;
        }

        /// <summary>
        /// Detects the state of the feature.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        static InstallState DetectFeatureState(Session session, string name)
        {
            var productCode = session["ProductCode"];

            var installedPackage = new ProductInstallation(productCode);
            if (installedPackage.IsInstalled)
                return installedPackage.Features
                                       .First(x => x.FeatureName == name)
                                       .State;
            else
                return InstallState.Absent;
        }
    }
}