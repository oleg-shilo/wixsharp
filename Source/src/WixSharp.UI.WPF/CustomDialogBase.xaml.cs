using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

namespace WixSharp.UI.WPF
{
    /// <summary>
    /// This class is not to be used directly. This class is used by WixSharp runtime as a
    /// container for a user defined content (a WPF UserControl) of the custom dialog.
    /// <para>This class is a convenient way of defining custom UI dialogs without defining
    /// the full UI layout of the dialog but only a small area implementing the business logic
    /// of the setup step/phase
    /// </para>
    /// </summary>
    /// <example>The following is an example of adding a UserControl ('CustomDialogPanel') as a
    /// content of the custom dialog. Note, your control must implement <see cref="IWpfDialogContent"/>
    /// interface so WixSharp recognizes it as an embeddable custom dialog content.<code>
    /// project.ManagedUI.InstallDialogs.Add&gt;WelcomeDialog&gt;()
    ///                                 .Add&gt;FeaturesDialog&gt;()
    ///                                 .Add&gt;CustomDialogWith&lt;CustomDialogPanel&gt;&gt;()
    /// . . .
    /// public partial class CustomDialogPanel : UserControl, IWpfDialogContent
    /// {
    ///     public CustomDialogPanel()
    ///     {
    ///         InitializeComponent();
    ///     }
    ///
    ///     public void Init(CustomDialogBase parent)
    ///     {
    ///         ISession session = parent?.ManagedFormHost.Runtime.Session;
    ///         . . .
    ///
    /// </code>
    /// </example>
    /// <seealso cref="WixSharp.UI.WPF.WpfDialog" />
    /// <seealso cref="WixSharp.IWpfDialog" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class CustomDialogBase : WpfDialog, IWpfDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDialogBase"/> class.
        /// </summary>
        public CustomDialogBase()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the panel (<see cref="StackPanel"/>) with the navigation buttons (Back, Next and Cancel).
        /// </summary>
        /// <value>
        /// The buttons panel.
        /// </value>
        public StackPanel ButtonsPanel => NavigationPanel;

        /// <summary>
        /// Gets the default navigation buttons (Back, Next and Cancel).
        /// </summary>
        /// <value>
        /// The navigation buttons.
        /// </value>
        public Button[] Buttons => NavigationPanel.Children.OfType<Button>().ToArray();

        /// <summary>
        /// Gets the standard 'Next' navigation button.
        /// </summary>
        /// <value>
        /// The 'Next' button.
        /// </value>
        public Button GoNextButton => Buttons.FirstOrDefault(b => b.Name == "GoNext");

        /// <summary>
        /// Gets the standard 'Back' navigation button.
        /// </summary>
        /// <value>
        /// The 'Beck' button.
        /// </value>
        public Button GoPrevButton => Buttons.FirstOrDefault(b => b.Name == "GoPrev");

        /// <summary>
        /// Gets the standard 'Cancel' navigation button.
        /// </summary>
        /// <value>
        /// The 'Cancel' button.
        /// </value>
        public Button CancelButton => Buttons.FirstOrDefault(b => b.Name == "Cancel");

        /// <summary>
        /// This method is invoked by WixSHarp runtime when the custom dialog content is internally fully initialized.
        /// This is a convenient place to do further initialization activities (e.g. localization).
        /// </summary>
        public void Init()
        {
            this.DataContext = model = new CustomDialogBaseModel { Host = ManagedFormHost };
        }

        CustomDialogBaseModel model;

        /// <summary>
        /// Sets the content of the user defined custom dialog panel. This method is to be only invoked by WixSHarp runtime.
        /// </summary>
        /// <param name="userContent">Content of the user.</param>
        public void SetUserContent(object userContent)
        {
            DialogContent.Content = userContent;

            if (userContent is IWpfDialogContent standardUserPanel)
            {
                standardUserPanel.Init(this);
            }
        }

        void GoPrev_Click(object sender, System.Windows.RoutedEventArgs e)
            => model.GoPrev();

        void GoNext_Click(object sender, System.Windows.RoutedEventArgs e)
            => model.GoNext();

        void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
            => model.Cancel();
    }

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the of property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void NotifyOfPropertyChange(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    internal class CustomDialogBaseModel : NotifyPropertyChangedBase
    {
        public ManagedForm Host { get; set; }

        public BitmapImage Banner => Host?.Runtime.Session.GetResourceBitmap("WixSharpUI_Bmp_Banner").ToImageSource();

        bool canProceed;

        public bool CanProceedIsChecked
        {
            get { return canProceed; }
            set
            {
                canProceed = value;
                NotifyOfPropertyChange(nameof(CanProceedIsChecked));
                NotifyOfPropertyChange(nameof(CanGoNext));
            }
        }

        public string User { get; set; } = Environment.UserName;

        public bool CanGoNext
            => CanProceedIsChecked;

        public void GoPrev()
            => Host?.Shell.GoPrev();

        public void GoNext()
            => Host?.Shell.GoNext();

        public void Cancel()
            => Host?.Shell.Cancel();
    }
}