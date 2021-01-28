using System.ComponentModel;
// ReSharper disable InconsistentNaming

namespace WixSharp.Nsis.WinVer
{
    /// <summary>
    /// Provides a windows version that can be used with AtLeastWin/IsWin/AtMostWin NSIS WinVer operators
    /// </summary>
    public enum WindowsVersionNumber
    {
        /// <summary>
        /// Represents Windows 95
        /// </summary>
        [Description("95")]
        _95,
        
        /// <summary>
        /// Represents Windows 98
        /// </summary>
        [Description("98")]
        _98,
        
        /// <summary>
        /// Represents Windows ME
        /// </summary>
        [Description("ME")]
        ME,
        
        /// <summary>
        /// Represents Windows NT4
        /// </summary>
        [Description("NT4")]
        NT4,
        
        /// <summary>
        /// Represents Windows 2000
        /// </summary>
        [Description("2000")]
        _2000,
        
        /// <summary>
        /// Represents Windows XP
        /// </summary>
        [Description("XP")]
        XP,
        
        /// <summary>
        /// Represents Windows 2003
        /// </summary>
        [Description("2003")]
        _2003,
        
        /// <summary>
        /// Represents Windows Vista
        /// </summary>
        [Description("Vista")]
        Vista,
        
        /// <summary>
        /// Represents Windows 2008
        /// </summary>
        [Description("2008")]
        _2008,
        
        /// <summary>
        /// Represents Windows 7
        /// </summary>
        [Description("7")]
        _7,
        
        /// <summary>
        /// Represents Windows 2008R2
        /// </summary>
        [Description("2008R2")]
        _2008R2,
        
        /// <summary>
        /// Represents Windows 8
        /// </summary>
        [Description("8")]
        _8,
        
        /// <summary>
        /// Represents Windows 2012
        /// </summary>
        [Description("2012")]
        _2012,
        
        /// <summary>
        /// Represents Windows 8.1
        /// </summary>
        [Description("8.1")]
        _8_1,
        
        /// <summary>
        /// Represents Windows 2012R2
        /// </summary>
        [Description("2012R2")]
        _2012R2,
        
        /// <summary>
        /// Represents Windows 10
        /// </summary>
        [Description("10")]
        _10
    }
}