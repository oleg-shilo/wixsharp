// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using WixToolset.Mba.Core;

    /// <summary>
    /// The model.
    /// </summary>
    public class Model
    {
        private const string BurnBundleInstallDirectoryVariable = "InstallFolder";
        private const string BurnBundleLayoutDirectoryVariable = "WixBundleLayoutDirectory";
        private const string BurnBundleVersionVariable = "WixBundleVersion";

        /// <summary>
        /// Creates a new model for the BA.
        /// </summary>
        /// <param name="bootstrapper">The BA.</param>
        public Model(WixBA bootstrapper)
        {
            this.BAManifest = bootstrapper.BAManifest;
            this.Bootstrapper = bootstrapper;
            this.Command = bootstrapper.Command;
            this.Engine = bootstrapper.Engine;
            this.Telemetry = new List<KeyValuePair<string, string>>();
            this.Version = this.Engine.GetVariableVersion(BurnBundleVersionVariable);
        }

        public IBootstrapperApplicationData BAManifest { get; }

        /// <summary>
        /// Gets the bootstrapper.
        /// </summary>
        public IDefaultBootstrapperApplication Bootstrapper { get; }

        /// <summary>
        /// Gets the bootstrapper command-line.
        /// </summary>
        public IBootstrapperCommand Command { get; }

        /// <summary>
        /// Gets the bootstrapper engine.
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// Gets the key/value pairs used in telemetry.
        /// </summary>
        public List<KeyValuePair<string, string>> Telemetry { get; private set; }

        /// <summary>
        /// Get or set the final result of the installation.
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Get the version of the install.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Get or set the path where the bundle is installed.
        /// </summary>
        public string InstallDirectory
        {
            get
            {
                if (!this.Engine.ContainsVariable(BurnBundleInstallDirectoryVariable))
                {
                    return null;
                }

                return this.Engine.GetVariableString(BurnBundleInstallDirectoryVariable);
            }

            set
            {
                this.Engine.SetVariableString(BurnBundleInstallDirectoryVariable, value, false);
            }
        }

        /// <summary>
        /// Get or set the path for the layout to be created.
        /// </summary>
        public string LayoutDirectory
        {
            get
            {
                if (!this.Engine.ContainsVariable(BurnBundleLayoutDirectoryVariable))
                {
                    return null;
                }

                return this.Engine.GetVariableString(BurnBundleLayoutDirectoryVariable);
            }

            set
            {
                this.Engine.SetVariableString(BurnBundleLayoutDirectoryVariable, value, false);
            }
        }

        public LaunchAction PlannedAction { get; set; }

        /// <summary>
        /// Creates a correctly configured HTTP web request.
        /// </summary>
        /// <param name="uri">URI to connect to.</param>
        /// <returns>Correctly configured HTTP web request.</returns>
        public HttpWebRequest CreateWebRequest(string uri)
        {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            request.UserAgent = String.Concat("WixInstall", this.Version.ToString());

            return request;
        }

        /// <summary>
        /// Gets the display name for a package if possible.
        /// </summary>
        /// <param name="packageId">Identity of the package to find the display name.</param>
        /// <returns>Display name of the package if found or the package id if not.</returns>
        public string GetPackageName(string packageId)
        {
            return this.BAManifest.Bundle.Packages.TryGetValue(packageId, out var package) ? package.DisplayName : packageId;
        }
    }
}
