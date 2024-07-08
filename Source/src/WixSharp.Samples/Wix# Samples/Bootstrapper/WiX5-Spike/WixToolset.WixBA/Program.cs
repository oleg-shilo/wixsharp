// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System.Windows;
    using WixToolset.BootstrapperApplicationApi;

    internal class Program
    {
        private static int Main()
        {
            MessageBox.Show("Hello 7777777!");
            var application = new WixBA();

            ManagedBootstrapperApplication.Run(application);

            return 0;
        }
    }
}