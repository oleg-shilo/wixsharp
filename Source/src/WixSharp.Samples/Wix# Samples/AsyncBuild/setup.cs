//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WixSharp;
using WixSharp.CommonTasks;

static class Script
{
    static public void Main()
    {
        Run();
    }

    static async void Run()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("setup.cs")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        // WixSharpAsyncHelper.ExecuteInNewContext(()=>project.BuildMsi()).Wait();
        // or
        // await WixSharpAsyncHelper.ExecuteInNewContext(()=>project.BuildMsi());
        // or

        await project.BuildAsync();

        Console.WriteLine("All done...");
    }
}

public static class WixSharpAsyncHelper
{
    public static Task<string> BuildAsync(this Project project)
    {
        return ExecuteInNewContext(() => project.BuildMsi());
    }

    public static Task<T> ExecuteInNewContext<T>(Func<T> action)
    {
        var taskResult = new TaskCompletionSource<T>();

        var asyncFlow = ExecutionContext.SuppressFlow();

        try
        {
            Task.Run(() =>
                {
                    try
                    {
                        var result = action();

                        taskResult.SetResult(result);
                    }
                    catch (Exception exception)
                    {
                        taskResult.SetException(exception);
                    }
                })
                .Wait();
        }
        finally
        {
            asyncFlow.Undo();
        }

        return taskResult.Task;
    }
}