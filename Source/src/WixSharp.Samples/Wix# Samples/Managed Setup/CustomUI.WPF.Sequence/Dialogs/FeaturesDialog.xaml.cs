using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF
{
    public partial class FeaturesDialog : WpfDialog, IWpfDialog
    {
        public FeaturesDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new FeaturesDialogModel(ManagedFormHost), this, null);
        }
    }

    public class Node : PropertyChangedBase
    {
        private bool @checked;

        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; } // with list and observable collection same results
        public bool Checked { get => @checked; set { @checked = value; NotifyOfPropertyChange(() => Checked); } }
        public bool DefaultChecked { get; set; }
        public object Data { get; set; }
        public bool IsEditable { get; set; } = true;
    }

    public class FeaturesDialogModel : Caliburn.Micro.Screen
    {
        ManagedForm Host;
        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        string selectedNodeDescription;
        public ObservableCollection<Node> RootNodes { get; set; } = new ObservableCollection<Node>();

        public FeaturesDialogModel(ManagedForm host)
        {
            this.Host = host;
            BuildFeaturesHierarchy();
        }

        public BitmapImage Banner => session?.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        public string SelectedNodeDescription
        {
            get => selectedNodeDescription;
            set { selectedNodeDescription = value; NotifyOfPropertyChange(() => SelectedNodeDescription); }
        }

        /// <summary>
        /// The collection of the features selected by user as the features to be installed.
        /// </summary>
        public static List<string> UserSelectedItems { get; set; }

        /// <summary>
        /// The initial/default set of selected items (features) before user made any selection(s).
        /// </summary>
        static List<string> InitialUserSelectedItems { get; set; }

        public void GoPrev()
        {
            SaveUserSelection();
            shell?.GoPrev();
        }

        /*https://msdn.microsoft.com/en-us/library/aa367536(v=vs.85).aspx
        * ADDLOCAL - list of features to install
        * REMOVE - list of features to uninstall
        * ADDDEFAULT - list of features to set to their default state
        * REINSTALL - list of features to repair*/

        public void GoNext()
        {
            if (Host == null) return;

            bool userChangedFeatures = UserSelectedItems?.JoinBy(",") != InitialUserSelectedItems.JoinBy(",");

            if (userChangedFeatures)
            {
                string itemsToInstall = features.Where(x => (x.View as Node).Checked)
                                                .Select(x => x.Name)
                                                .JoinBy(",");

                string itemsToRemove = features.Where(x => !(x.View as Node).Checked)
                                               .Select(x => x.Name)
                                               .JoinBy(",");

                if (itemsToRemove.Any())
                    session["REMOVE"] = itemsToRemove;

                if (itemsToInstall.Any())
                    session["ADDLOCAL"] = itemsToInstall;
            }
            else
            {
                session["REMOVE"] = "";
                session["ADDLOCAL"] = "";
            }

            SaveUserSelection();
            shell.GoNext();
        }

        public void Cancel()
            => shell?.Cancel();

        public void Reset()
        {
            features.ForEach(x => (x.View as Node).Checked = x.DefaultIsToBeInstalled());
        }

        FeatureItem[] features;

        void BuildFeaturesHierarchy()
        {
            features = session.Features; // must make a copy of the features as they cannot be modified in the session

            // build the hierarchy tree
            var visibleRootItems = features.Where(x => x.ParentName.IsEmpty())
                                           .OrderBy(x => x.RawDisplay)
                                           .ToArray();

            var itemsToProcess = new Queue<FeatureItem>(visibleRootItems); // features to find the children for

            while (itemsToProcess.Any())
            {
                var item = itemsToProcess.Dequeue();

                // create the view of the feature
                var view = new Node
                {
                    Name = item.Title,
                    Data = item, // link view to model
                    IsEditable = !item.DisallowAbsent,
                    DefaultChecked = item.DefaultIsToBeInstalled(),
                    Checked = item.DefaultIsToBeInstalled()
                };

                item.View = view; // link model to view

                if (item.Parent != null && item.Display != FeatureDisplay.hidden)
                    (item.Parent.View as Node).Nodes.Add(view); //link child view to parent view

                // even if the item is hidden process all its children so the correct hierarchy is established

                // find all children
                features.Where(x => x.ParentName == item.Name)
                        .ForEach(c =>
                         {
                             c.Parent = item; //link child model to parent model
                             itemsToProcess.Enqueue(c); //schedule for further processing
                         });

                if (UserSelectedItems != null)
                    view.Checked = UserSelectedItems.Contains((view.Data as FeatureItem).Name);
            }

            // add views to the treeView control
            visibleRootItems
                .Where(x => x.Display != FeatureDisplay.hidden)
                .Select(x => (Node)x.View)
                .ForEach(node => RootNodes.Add(node));

            InitialUserSelectedItems = features.Where(x => (x.View as Node).Checked)
                                               .Select(x => x.Name)
                                               .OrderBy(x => x)
                                               .ToList();
        }

        void SaveUserSelection()
        {
            UserSelectedItems = features.Where(x => x.IsViewChecked())
                                        .Select(x => x.Name)
                                        .OrderBy(x => x)
                                        .ToList();
        }
    }
}