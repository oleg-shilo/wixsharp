using System.Runtime.InteropServices;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

// ----------------

// [CustomAction]
// public static ActionResult CustomActionCore(Session session)
// {
//     try
//     {
//         MsgBox("EnteryPoint", "WixSharp.Managed");
//         // // https://github.com/dotnet/corert/blob/master/Documentation/using-corert/reflection-in-aot-mode.md

//         // var required_for_aot_inclusion_of_reflection_metadata = typeof(Script).GetMembers(Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod);

//         // var handlerClass = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name.EndsWith("<>c"));
//         // var handlerMethod = (MethodInfo)handlerClass?
//         //     .GetMembers(Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod)?
//         //     .FirstOrDefault(x => x.Name == "<Main>b__5_0");

//         // // var arg = AotInterop.Convert(session);

//         // if (handlerClass != null)
//         // {
//         //     object instance = Activator.CreateInstance(handlerClass);
//         //     handlerMethod?.Invoke(instance, new object[] { null, null });
//         // }
//     }
//     catch (Exception e)
//     {
//         MsgBox(e.Message, "WixSharp.Managed");
//     }
//     return ActionResult.UserExit;
// }

// public class AotInterop
// {
// public static SetupEventArgs Convert(Session session)
// {
//     //Debugger.Launch();
//     var result = new SetupEventArgs { Session = session };
//     try
//     {
//         string data = session.Property("WIXSHARP_RUNTIME_DATA");
//         result.Data.InitFrom(data);
//         SetEnvironmentVariables(result.Data);
//         SetEnvironmentVariables(session.CustomActionData);

//         // InjectWixSharpRuntimeData(session);
//     }
//     catch (Exception e)
//     {
//         session.Log(e.Message);
//     }
//     return result;
// }

// static IDictionary<string, string> SetEnvironmentVariables(IDictionary<string, string> data)
// {
//     foreach (var key in data.Keys)
//         Environment.SetEnvironmentVariable(key, data[key]);

//     return data;
// }

// static void InjectWixSharpRuntimeData(Session session)
// {
//     var data = new SetupEventArgs.AppData();
//     try
//     {
//         data["Installed"] = session["Installed"];
//         data["REMOVE"] = session["REMOVE"];
//         data["ProductName"] = session["ProductName"];
//         data["ProductCode"] = session["ProductCode"];
//         data["UpgradeCode"] = session["UpgradeCode"];
//         data["REINSTALL"] = session["REINSTALL"];
//         data["MsiFile"] = session["OriginalDatabase"];
//         data["UPGRADINGPRODUCTCODE"] = session["UPGRADINGPRODUCTCODE"];
//         data["FOUNDPREVIOUSVERSION"] = session["FOUNDPREVIOUSVERSION"];
//         data["UILevel"] = session["UILevel"];
//         data["WIXSHARP_MANAGED_UI"] = session["WIXSHARP_MANAGED_UI"];
//         data["WIXSHARP_MANAGED_UI_HANDLE"] = session["WIXSHARP_MANAGED_UI_HANDLE"];
//     }
//     catch (Exception e)
//     {
//         session.Log(e.Message);
//     }

//     data.MergeReplace(session["WIXSHARP_RUNTIME_DATA"]);

//     session["WIXSHARP_RUNTIME_DATA"] = data.ToString();
// }

// [UnmanagedCallersOnly(EntryPoint = "OnLoad")]
// public static uint OnLoadAot(IntPtr handle)
// {
// }

//     [UnmanagedCallersOnly(EntryPoint = "CustomActionCore")]
//     public static uint CustomActionCore(IntPtr handle)
//            => (uint)Script.CustomActionCore(Session.FromHandle(handle, false));
// }
// ----------------

// [CustomAction]
// public static ActionResult CustomActionCore(Session session)
// {
//     try
//     {
//         MsgBox("EnteryPoint", "WixSharp.Managed");
//         // // https://github.com/dotnet/corert/blob/master/Documentation/using-corert/reflection-in-aot-mode.md

//         // var required_for_aot_inclusion_of_reflection_metadata = typeof(Script).GetMembers(Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod);

//         // var handlerClass = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name.EndsWith("<>c"));
//         // var handlerMethod = (MethodInfo)handlerClass?
//         //     .GetMembers(Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod)?
//         //     .FirstOrDefault(x => x.Name == "<Main>b__5_0");

//         // // var arg = AotInterop.Convert(session);

//         // if (handlerClass != null)
//         // {
//         //     object instance = Activator.CreateInstance(handlerClass);
//         //     handlerMethod?.Invoke(instance, new object[] { null, null });
//         // }
//     }
//     catch (Exception e)
//     {
//         MsgBox(e.Message, "WixSharp.Managed");
//     }
//     return ActionResult.UserExit;
// }

// public class AotInterop
// {
// public static SetupEventArgs Convert(Session session)
// {
//     //Debugger.Launch();
//     var result = new SetupEventArgs { Session = session };
//     try
//     {
//         string data = session.Property("WIXSHARP_RUNTIME_DATA");
//         result.Data.InitFrom(data);
//         SetEnvironmentVariables(result.Data);
//         SetEnvironmentVariables(session.CustomActionData);

//         // InjectWixSharpRuntimeData(session);
//     }
//     catch (Exception e)
//     {
//         session.Log(e.Message);
//     }
//     return result;
// }

// static IDictionary<string, string> SetEnvironmentVariables(IDictionary<string, string> data)
// {
//     foreach (var key in data.Keys)
//         Environment.SetEnvironmentVariable(key, data[key]);

//     return data;
// }

// static void InjectWixSharpRuntimeData(Session session)
// {
//     var data = new SetupEventArgs.AppData();
//     try
//     {
//         data["Installed"] = session["Installed"];
//         data["REMOVE"] = session["REMOVE"];
//         data["ProductName"] = session["ProductName"];
//         data["ProductCode"] = session["ProductCode"];
//         data["UpgradeCode"] = session["UpgradeCode"];
//         data["REINSTALL"] = session["REINSTALL"];
//         data["MsiFile"] = session["OriginalDatabase"];
//         data["UPGRADINGPRODUCTCODE"] = session["UPGRADINGPRODUCTCODE"];
//         data["FOUNDPREVIOUSVERSION"] = session["FOUNDPREVIOUSVERSION"];
//         data["UILevel"] = session["UILevel"];
//         data["WIXSHARP_MANAGED_UI"] = session["WIXSHARP_MANAGED_UI"];
//         data["WIXSHARP_MANAGED_UI_HANDLE"] = session["WIXSHARP_MANAGED_UI_HANDLE"];
//     }
//     catch (Exception e)
//     {
//         session.Log(e.Message);
//     }

//     data.MergeReplace(session["WIXSHARP_RUNTIME_DATA"]);

//     session["WIXSHARP_RUNTIME_DATA"] = data.ToString();
// }

// [UnmanagedCallersOnly(EntryPoint = "OnLoad")]
// public static uint OnLoadAot(IntPtr handle)
// {
// }

//     [UnmanagedCallersOnly(EntryPoint = "CustomActionCore")]
//     public static uint CustomActionCore(IntPtr handle)
//            => (uint)Script.CustomActionCore(Session.FromHandle(handle, false));
// }