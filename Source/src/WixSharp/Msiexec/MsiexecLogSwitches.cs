using System;
using System.ComponentModel;

namespace WixSharp.Msiexec
{
    /// <summary>
    /// Represents Msiexec log options
    /// </summary>
    [Flags]
    public enum MsiexecLogSwitches
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Status messages.
        /// </summary>
        [Description("I")]
        I = 1,

        /// <summary>
        /// Nonfatal warnings.
        /// </summary>
        [Description("W")]
        W = 2,

        /// <summary>
        /// All error messages.
        /// </summary>
        [Description("E")]
        E = 4,

        /// <summary>
        /// Start up of actions.
        /// </summary>
        [Description("A")]
        A = 8,

        /// <summary>
        /// Action-specific records.
        /// </summary>
        [Description("R")]
        R = 16,

        /// <summary>
        /// User requests.
        /// </summary>
        [Description("U")]
        U = 32,

        /// <summary>
        /// Initial UI parameters.
        /// </summary>
        [Description("C")]
        C = 64,

        /// <summary>
        /// Out-of-memory or fatal exit information.
        /// </summary>
        [Description("M")]
        M = 128,

        /// <summary>
        /// Out-of-disk-space messages.
        /// </summary>
        [Description("O")]
        O = 256,

        /// <summary>
        /// Terminal properties.
        /// </summary>
        [Description("P")]
        P = 512,

        /// <summary>
        /// Verbose output.
        /// </summary>
        [Description("V")]
        V = 1024,

        /// <summary>
        /// Extra debugging information.
        /// </summary>
        [Description("X")]
        X = 2048,

        /// <summary>
        /// Append to existing log file.
        /// </summary>
        [Description("+")]
        Append = 4096,

        /// <summary>
        /// Flush each line to the log.
        /// </summary>
        [Description("!")]
        FlushEachLine = 8192,

        /// <summary>
        /// Log all information, except for v and x options.
        /// </summary>
        [Description("*")]
        Star = 16384,
    }
}