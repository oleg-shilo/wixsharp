[![Build status](https://ci.appveyor.com/api/projects/status/jruj9dmf2dwjn5p3?svg=true)](https://ci.appveyor.com/project/oleg-shilo/wixsharp)
[![NuGet version (WixSharp)](https://img.shields.io/nuget/v/CS-Script.svg?style=flat-square)](https://www.nuget.org/packages/WixSharp/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](http://www.csscript.net/Donation.html)

<img align="right" src="https://github.com/oleg-shilo/wixsharp/blob/master/Documentation/wiki_images/wixsharp_logo.png" alt="" style="float:right">

# Wix# (WixSharp) - managed interface for WiX

**_Framework for building a complete MSI or WiX source code by using script files written with the C# syntax._**

_In July 2014 Wix# was migrated to CodePlex [Wix#](https://wixsharp.codeplex.com/) and re-released under MIT license. It was subsequently migrated from CodePlex to GitHub. You can still find old releases and some useful content from the past discussions on CodePlex._

## Project Description

Wix# (WixSharp) is a member in the [CS-Script](https://csscriptsource.codeplex.com/) family. Wix# allows building a complete MSI or WiX source code by executing script files written with 
the plain C# syntax. Wix# engine uses a C# class structure to mimic WiX entities and their relationships in order to produce a valid deployment model.

Wix# answers many MSI authoring challenges. It solves the common MSI/WiX authoring limitations in a very elegant and yet unorthodox way. Wix# follows the steps of other 
[transcompilers](http://en.wikipedia.org/wiki/Source-to-source_compiler) like Script#, CoffeeScript or GWT by using source code of a more manageable syntax (C# in this case) to produce 
the desired source code of a less manageable syntax (WiX). A "more manageable syntax" in this context means less verbose and more readable code, better compile-time error checking and 
availability of more advanced tools.

Wix# also removes necessity to develop MSI sub-modules (Custom Actions) in the completely different language (e.g. C++) by allowing both the components and behaviour to be defined in the 
same language (C#). This also allows homogeneous, simplified and more consistent source code structure.

**_Overview_**

If you are planing to use Wix# on Linux you my find this [article](https://wixsharp.codeplex.com/wikipage?title=Using%20WixSharp%20On%20Linux) being useful.

You can find the instructions on how to author MSI setups with WixSharp in the [Documentation](https://github.com/oleg-shilo/wixsharp/wiki) section. And this section only highlights 
some of the available features.

> _You can use Visual Studio console application project and NuGet package as the starting point._
![image](https://github.com/oleg-shilo/wixsharp/raw/master/Documentation/wiki_images/nuget.png)

> _Alternatively you can install ["WixSharp Project Templates"](https://visualstudiogallery.msdn.microsoft.com/4e093ce7-be66-40ed-ab16-61a1186c530e) Visual Studio extension. Read more 
about the Wix# VS templates [here](https://github.com/oleg-shilo/wixsharp/wiki/VS2013-%E2%80%93-2015-Templates)._

Wix# allows very simple and expressive deployment definition. This is an example of a simple Wix# script:
```C#
using System;
using WixSharp;
 
class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(@"Files\Docs\Manual.txt"),
                              new File(@"Files\Bin\MyApp.exe")));
 
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
 
        Compiler.BuildMsi(project);
    }
}
```
One of the most intriguing features of Wix# is the ability to define/implement managed Custom Actions directly in the script file:
```C#
using System;
using System.Windows.Forms;
using WixSharp;
using Microsoft.Deployment.WindowsInstaller;
 
class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("CustomActionTest",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new DirFiles(@"Release\Bin\*.*")),
                          new ManagedAction(CustomActions.MyAction));
 
        BuildMsi(project);
    }
}
 
public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World!", "Embedded Managed CA");
        session.Log("Begin MyAction Hello World");
 
        return ActionResult.Success;
    }
}
``` 

Another important feature is the support for custom UI including WPF external UI:
![image](https://github.com/oleg-shilo/wixsharp/raw/master/Documentation/wiki_images/wpf_ui.png)

The package in the [Releases](https://github.com/oleg-shilo/wixsharp/releases) section contains an extensive collection of Wix# samples covering the following development scenarios:

* Visual Studio integration including [NuGet](https://www.nuget.org/packages/WixSharp/) packages and VS2013/2015 [project templates extension](https://visualstudiogallery.msdn.microsoft.com/4e093ce7-be66-40ed-ab16-61a1186c530e)
* Installing file(s) into Program Files directory
* Changing installation directory
* Installing shortcuts to installed files
* Conditional installations
* Installing Windows service
* Installing IIS Web site
* Modifying app config file as a post-install action (Deferred actions)
* Installing "Uninstall Product" shortcut into "Program Menu" directory
* Installing registry key
* Showing custom licence file during the installation
* Launching installed application after/during the installation with Custom Action
* Executing VBScript Custom Action
* Executing Managed (C#) Custom Action
* Executing conditional actions
* Targeting x64 OSs  
* Registering assembly in GAC
* File Type registration
* Setting/Reading MSI properties during the installation
* Run setup with no/minimal/full UI
* Localization
* Major Upgrade deployment
* Authoring and using MergeModules
* Pre-install registry search
* Customization of setup dialogs images
* Rebooting OS after the installation
* Building MSI with and without Visual Studio
* Simplified Managed bootstrapper for UI based deployments
* Simple Native bootstrapper
* Custom MSI dialogs
* Custom WinForms dialogs
* Custom external UI
* Console setup application
* WinForm setup application
* WPF setup application
