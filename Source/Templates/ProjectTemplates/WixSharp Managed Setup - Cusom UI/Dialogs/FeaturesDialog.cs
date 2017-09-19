using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The logical equivalent of the standard Features dialog. Though it implement slightly
    /// different user experience as it has checkboxes bound to the features instead of icons context menu
    /// as MSI dialog has.
    /// </summary>
    public partial class FeaturesDialog : ManagedForm, IManagedDialog
    {
        /*https://msdn.microsoft.com/en-us/library/aa367536(v=vs.85).aspx
         * ADDLOCAL - list of features to install
         * REMOVE - list of features to uninstall
         * ADDDEFAULT - list of features to set to their default state
         * REINSTALL - list of features to repair*/

        FeatureItem[] features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeaturesDialog"/> class.
        /// </summary>
        public FeaturesDialog()
        {
            InitializeComponent();
            label1.MakeTransparentOn(banner);
            label2.MakeTransparentOn(banner);
        }

        void FeaturesDialog_Load(object sender, System.EventArgs e)
        {
            //Debug.Assert(false);
            string drawTextOnlyProp = MsiRuntime.Session.Property("WixSharpUI_TreeNode_TexOnlyDrawing");

            bool drawTextOnly = true;

            if (drawTextOnlyProp.IsNotEmpty())
            {
                if (string.Compare(drawTextOnlyProp, "false", true) == 0)
                    drawTextOnly = false;
            }
            else
            {
                float dpi = this.CreateGraphics().DpiY;
                if (dpi == 96) // the checkbox custom drawing is only compatible with 96 DPI
                    drawTextOnly = false;
                else
                    drawTextOnly = true;
            }

            ReadOnlyTreeNode.Behavior.AttachTo(featuresTree, drawTextOnly);

            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            BuildFeaturesHierarchy();

            ResetLayout();
        }

        void ResetLayout()
        {
            // The form controls are properly anchored and will be correctly resized on parent form
            // resizing. However the initial sizing by WinForm runtime doesn't a do good job with DPI
            // other than 96. Thus manual resizing is the only reliable option apart from going WPF.

            float ratio = (float)banner.Image.Width / (float)banner.Image.Height;
            topPanel.Height = (int)(banner.Width / ratio);
            topBorder.Top = topPanel.Height + 1;

            var upShift = (int)(next.Height * 2.3) - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height += upShift;

            middlePanel.Top = topBorder.Bottom + 5;
            middlePanel.Height = (bottomPanel.Top - 5) - middlePanel.Top;

            featuresTree.Width = (int)((middlePanel.Width / 3.0) * 1.75);

            descriptionPanel.Left = featuresTree.Right + 10;
            descriptionPanel.Width = middlePanel.Width - descriptionPanel.Left - 10;
        }

        /// <summary>
        /// The collection of the features selected by user as the features to be installed.
        /// </summary>
        public static List<string> UserSelectedItems;

        void BuildFeaturesHierarchy()
        {
            //Cannot use MsiRuntime.Session.Features (FeatureInfo collection).
            //This WiX feature is just not implemented yet. All members except 'Name' throw InvalidHandeException
            //Thus instead of using FeatureInfo just collect the names and query database for the rest of the properties.
            string[] names = MsiRuntime.Session.Features.Select(x => x.Name).ToArray();

            features = names.Select(name => new FeatureItem(MsiRuntime.Session, name)).ToArray();

            //build the hierarchy tree
            var rootItems = features.Where(x => x.ParentName.IsEmpty())
                                    .OrderBy(x => x.RawDisplay)
                                    .ToArray();

            var itemsToProcess = new Queue<FeatureItem>(rootItems); //features to find the children for

            while (itemsToProcess.Any())
            {
                var item = itemsToProcess.Dequeue();

                //create the view of the feature
                var view = new ReadOnlyTreeNode
                {
                    Text = item.Title,
                    Tag = item, //link view to model
                    IsReadOnly = item.DisallowAbsent,
                    DefaultChecked = item.DefaultIsToBeInstalled(),
                    Checked = item.DefaultIsToBeInstalled()
                };

                item.View = view;

                if (item.Parent != null && item.Display != FeatureDisplay.hidden)
                    (item.Parent.View as TreeNode).Nodes.Add(view); //link child view to parent view

                // even if the item is hidden process all its children so the correct hierarchy is established

                // find all children
                features.Where(x => x.ParentName == item.Name)
                        .ForEach(c =>
                                 {
                                     c.Parent = item; //link child model to parent model
                                     itemsToProcess.Enqueue(c); //schedule for further processing
                                 });

                if (UserSelectedItems != null)
                    view.Checked = UserSelectedItems.Contains((view.Tag as FeatureItem).Name);

                if (item.Display == FeatureDisplay.expand)
                    view.Expand();
            }

            //add views to the treeView control
            rootItems.Where(x => x.Display != FeatureDisplay.hidden)
                     .Select(x => x.View)
                     .Cast<TreeNode>()
                     .ForEach(node => featuresTree.Nodes.Add(node));

            isAutoCheckingActive = true;
        }

        void SaveUserSelection()
        {
            UserSelectedItems = features.Where(x => x.IsViewChecked())
                                        .Select(x => x.Name)
                                        .ToList();
        }

        void back_Click(object sender, System.EventArgs e)
        {
            SaveUserSelection();
            Shell.GoPrev();
        }

        void next_Click(object sender, System.EventArgs e)
        {
            string itemsToInstall = features.Where(x => x.IsViewChecked())
                                        .Select(x => x.Name)
                                        .Join(",");

            string itemsToRemove = features.Where(x => !x.IsViewChecked())
                                           .Select(x => x.Name)
                                           .Join(",");

            if (itemsToRemove.Any())
                MsiRuntime.Session["REMOVE"] = itemsToRemove;

            if (itemsToInstall.Any())
                MsiRuntime.Session["ADDLOCAL"] = itemsToInstall;

            SaveUserSelection();
            Shell.GoNext();
        }

        void cancel_Click(object sender, System.EventArgs e)
        {
            Shell.Cancel();
        }

        void featuresTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            description.Text = e.Node.FeatureItem().Description;
        }

        bool isAutoCheckingActive = false;

        void featuresTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (isAutoCheckingActive)
            {
                isAutoCheckingActive = false;
                bool newState = e.Node.Checked;
                var queue = new Queue<TreeNode>();
                queue.EnqueueRange(e.Node.Nodes.ToArray());

                while (queue.Any())
                {
                    var node = queue.Dequeue();
                    node.Checked = newState;
                    queue.EnqueueRange(node.Nodes.ToArray());
                }

                if (e.Node.Checked)
                {
                    var parent = e.Node.Parent;
                    while (parent != null)
                    {
                        parent.Checked = true;
                        parent = parent.Parent;
                    }
                }

                isAutoCheckingActive = true;
            }
        }

        void reset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            isAutoCheckingActive = false;
            features.ForEach(f => f.ResetViewChecked());
            isAutoCheckingActive = true;
        }
    }
}