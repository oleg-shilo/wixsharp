// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

// Identifies the class that derives from IBootstrapperApplicationFactory and is the BAFactory class that gets
// instantiated by the interop layer
[assembly: WixToolset.Mba.Core.BootstrapperApplicationFactory(typeof(WixToolset.WixBA.WixBAFactory))]

namespace WixToolset.WixBA
{
    using System.Diagnostics;
    using System.Windows;
    using WixToolset.Mba.Core;

    public class WixBAFactory : BaseBootstrapperApplicationFactory
    {
        protected override IBootstrapperApplication Create(IEngine engine, IBootstrapperCommand command)
        {
            MessageBox.Show("Loading WixBAFactory");
            Debug.Assert(false);
            return new WixBA(engine, command);
        }
    }
}