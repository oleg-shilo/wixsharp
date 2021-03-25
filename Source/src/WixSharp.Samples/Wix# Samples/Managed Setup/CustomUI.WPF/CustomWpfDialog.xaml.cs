using System;
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

namespace MyProduct
{
    public partial class CustomDialogView : UserControl
    {
        public CustomDialogView()
        {
            InitializeComponent();
        }

        ManagedForm host;

        public CustomDialogView(ManagedForm host)
        {
            InitializeComponent();

            this.host = host;

            host.Text = host.Runtime.Localize("ProductName") + " Setup";
            banner.Source = host.Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();
            host.Runtime.Localize(this.Root);

            Gritting.Text = "Hello WPF World!";
        }

        void GoPrev_Click(object sender, RoutedEventArgs e)
        {
            host.Shell.GoPrev();
        }

        void GoNext_Click(object sender, RoutedEventArgs e)
        {
            host.Shell.GoNext();
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            host.Shell.Cancel();
        }
    }

    static class Extensions
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

    public partial class UserNameDialog : ManagedForm, IManagedDialog
    {
        public UserNameDialog()
        {
            this.Load += (s, _e) =>
            {
                var panel = new CustomDialogView(this);
                var host = new System.Windows.Forms.Integration.ElementHost();
                host.Dock = System.Windows.Forms.DockStyle.Fill;
                host.Child = panel;
                this.Controls.Add(host);
            };
        }
    }
}