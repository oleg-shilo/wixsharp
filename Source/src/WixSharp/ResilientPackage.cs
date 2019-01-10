using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.CommonTasks;
using WixSharp.Utilities;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Allows to repair the MSI package even when the original installation package is no longer available.
    /// <para>Adds an additional source of the resiliency by attempting to create a symbolic link to the locally cached MSI package (%WINDIR%\Installer)
    /// or a hard link/full copy to the original installation MSI package in the specified directory.</para>
    /// <para><c>WIXSHARP_RESILIENT_SOURCE_DIR</c> property can be used to configure the target directory. <c>INSTALLDIR property is used by default.</c></para>
    /// <para>Windows 7 is shipped with the Windows Installer version 5.0, which unlike the previous versions of the windows installer caches the entire MSI,
    /// including internal CAB files. Unfortunately the complete cached MSI package is not used for repairs, a call is made to the original source
    /// which might not be available.
    ///</para>
    /// <para>See also:
    /// <list type="bullet">
    /// <item>https://docs.microsoft.com/en-us/windows/desktop/msi/source-resiliency</item>
    /// <item>https://www.symantec.com/connect/articles/reducing-windows-installer-disk-wastage-windows-7</item>
    /// </list>
    /// </para>
    /// </summary>
    public static class ResilientPackage
    {
        const string WIXSHARP_PACKAGENAME = "WIXSHARP_PACKAGENAME";
        const string WIXSHARP_RESILIENT_SOURCE_DIR = "WIXSHARP_RESILIENT_SOURCE_DIR";

        /// <summary>
        /// Enables source resiliency for the installer.
        /// Creates a symbolic link/hard link or makes a copy of the original MSI package in the specified location and points SOURCELIST to it.
        /// </summary>
        /// <param name="project">The project.</param>
        public static void EnableResilientPackage(this Project project)
        {
            project.EnableResilientPackage("{$ResilientPackageIstallDir}");

            project.WixSourceFormated += (ref string content) =>
            {
                content = content.Replace("{$ResilientPackageIstallDir}", project.ActualInstallDirId);
            };
        }

        /// <summary>
        /// Enables source resiliency for the installer.
        /// Creates a symbolic link/hard link or makes a copy of the original MSI package in the specified location and points SOURCELIST to it.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="resilientSourceDir">Resilient source directory.</param>
        public static void EnableResilientPackage(this Project project, string resilientSourceDir)
        {
            project.AddActions(
                new SetPropertyAction(new Id($"WixSharp_SetProperty_{WIXSHARP_RESILIENT_SOURCE_DIR}"),
                    WIXSHARP_RESILIENT_SOURCE_DIR, $"[{resilientSourceDir}]",
                    Return.check,
                    When.Before, Step.InstallInitialize,
                    $"{WIXSHARP_RESILIENT_SOURCE_DIR}=\"\""),

                new SetPropertyAction(new Id("WixSharp_SetProperty_SOURCELIST"),
                    "SOURCELIST", $"[{WIXSHARP_RESILIENT_SOURCE_DIR}]",
                    Return.check,
                    When.Before, Step.PublishProduct,
                    Condition.NOT_Installed)
            );

            var assembly = typeof(ResilientPackage).Assembly.Location;

            project.AddActions(
                new ManagedAction(new Id(nameof(WixSharp_SetPackageName_Action)),
                    WixSharp_SetPackageName_Action, assembly,
                    Return.ignore,
                    When.Before, Step.InstallInitialize,
                    Condition.BeingUninstalled),

                new ElevatedManagedAction(new Id(nameof(WixSharp_RemoveResilientPackage_Action)),
                    WixSharp_RemoveResilientPackage_Action, assembly,
                    Return.ignore,
                    When.Before, Step.RemoveFiles,
                    Condition.BeingUninstalled)
                {
                    UsesProperties = $"{WIXSHARP_RESILIENT_SOURCE_DIR},{WIXSHARP_PACKAGENAME}"
                },

                new ElevatedManagedAction(new Id(nameof(WixSharp_CreateResilientPackage_Action)),
                    WixSharp_CreateResilientPackage_Action, assembly,
                    Return.ignore,
                    When.Before, Step.InstallFinalize,
                    Condition.NOT_Installed | "REINSTALL<>\"\"")
                {
                    UsesProperties = $"UserSID,OriginalDatabase,ALLUSERS,{WIXSHARP_RESILIENT_SOURCE_DIR}"
                }
            );
        }

        /// <summary>
        /// Internal ResilientPackage action. It must be public for the DTF accessibility but it is not to be used by the user/developer.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult WixSharp_SetPackageName_Action(Session session)
        {
            return session.HandleErrors(() =>
            {
                var productCode = session.Property("ProductCode");

                string packageName = null;
                try
                {
                    packageName = GetPackageName(productCode);
                }
                catch (Exception e)
                {
                    session.Log(e.ToString());
                }

                if (packageName.IsEmpty())
                {
                    packageName = GetPackageNameFromRegistry(productCode);
                }

                session[WIXSHARP_PACKAGENAME] = packageName;
            });
        }

        /// <summary>
        /// Internal ResilientPackage action. It must be public for the DTF accessibility but it is not to be used by the user/developer.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult WixSharp_RemoveResilientPackage_Action(Session session)
        {
            return session.HandleErrors(() =>
            {
                var packageName = session.Property(WIXSHARP_PACKAGENAME);
                var resilientLocation = session.Property(WIXSHARP_RESILIENT_SOURCE_DIR);
                var resilientPackage = IO.Path.Combine(resilientLocation, packageName);

                IO.File.Delete(resilientPackage);
            });
        }

        /// <summary>
        /// Internal ResilientPackage action. It must be public for the DTF accessibility but it is not to be used by the user/developer.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult WixSharp_CreateResilientPackage_Action(Session session)
        {
            return session.HandleErrors(() => CreateResilientPackage(session));
        }

        [Flags]
        internal enum SymbolicLinkFlag
        {
            File = 0,
            Directory = 1,
            AllowUnprivilegedCreate = 2
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLinkFlag dwFlags);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        static void CreateResilientPackage(Session session)
        {
            var productCode = session.Property("ProductCode");

            var userSID = session.Property("ALLUSERS") == "1" ? "S-1-5-18" : session.Property("UserSID");
            var localPackage = GetLocalPackageFromRegistry(productCode, userSID);

            session.Log($"LocalPackage:'{localPackage}'");

            var resilientLocation = session.Property(WIXSHARP_RESILIENT_SOURCE_DIR);
            var originalPackage = session.Property("OriginalDatabase");
            var packageName = IO.Path.GetFileName(originalPackage);
            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentNullException($"PackageName is null.");
            }
            var resilientPackage = IO.Path.Combine(resilientLocation, packageName);

            var resilientPackageInfo = new IO.FileInfo(resilientPackage);
            if (resilientPackageInfo.Exists && resilientPackage.Equals(originalPackage, StringComparison.OrdinalIgnoreCase) && !IsSymbolicLink(resilientPackageInfo))
            {
                return;
            }

            IO.File.Delete(resilientPackage);

            // NOTES: * CreateSymbolicLink() fails under Windows 7 in the elevated context (works with Windows 8 and above),
            //          so the execution falls back to the CreateHardLink().
            //
            //        * Non-elevated installers don't have access to the %WINDIR%\Installer, so the execution falls back to the file copying.
            //
            //        * One should be careful with trying to created a hard link to the "originalPackage", because when MSI is installed through
            //          the NSIS bootstrapper, the bootstrapper is extracting MSI in a temporary folder with very restrictive access rights.
            //          A hard link to the MSI has the same restrictive access rights preventing it from doing repairs through ARP applet.
            //
            //        * Hard links should not be created to the "localPackage" (e.g. %WINDIR%\Installer\xxxxxxx.msi), because during the uninstall
            //          the local package file and therefore the hard-linked file are both locked by MSI installer and cannot be removed.

            // Create a symbolic link
            var result = CreateSymbolicLink(resilientPackage, localPackage, SymbolicLinkFlag.File);
            if (!result)
            {
                var errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                session.Log($"Failed to create a symbolic link. Link:'{resilientPackage}' Target:'{localPackage}' Error:{errorMessage}");
            }

            // Copy the file
            if (!result)
            {
                IO.File.Copy(originalPackage, resilientPackage, true);
            }
        }

        static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        static string CompressGUID(string guid)
        {
            var builder = new StringBuilder(5);
            var parts = guid.Trim('{', '}').Split('-');

            for (var ix = 0; ix < 3; ++ix)
            {
                builder.Append(Reverse(parts[ix]));
            }

            var tail = parts[3] + parts[4];
            for (var ix = 0; ix < tail.Length; ix += 2)
            {
                builder.Append(tail[ix + 1]);
                builder.Append(tail[ix]);
            }
            return builder.ToString();
        }

        static string GetLocalPackageFromRegistry(string productCode, string userSID)
        {
            var compressedGuid = CompressGUID(productCode);
            var keyName = $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\{userSID}\Products\{compressedGuid}\InstallProperties";

            return RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, keyName, "LocalPackage");

            // NOTE: Remove RegistryWOW6432 and replace with the following code once move to .Net Framework 4+
            //
            // string localPackage;
            // using (var localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            // {
            //     using (var installProperties = localKey.OpenSubKey(keyName, false))
            //     {
            //         localPackage = (string)installProperties?.GetValue("LocalPackage");
            //     }
            // }
            //
            // return localPackage;
        }

        static string GetPackageName(string productCode)
        {
            var product = new ProductInstallation(productCode);
            return product.AdvertisedPackageName;
        }

        static string GetPackageNameFromRegistry(string productCode)
        {
            var compressedGuid = CompressGUID(productCode);
            var keyName = $@"SOFTWARE\Classes\Installer\Products\{compressedGuid}\SourceList";

            return RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, keyName, "PackageName");

            // NOTE: Remove RegistryWOW6432 and replace with the following code once move to .Net Framework 4+
            //
            // string packageName;
            //
            // using (var localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            // {
            //     using (var sourceList = localKey.OpenSubKey(keyName, false))
            //     {
            //         packageName = (string)sourceList?.GetValue("PackageName");
            //     }
            // }
            // return packageName;
        }

        // ReSharper disable once UnusedMember.Local
        static bool IsSymbolicLink(string path)
        {
            var fileInfo = new IO.FileInfo(path);
            return IsSymbolicLink(fileInfo);
        }

        static bool IsSymbolicLink(IO.FileInfo fileInfo)
        {
            return (fileInfo.Attributes & IO.FileAttributes.ReparsePoint) != 0;
        }
    }
}