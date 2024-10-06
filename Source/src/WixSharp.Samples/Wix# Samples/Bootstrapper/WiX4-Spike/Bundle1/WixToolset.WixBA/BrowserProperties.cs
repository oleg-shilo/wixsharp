// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Dependency Properties to support using a WebBrowser object.
    /// </summary>
    class BrowserProperties
    {
        /// <summary>
        /// Dependency Propery used to pass an HTML string to the webBrowser object.
        /// </summary>
        public static readonly DependencyProperty HtmlDocProperty =
            DependencyProperty.RegisterAttached("HtmlDoc", typeof(string), typeof(BrowserProperties), new PropertyMetadata(OnHtmlDocChanged));

        public static string GetHtmlDoc(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlDocProperty);
        }

        public static void SetHtmlDoc(DependencyObject dependencyObject, string htmldoc)
        {
            dependencyObject.SetValue(HtmlDocProperty, htmldoc);
        }

        /// <summary>
        /// Event handler that passes the HtmlDoc Dependency Property to MavigateToString method.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnHtmlDocChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = (WebBrowser)d;
            webBrowser.NavigateToString((string)e.NewValue);
        }
    }
}
