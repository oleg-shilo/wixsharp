#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

using IO = System.IO;
using Reflection = System.Reflection;

// I am checking it for null anyway but when compiling AOT the output becomes too noisy
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file

namespace WixSharp
{
    class ProjectValidator
    {
        static bool IsValidVersion(string versionText)
        {
            if (string.IsNullOrEmpty(versionText))
                return true;

            if (versionText == "%this%")
                return true;

            try
            {
                new Version(versionText);
                return true;
            }
            catch
            { return false; }
        }

        public static void Validate(Project project)
        {
            if (project.Media.Any() && project.GenericItems.Any(item => item is MediaTemplate))
            {
                throw new ValidationException("Project contains both Media and MediaTemplate elements. Use only one or another (e.g. call project.Media.Clear()).");
            }

            if (project.MajorUpgradeStrategy != null)
            {
                if (project.MajorUpgradeStrategy.UpgradeVersions == null && project.MajorUpgradeStrategy.PreventDowngradingVersions == null)
                {
                    throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.UpgradeVersions and PreventDowngradingVersions are not defined.");
                }

                if (project.MajorUpgradeStrategy.UpgradeVersions != null)
                {
                    if (!IsValidVersion(project.MajorUpgradeStrategy.UpgradeVersions.Minimum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.UpgradeVersions.Minimum value is invalid.");

                    if (!IsValidVersion(project.MajorUpgradeStrategy.UpgradeVersions.Maximum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.UpgradeVersions.Maximum value is invalid.");
                }

                if (project.MajorUpgradeStrategy.PreventDowngradingVersions != null)
                {
                    if (!IsValidVersion(project.MajorUpgradeStrategy.PreventDowngradingVersions.Minimum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.PreventDowngradingVersions.Minimum value is invalid.");

                    if (!IsValidVersion(project.MajorUpgradeStrategy.PreventDowngradingVersions.Maximum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.PreventDowngradingVersions.Maximum value is invalid.");
                }
            }

            if (project is ManagedProject && !project.BackgroundImage.IsEmpty() && project.ValidateBackgroundImage)
            {
                bool invalidAspectRatio = false;
                try
                {
                    string imageFile = Utils.PathCombine(project.SourceBaseDir, project.BackgroundImage);
                    if (IO.File.Exists(imageFile))
                    {
                        using (var img = Bitmap.FromFile(imageFile))
                        {
                            if (img.Width > img.Height)
                                invalidAspectRatio = true;
                        }
                    }
                }
                catch { }

                if (invalidAspectRatio)
                    throw new ValidationException(
                        "Project.BackgroundImage has incompatible (with ManagedUI default dialogs) aspect ratio. The expected ratio is close to W(156)/H(312). " +
                        "The background image (left side banner) in ManagedUI dialogs is left-docked at runtime and if it's too wide it can push away (to right) " +
                        "the all other UI elements. " +
                        "You can suppress image validation by setting Project.ValidateBackgroundImage to 'false'.");
            }

            //important to use RawId to avoid triggering Id premature auto-generation
            if (project.AllDirs.Count(x => (x.RawId == Compiler.AutoGeneration.InstallDirDefaultId && Compiler.AutoGeneration.InstallDirDefaultId != null) || x.IsInstallDir) > 1)
                throw new ValidationException("More than a single Dir marked as InstallDir. Ensure that only a single directory marked as InstallDir with Dir.IsInstallDir property or with the id 'INSTALLDIR' value");

            foreach (Dir dir in project.AllDirs)
                if (dir.Name != null && (dir.Name.StartsWith("%") || dir.Name.EndsWith("%")))
                    if (!Compiler.EnvironmentConstantsMapping.ContainsKey(dir.Name))
                        throw new ValidationException("WixSharp.Dir.Name is set to unknown environment constant '" + dir.Name + "'.\n" +
                                      "For the list of supported constants analyze WixSharp.Compiler.EnvironmentConstantsMapping.Keys.");

            var incosnistentRefAsmActions =
                    project.Actions.OfType<ManagedAction>()
                                   .GroupBy(a => a.ActionAssembly)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => new
                                       {
                                           Assembly = g.Key,
                                           Info = g.Select(a => new { Name = a.MethodName, RefAsms = a.RefAssemblies.Select(r => Path.GetFileName(r)).ToArray() }).ToArray(),
                                           IsInconsistent = g.Select(action => action.GetRefAssembliesHashCode(project.DefaultRefAssemblies)).Distinct().Count() > 1,
                                       })
                                   .Where(x => x.IsInconsistent)
                                       .FirstOrDefault();

            if (incosnistentRefAsmActions != null)
            {
                var errorInfo = new StringBuilder();
                errorInfo.Append(">>>>>>>>>>>>\n");
                errorInfo.Append("Asm: " + incosnistentRefAsmActions.Assembly + "\n");
                foreach (var item in incosnistentRefAsmActions.Info)
                {
                    errorInfo.Append("    ----------\n");
                    errorInfo.Append("    Action: " + item.Name + "\n");
                    errorInfo.AppendFormat("    RefAsms: {0} items\n", item.RefAsms.Length);
                    foreach (var name in item.RefAsms)
                        errorInfo.Append("       - " + name + "\n");
                }
                errorInfo.Append(">>>>>>>>>>>>\n");

                throw new ApplicationException(string.Format("Assembly '{0}' is used by multiple ManagedActions but with the inconsistent set of referenced assemblies. " +
                                                             "Ensure that all declarations have the same referenced assemblies by either using identical declarations or by using " +
                                                             "Project.DefaultRefAssemblies.\n{1}", incosnistentRefAsmActions.Assembly, errorInfo));
            }

            // https://wixsharp.codeplex.com/discussions/646085
            // Have to disable validation as it only considers 'install' but not 'uninstall'.
            // Possible solution is to analyse the action.condition and determine if it is
            // install /uninstall but it is impossible to do. At least with the adequate accuracy.
            // var incosnistentInstalledFileActions = project.Actions
            //                                               .OfType<InstalledFileAction>()
            //                                               .Where(x => x.When != When.After || x.Step != Step.InstallExecute)
            //                                               .Any();
            // if (incosnistentInstalledFileActions)
            //     try
            //     {
            //         var msg = "Warning: InstalledFileAction should be scheduled for after InstallExecute. Otherwise it may produce undesired side effects.";
            //         Debug.WriteLine(msg);
            //         Console.WriteLine(msg);
            //     }
            //     catch { }
        }

        public static void ValidateCAAssembly(string caAssembly)
        {
            string dtfAssembly = typeof(CustomActionAttribute).Assembly.Location;

            // need to do it in a separate domain as we do not want to lock the assembly and
            // `ReflectionOnlyLoadFrom` is incompatible with the task

            if (Compiler.AutoGeneration.ValidateCAAssemblies == CAValidation.InRemoteAppDomain)
            {
                // throw new NotImplementedException("The method is not implemented on .NET Core");

                // will not lock the file and will unload the assembly
                // Utils.ExecuteInTempDomain<AsmReflector>(asmReflector =>
                //     {
                //         asmReflector.ValidateCAAssembly(caAssembly, dtfAssembly);
                //     });
            }
            else if (Compiler.AutoGeneration.ValidateCAAssemblies == CAValidation.InCurrentAppDomain)
            {
                // will not lock the file but will not unload the assembly
                new AsmReflector().ValidateCAAssemblyLocally(caAssembly, dtfAssembly);
            }
            else
            {
                // disabled
            }
        }
    }

    class AsmReflector : MarshalByRefObject
    {
        public string OriginalAssemblyFile(string file)
        {
            return Utils.OriginalAssemblyFile(file);
        }

        public string AssemblyScopeName(string file)
        {
#if !NETCORE
            return Reflection.Assembly.ReflectionOnlyLoad(System.IO.File.ReadAllBytes(file)).ManifestModule.ScopeName;
#else
            throw new NotImplementedException("Not supported on .NET Core builds");
#endif
        }

        public bool ValidateCustomBaAssembly(string assembly)
        {
            var asm = System.Reflection.Assembly.LoadFrom(assembly);
            var valid = asm.GetCustomAttributes(false)
                           .Any(x => x.GetType().FullName == "WixToolset.Mba.Core.BootstrapperApplicationFactoryAttribute");

            return valid;
        }

        public string[] GetRefAssemblies(string file)
        {
            return ReflectionExtensions.GetRefAssembliesOf(file);
        }

        public void ValidateCAAssembly(string file, string dtfAsm)
        {
            // `ValidateCAAssemblyImpl` will load assembly from `file` for validation. Though for this to happen
            // the AppDomain will need to be able resolve the only dependence assembly `file` has - dtfAsm.
            // Thus always resolve it to dtfAsm (regardless of `args.Name` value) when AssemblyResolve is fired.
            Reflection.Assembly resolver(object sender, ResolveEventArgs args)
            {
                return System.Reflection.Assembly.LoadFrom(dtfAsm);
            }

            AppDomain.CurrentDomain.AssemblyResolve += resolver;
            ValidateCAAssemblyImpl(file, dtfAsm, loadFromMemory: false);
            AppDomain.CurrentDomain.AssemblyResolve -= resolver;
        }

        internal void ValidateCAAssemblyLocally(string file, string dtfAsm)
        {
            ValidateCAAssemblyImpl(file, dtfAsm, loadFromMemory: true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal void ValidateCAAssemblyImpl(string file, string dtfAsm, bool loadFromMemory)
        {
            //Debug.Assert(false);
            try
            {
                var bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static;

                // `ReflectionOnlyLoadFrom` cannot preload all required assemblies and triggers
                // "System.InvalidOperationException: 'It is illegal to reflect on the custom attributes
                // of a Type loaded via ReflectionOnlyGetType (see Assembly.ReflectionOnly) -- use CustomAttributeData
                // instead.'" exception. Thus need to use `LoadFrom`, which locks the assembly unless the operation is
                // performed in the temp AppDomain, which is unloaded after at the end.
                // Unfortunately `AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve` does not help (does not get fired).

                // var assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(file);
                var assembly = loadFromMemory ?
                    Reflection.Assembly.Load(System.IO.File.ReadAllBytes(file)) :
                    Reflection.Assembly.LoadFrom(file);

                var caMembers = assembly.GetTypes()
                                        .SelectMany(t => t.GetMembers(bf)
                                                          .Where(mem => mem.GetCustomAttributes(false)
                                                          .Where(x => x.ToString() == "Microsoft.Deployment.WindowsInstaller.CustomActionAttribute")
                                                          .Any()))
                                        .ToArray();

                var invalidMembers = new List<string>();
                foreach (MemberInfo mi in caMembers)
                {
                    string fullName = mi.DeclaringType.FullName + "." + mi.Name;

                    if (!mi.DeclaringType.IsPublic)
                        if (!invalidMembers.Contains(fullName))
                            invalidMembers.Add(fullName);

                    if (mi.MemberType != MemberTypes.Method)
                    {
                        if (!invalidMembers.Contains(fullName))
                            invalidMembers.Add(fullName);
                    }
                    else
                    {
                        var method = (mi as MethodInfo);
                        if (!method.IsPublic || !method.IsStatic)
                            if (!invalidMembers.Contains(fullName))
                                invalidMembers.Add(fullName);
                    }
                }
                if (invalidMembers.Any())
                {
                    Compiler.OutputWriteLine("Warning: some of the type members are marked with [CustomAction] attribute but they don't meet the MakeSfxCA criteria of being public static method of a public type:\n");
                    foreach (var member in invalidMembers)
                        Compiler.OutputWriteLine("  " + member);
                    Compiler.OutputWriteLine("");
                }
            }
            catch { }
        }
    }
}