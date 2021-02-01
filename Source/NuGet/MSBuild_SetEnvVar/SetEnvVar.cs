using System;
using Microsoft.Build;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WixSharp.Build
{
    public class SetEnvVar : Task
    {
        [Required]
        public string Values { get; set; }

        public override bool Execute()
        {
            string[] vals = Values.Replace(";;", "$(separator)").Split(';');

            foreach (string keyValue in vals)
                try
                {
                    string[] parts = keyValue.Replace("$(separator)", ";").Split('=');
                    Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                }
                catch { }
            return true;
        }
    }
}