//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
	static public void Main(string[] args)
	{
		var project = new Project
		{
			Name = "CustomActionTest",
			UI = WUI.WixUI_ProgressOnly,

			Dirs = new[]
			{
				new Dir(@"%ProgramFiles%\My Company\My Product",
					new File(@"Files\Registrator.exe"))
			},
			Actions = new WixSharp.Action[]
			{
				new InstalledFileAction("Registrator.exe", "", Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed, "Registrator.exe", "/u") {Execute = Execute.deferred},
				new InstalledFileAction("Registrator.exe", "/u", Return.check, When.Before, Step.RemoveFiles, Condition.Installed, "Registrator.exe", "") {Execute = Execute.deferred},

				new ElevatedManagedAction(CustomActions.Install, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed, CustomActions.Rollback)
				{
					UsesProperties = "Prop=Install;",
					RollbackArg = "Prop=Rollback;",
					Execute = Execute.deferred
				},

				new CustomActionRef("WixFailWhenDeferred", When.Before, Step.InstallFinalize, "1"),
			}
		};

		project.IncludeWixExtension(WixExtension.Util);

		Compiler.PreserveTempFiles = true;

		Compiler.BuildMsi(project);
	}
}

public class CustomActions
{
	[CustomAction]
	public static ActionResult Install(Session session)
	{
		MessageBox.Show(String.Format("{0}", session.Property("Prop")), "Install");

		return ActionResult.Success;
	}

	[CustomAction]
	public static ActionResult Rollback(Session session)
	{
		MessageBox.Show(String.Format("{0}", session.Property("Prop")), "Rollback");

		return ActionResult.Success;
	}
}
