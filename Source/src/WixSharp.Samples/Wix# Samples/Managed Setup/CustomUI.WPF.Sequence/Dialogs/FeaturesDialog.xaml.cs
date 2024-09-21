using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF.Sequence
{
    public partial class FeaturesDialog : WpfDialog, IWpfDialog
    {
        FeaturesDialogModel model;

        public FeaturesDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            model = new FeaturesDialogModel(ManagedFormHost);
            ViewModelBinder.Bind(model, this, null);
        }

        void Features_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var data = (FeatureItem)(e.NewValue as Node)?.Data;
            model.SelectedNodeDescription = data?.Description;
        }
    }

    public class Node : PropertyChangedBase
    {
        bool @checked;

        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>(); // with list and observable collection same results

        internal bool ignoreChldrenCheck = false;

        public bool IsPartialChecked
        {
            get
            {
                bool allChldrenInSameState =
                    Nodes.Count == 0 ||
                    Nodes.All(x => x.Checked) ||
                    Nodes.All(x => !x.Checked);

                return !allChldrenInSameState;
            }
        }

        public bool Checked
        {
            get => @checked; set
            {
                if (@checked != value)
                {
                    @checked = value;

                    if (ignoreChldrenCheck)
                    {
                        ignoreChldrenCheck = false;
                    }
                    else
                    {
                        Nodes.ForEach(x =>
                        {
                            x.ignoreChldrenCheck = true;
                            x.Checked = value;
                            x.ignoreChldrenCheck = false;
                        });

                        if (this.Parent != null)
                        {
                            Parent.ignoreChldrenCheck = true;
                            Parent.Checked = Parent.Nodes.Any(x => x.Checked);
                            Parent.ignoreChldrenCheck = false;
                        }
                    }
                }
                // allways update view in case if the Checked update was triggered by children even when @checked value is not changed
                NotifyOfPropertyChange(() => Checked);
                NotifyOfPropertyChange(() => IsPartialChecked);
            }
        }

        public bool DefaultChecked { get; set; }
        public object Data { get; set; }
        public Node Parent => ((FeatureItem)this.Data).Parent?.ViewModel as Node;
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

        public BitmapImage Banner => session?.GetResourceBitmap("WixSharpUI_Bmp_Banner").ToImageSource();

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
                string itemsToInstall = features.Where(x => (x.ViewModel as Node).Checked)
                                                .Select(x => x.Name)
                                                .JoinBy(",");

                string itemsToRemove = features.Where(x => !(x.ViewModel as Node).Checked)
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
            features.ForEach(x => (x.ViewModel as Node).Checked = x.DefaultIsToBeInstalled());
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
                var viewModel = new Node
                {
                    Name = item.Title,
                    Data = item, // link view to model
                    IsEditable = !item.DisallowAbsent,
                    DefaultChecked = item.DefaultIsToBeInstalled(),
                    Checked = item.DefaultIsToBeInstalled()
                };

                item.ViewModel = viewModel; // link model to view

                if (item.Parent != null && item.Display != FeatureDisplay.hidden)
                    (item.Parent.ViewModel as Node).Nodes.Add(viewModel); // link child view to parent view

                // even if the item is hidden process all its children so the correct hierarchy is established

                // find all children
                features.Where(x => x.ParentName == item.Name)
                        .ForEach(c =>
                         {
                             c.Parent = item; //link child model to parent model
                             itemsToProcess.Enqueue(c); //schedule for further processing
                         });

                if (UserSelectedItems != null)
                    viewModel.Checked = UserSelectedItems.Contains((viewModel.Data as FeatureItem).Name);
            }

            // add views to the treeView control
            visibleRootItems
                .Where(x => x.Display != FeatureDisplay.hidden)
                .Select(x => (Node)x.ViewModel)
                .ForEach(node => RootNodes.Add(node));

            InitialUserSelectedItems = features.Where(x => (x.ViewModel as Node).Checked)
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