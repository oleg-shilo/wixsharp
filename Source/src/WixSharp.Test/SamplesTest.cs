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
                                               ASP.NETApp,
                                               EnvVariables,
                                               WixBootstrapper"
                                .Split(',').Select(x => x.Trim());

        int completedSamples = 0;
        int samplesTotal = 0;
        Stopwatch testTime = new Stopwatch();

        [Fact]
        public void CanBuildAllSamples()
        {
            if (Environment.GetEnvironmentVariable("APPVEYOR") != null)
                return;

            //need to exclude some samples; for example the two samples from the same dir will interfere with each other;
            string[] exclude = new string[] { };

            var failedSamples = new List<string>();
            int startStep = 0;
            int? howManyToRun = null; //null - all
            // howManyToRun = 10;
            int sampleDirIndex = 0;

            var files = Directory.GetFiles(@"..\..\..\WixSharp.Samples\Wix# Samples", "build*.cmd", SearchOption.AllDirectories);

            files = files.Where(f => !exclude.Any(y => f.EndsWith(y))).ToArray();

            files = files.Skip(startStep).ToArray();

            // files = files.Where(x => x.Contains("DeferredActions")).ToArray();

            if (howManyToRun.HasValue)
                files = files.Take(howManyToRun.Value).ToArray();

            samplesTotal = files.Count();
            testTime.Reset();
            testTime.Start();

            var samples = files.GroupBy(x => Path.GetDirectoryName(x));

            var allSamples = samples.Select(g => new { Category = g.Key, Items = g, Index = ++sampleDirIndex });

            var activeBuilds = 0;
            Parallel.ForEach(allSamples, (group) =>
            {
                activeBuilds++;
                Debug.WriteLine($">>>>>>>>>>> {activeBuilds} >>>>>>>>>>>>");
                string sampleDir = group.Category;

                var tempFiles = Directory.GetFiles(sampleDir, "*.dll")
                                         .Select(x => new { dll = x, pdb = x.PathChangeExtension(".pdb") })
                                         .Where(x => IO.File.Exists(x.pdb))
                                         .ToArray();

                var sampleFiles = Directory.GetFiles(sampleDir, "build*.cmd")
                                           .Select(x => Path.GetFullPath(x))
                                           .ToArray();

                foreach (string batchFile in sampleFiles)
                {
                    foreach (var item in tempFiles)
                    {
                        item.dll.DeleteIfExists();
                        item.pdb.DeleteIfExists();
                    }

                    if (!BuildSample(batchFile, group.Index, failedSamples))
                        BuildSample(batchFile, group.Index, failedSamples); // just in case try again. MSI builds are flake
                }
                activeBuilds--;
                Debug.WriteLine($"<<<<<<<<<<<< {activeBuilds} <<<<<<<<<<<<");
            });

            while (completedSamples < samplesTotal)
                Thread.Sleep(1000);

            testTime.Stop();
            LogAppend("\r\n--- END ---");

            if (failedSamples.Any())
            {
                string error = " Completed Samples: " + completedSamples + "\r\n Failed Samples:\r\n" + string.Join(Environment.NewLine, failedSamples.ToArray());
                Assert.True(false, error);
            }
        }

        bool BuildSample(string batchFile, int currentStep, List<string> failedSamples)
        {
            var result = true;

            string errorOutput = "";

            try
            {
                var dir = Path.GetDirectoryName(batchFile);

                bool nonMsi = nonMsiProjects.Where(x => batchFile.Contains(x)).Any();

                if (!nonMsi)
                {
                    DeleteAllMsis(dir);
                    Assert.False(HasAnyMsis(dir), "Cannot clear directory for the test...");
                }

                DisablePause(batchFile);

                string output = Run(batchFile);

                if (output.Contains(" : error") || output.Contains("Error: ") || (!nonMsi && !HasAnyMsis(dir)))
                {
                    result = false;
                    errorOutput = output;

                    if (batchFile.EndsWith(@"Signing\Build.cmd") && output.Contains("SignTool Error:"))
                    {
                        //just ignore as the certificate is just a demo certificate
                    }
                    else
                        lock (failedSamples)
                        {
                            var prefix = $"{currentStep}:";
                            failedSamples.RemoveAll(x => x.StartsWith(prefix));
                            failedSamples.Add(prefix + batchFile);
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
                Log(currentStep, failedSamples, errorOutput);
                RestorePause(batchFile);
            }

            return result;
        }

        void Log(int currentStep, List<string> failedSamples, string output)
        {
            lock (failedSamples)
            {
                var items = failedSamples.Distinct().ToList();

                var logFile = @"..\..\..\WixSharp.Samples\test_progress.txt";
                var content = string.Format("Failed-{0}; Completed-{1}; Total-{2}; Time-{3}\r\n",
                    items.Count, completedSamples, samplesTotal, testTime.Elapsed) + string.Join(Environment.NewLine, items.ToArray());
                IO.File.WriteAllText(logFile, content);

                if (output.IsNotEmpty())
                    IO.File.AppendAllText(logFile.PathChangeExtension(".log"), $"\r\n{currentStep}: {output}");
            }
        }

        void LogAppend(string text)
        {
            var logFile = @"..\..\..\WixSharp.Samples\test_progress.txt";
            using (var writer = new StreamWriter(logFile, true))
                writer.WriteLine(text);
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

            string line;
            var output = new StringBuilder();
            while (null != (line = process.StandardOutput.ReadLine()))
            {
                output.AppendLine(line);
            }

            process.WaitForExit();

            return output.ToString();
        }
    }
}