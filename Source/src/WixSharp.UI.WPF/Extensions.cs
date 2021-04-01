using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharp.UI.WPF
{
    public static class Extensions
    {
        public static BitmapImage ToImageSource(this Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static InstallerRuntime Localize(this InstallerRuntime runtime, DependencyObject parent)
        {
            string translate(string text)
                => runtime.Localize(text.Trim('[', ']'))
                          .TrimStart('&'); // trim buttons text "&Next"
            bool isLocalizable(string text)
                => text.StartsWith("[") && text.EndsWith("]");

            parent
                .GetChildrenOfType<TextBlock>()
                .Where(x => isLocalizable(x.Text))
                .ForEach(x => x.Text = translate(x.Text));

            parent
                .GetChildrenOfType<Button>()
                .Where(x => isLocalizable(x.Content.ToString()))
                .ForEach(x => x.Content = translate(x.Content.ToString()));

            return runtime;
        }

        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in GetChildrenOfType<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }

    class WpfDialogMock : UI.WPF.WpfDialog { } // this private class is needed for testing

    public class WpfDialog : UserControl, IManagedDialog
    {
        string dialogTitle;

        public string DialogTitle
        {
            get
            {
                return dialogTitle;
            }

            set
            {
                dialogTitle = value;

                ManagedFormHost?.Localize();
            }
        }

        public IManagedDialog Host { get; set; }
        public ManagedForm ManagedFormHost { get => (ManagedForm)Host; }
        public IManagedUIShell Shell { get; set; }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        public void Localize(DependencyObject element = null)
        {
            var root = (element ?? this.Content as DependencyObject);

            // resolve and translate all elements with translatable content ("[<localization_key>]")
            if (root != null)
                this.ManagedFormHost.Runtime.Localize(root);
        }

        virtual public void OnExecuteComplete()
        {
        }

        /// <summary>
        /// Called when MSI execute started.
        /// </summary>
        virtual public void OnExecuteStarted()
        {
        }

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        virtual public void OnProgress(int progressPercentage)
        {
        }

        /// <summary>
        /// Processes information and progress messages sent to the user interface.
        /// <para> This method directly mapped to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI.ProcessMessage" />.</para>
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        virtual public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
            => MessageResult.OK;
    }

    public class WpfDialogHost : ManagedForm, IManagedDialog, IWpfDialogHost
    {
        IWpfDialog content;

        public void SetDialogContent(IWpfDialog content)
        {
            this.content = content;

            this.Load += (s, _e) =>
            {
                content.Host = this;
                var host = new System.Windows.Forms.Integration.ElementHost();
                host.Dock = System.Windows.Forms.DockStyle.Fill;
                host.Child = (UserControl)content;
                this.Controls.Add(host);
                content.Init();
                this.Localize();
            };
        }

        override protected void OnShellChanged()
        {
            if (content is IManagedDialog)
                (this.content as IManagedDialog).Shell = this.Shell;
        }
    }
}