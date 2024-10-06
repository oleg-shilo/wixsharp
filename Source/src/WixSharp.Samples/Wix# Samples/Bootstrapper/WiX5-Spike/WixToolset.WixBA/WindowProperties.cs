// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Dependency Properties associated with the main Window object.
    /// </summary>
    public class WindowProperties : DependencyObject
    {
        /// <summary>
        /// Dependency Property to hold the result of detecting the relative luminosity (or brightness) of a Windows background.
        /// </summary>
        public static readonly DependencyProperty IsLightBackgroundProperty = DependencyProperty.Register(
            "IsLightBackground", typeof(bool), typeof(WindowProperties), new PropertyMetadata( false ));

        private static Lazy<WindowProperties> _instance = new Lazy<WindowProperties>(() =>
        {
            WindowProperties wp = new WindowProperties();
            wp.CheckBackgroundBrightness();
            return wp;
        });

        public static WindowProperties Instance
        {
            get
            {
                return _instance.Value;
            }
        }


        public bool IsLightBackground
        {
            get { return (bool)GetValue(IsLightBackgroundProperty); }
            private set { SetValue(IsLightBackgroundProperty, value); }
        }

        /// <summary>
        /// Use the Luminosity parameter of the background color to detect light vs dark theme settings.
        /// </summary>
        /// <remarks>
        /// This approach detects both the common High Contrast themes (White vs Black) and custom themes which may have relatively lighter backgrounds.
        /// </remarks>
        public void CheckBackgroundBrightness()
        {
            SolidColorBrush windowbrush = System.Windows.SystemColors.WindowBrush;
            System.Drawing.Color dcolor = System.Drawing.Color.FromArgb(windowbrush.Color.A, windowbrush.Color.R, windowbrush.Color.G, windowbrush.Color.B);

            var brightness = dcolor.GetBrightness();
            // Test for 'Lightness' at an arbitrary point, approaching 1.0 (White).           
            if (0.7 < brightness)
            {
                this.IsLightBackground = true;
            }
            else
            {
                this.IsLightBackground = false;
            }
        }
    }
}
