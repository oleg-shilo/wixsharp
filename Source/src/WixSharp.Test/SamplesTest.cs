using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

using IO = System.IO;

namespace WixSharp.Test
{
    public class SamplesTest
    {
        IEnumerable<string> nonMsiProjects = @"CustomAttributes,
                                               External_UI,
                                               Custom_IDs,
                                               Self-executable_Msi,
                                               MultiLanguageUI,
                                               ASP.NETApp,
                                               EnvVariables"
                                .Split(',').Select(x => x.Trim());

        string[] nonTestableProjects = "MultiLanguageUI".Split(',').Select(x => x.Trim()).ToArray();

        string[] nonPortedWix4Projects = (""             // WIX4-TODO: WiX4 defect (cannot find element from the valid extension)
                                          ).Split(',').Select(x => x.Trim()).ToArray();

        int completedSamples = 0;
        int samplesTotal = 0;
        Stopwatch testTime = new Stopwatch();

        [Fact()]
        public void CanBuildAllSamples()
        {
            // it's no longer holding any value to test building samples from shell as they are all now built as part of the
            // solution build.
            // return;

            if (Environment.GetEnvironmentVariable("APPVEYOR") != null)
                return;

            // need to exclude some samples; for example the two samples from the same dir will interfere with each other;
            // or some other tests are built as a part of the solution
            string[] exclude = new string[] { };

            var failedSamples = new List<string>();
            int startStep = 0;
            int? howManyToRun = null; //null - all
            int? whichOneToRun = null; //null - all
            int sampleDirIndex = 0;

            var files = Directory.GetFiles(@"..\..\..\WixSharp.Samples\Wix# Samples", "build*.cmd", SearchOption.AllDirectories)
                                 .OrderBy(x => x)
                                 .Where(x => !x.PathGetFileName().ToLower().Contains("build_"))
                                 .ToArray();

            var compiled_scripts = Directory.GetFiles(@"..\..\..\WixSharp.Samples\Wix# Samples", "setup*.cs.dll", SearchOption.AllDirectories);

            compiled_scripts.ForEach(x => System.IO.File.Delete(x));

            files = files.Where(f => !exclude.Any(y => f.EndsWith(y, ignoreCase: true))).ToArray();

            testTime.Reset();
            testTime.Start();

            var samples = files.GroupBy(x => Path.GetDirectoryName(x));

            var allSamples = samples.Select(g => new { Category = g.Key, Items = g, Index = ++sampleDirIndex })
                                    .OrderBy(x => x.Index)
                                    .ToArray();

            allSamples = allSamples.Skip(startStep).ToArray();

            if (whichOneToRun.HasValue)
                allSamples = new[] { allSamples[whichOneToRun.Value] };

            if (howManyToRun.HasValue)
                allSamples = allSamples.Take(howManyToRun.Value).ToArray();

            samplesTotal = allSamples.Count();

            var parallel = false;

#if DEBUG
            parallel = true;
            ShowLogFileToObserveProgress();
            // System.Diagnostics.Process.GetProcessesByName("cmd").ToList().ForEach(x => { try { x.Kill(); } catch { } });
            // System.Diagnostics.Process.GetProcessesByName("cscs").ToList().ForEach(x => { try { x.Kill(); } catch { } });
            // System.Diagnostics.Process.GetProcessesByName("conhost").ToList().ForEach(x => { try { x.Kill(); } catch { } });
#endif

            void processDir(dynamic group)
            {
                string sampleDir = group.Category;

                var sampleFiles = Directory.GetFiles(sampleDir, "build*.cmd")
                                           .Select(x => Path.GetFullPath(x))
                                           .Where(x => !x.PathGetFileName().ToLower().Contains("build_"))
                                           .ToArray();
                foreach (string batchFile in sampleFiles)
                {
                    BuildSample(batchFile, group.Index, failedSamples);
                }
            };

            if (parallel)
            {
                // allSamples.ForEach(item =>
                //     ThreadPool.QueueUserWorkItem(x =>
                //         processDir(item)));

                Parallel.ForEach(allSamples, processDir);
                while (completedSamples < samplesTotal)
                {
                    Thread.Sleep(1000);
                }
            }
            else
            {
                foreach (var item in allSamples)
                    processDir(item);
            }

            testTime.Stop();
            LogAppend("\r\n--- END ---");

            if (failedSamples.Any())
            {
                string error = " Completed Samples: " + completedSamples + "\r\n Failed Samples:\r\n" + string.Join(Environment.NewLine, failedSamples.ToArray());
                Assert.True(false, error);
            }
        }

        void BuildSample(string batchFile, int currentStep, List<string> failedSamples)
        {
            try
            {
                var dir = Path.GetDirectoryName(batchFile);

                bool ignorePresentMsi = (dir.EndsWith("Self-executable_Msi", true));

                bool nonMsi = nonMsiProjects.Where(x => batchFile.Contains(x)).Any();
                bool ignoreSample = nonPortedWix4Projects.Concat(nonTestableProjects).Where(x => batchFile.Contains(x)).Any();
                if (ignoreSample)
                    return;

                if (!nonMsi && !ignorePresentMsi)
                {
                    DeleteAllMsis(dir);
                    Assert.False(HasAnyMsis(dir), "Cannot clear directory for the test...");
                }

                DisablePause(batchFile);

                string output = Run(batchFile);

                IO.File.WriteAllText(batchFile + ".log",
                    $"CurrDir: {Environment.CurrentDirectory}{Environment.NewLine}" +
                    $"Cmd: {batchFile}{Environment.NewLine}" +
                    $"======================================{Environment.NewLine}" +
                    $"{output}");

                if (output.Contains(" : error") || output.Contains("Error: ") || (!nonMsi && !HasAnyMsis(dir)))
                {
                    if (batchFile.EndsWith(@"Signing\Build.cmd") && output.Contains("SignTool Error:"))
                    {
                        //just ignore as the certificate is just a demo certificate
                    }
                    else
                        lock (failedSamples)
                        {
                            failedSamples.Add((currentStep - 1) + ":" + batchFile); // print index so it's easy to find it in the log
                        }
                }

                if (!nonMsi)
                    DeleteAllMsis(dir);
            }
            catch (Exception e)
            {
                lock (failedSamples)
                {
                    failedSamples.Add(currentStep + ":" + batchFile + "\t" + e.Message.Replace("\r\n", "\n").Replace("\n", ""));
                }
            }
            finally
            {
                completedSamples++;
                Log(currentStep, failedSamples);
                RestorePause(batchFile);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "useful for debugging")]
        void Log(int currentStep, List<string> failedSamples)
        {
            lock (failedSamples)
            {
                var logFile = @"..\..\..\WixSharp.Samples\test_progress.txt";
                var content = string.Format("Failed-{0}; Total Completed-{1}; Scenarios-{2}; Time-{3}\r\n", failedSamples.Count, completedSamples, samplesTotal, testTime.Elapsed) + string.Join(Environment.NewLine, failedSamples.ToArray());
                IO.File.WriteAllText(logFile, content);
            }
        }

        void LogAppend(string text)
        {
            var logFile = @"..\..\..\WixSharp.Samples\test_progress.txt";
            using (var writer = new StreamWriter(logFile, true))
                writer.WriteLine(text);
        }

        void ShowLogFileToObserveProgress()
        {
            var logFile = @"..\..\..\WixSharp.Samples\test_progress.txt".PathGetFullPath();
            if (IO.File.Exists(logFile))
                IO.File.WriteAllText(logFile, "The file will be updated as soon as the samples will start being tested.");

            try
            {
                // sublimme is always installed in x64 progfiles, but this process is x86 process so
                // envar always returns x86 location fro %PROGRAMFILES%
                Process.Start(@"%PROGRAMFILES%\Sublime Text 3\sublime_text.exe".ExpandEnvVars().Replace(" (x86)", ""), $"\"{logFile}\"");
            }
            catch
            {
                try
                {
                    Process.Start("notepad++.exe", $"\"{logFile}\"");
                }
                catch
                {
                    // notepad does not support reloading on changes
                    // Process.Start("notepad.exe", $"\"{logFile}\"");
                }
            }
        }

        void DisablePause(string batchFile)
        {
            var batchContent = IO.File.ReadAllText(batchFile).Replace("\npause", "\nrem pause");
            IO.File.WriteAllText(batchFile, batchContent);
        }

        void RestorePause(string batchFile)
        {
            var batchContent = IO.File.ReadAllText(batchFile).Replace("\nrem pause", "\npause");
            IO.File.WriteAllText(batchFile, batchContent);
        }

        bool HasAnyMsis(string dir)
        {
            return Directory.GetFiles(dir, "*.ms?").Any();
        }

        void DeleteAllMsis(string dir)
        {
            foreach (var msiFile in Directory.GetFiles(dir, "*.ms?"))
                System.IO.File.Delete(msiFile);
        }

        string Run(string batchFile)
        {
            var process = new Process();
            process.StartInfo.FileName = batchFile;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(batchFile);
            process.Start();

            return process.StandardOutput.ReadToEnd();

            // string line;
            // var output = new StringBuilder();
            // while (null != (line = process.StandardOutput.ReadLine()))
            // {
            //     output.AppendLine(line);
            // }

            // process.WaitForExit();

            // return output.ToString();
        }
    }
}