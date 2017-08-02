using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public object View;

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

            var data = session.OpenView("select * from Feature where Feature = '" + name + "'");

            Dictionary<string, object> row = data.FirstOrDefault();

            if (row != null)
            {
                Name = name;
                ParentName = (string)row["Feature_Parent"];
                Title = (string)row["Title"];
                Description = (string)row["Description"];

                var rawDisplay = (int)row["Display"];
                Display = rawDisplay.MapToFeatureDisplay();

                var defaultState = (InstallState)row["Level"];

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

            var installedPackage = new Microsoft.Deployment.WindowsInstaller.ProductInstallation(productCode);
            if (installedPackage.IsInstalled)
                return installedPackage.Features
                .First(x => x.FeatureName == name)
                .State;
            else
                return InstallState.Absent;
        }
    }
}