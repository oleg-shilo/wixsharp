using System;
using IO = System.IO;

namespace WixSharp.Nsis
{
    /// <summary>
    /// Defines a splash screen for the <see cref="NsisBootstrapper"/>.
    /// </summary>
    public class SplashScreen
    {
        /// <summary>
        /// Creates instance of the <see cref="SplashScreen"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="fileName">The splash bitmap filename. Only BMP files are supported.</param>
        /// <exception cref="ArgumentNullException">filename is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">filename is not in .BMP format.</exception>
        public SplashScreen(string fileName) : this(fileName, TimeSpan.FromSeconds(1))
        {
        }

        /// <summary>
        /// Creates instance of the <see cref="SplashScreen"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="fileName">The splash bitmap filename. Only BMP files are supported.</param>
        /// <param name="delay">The length to show the splash screen.</param>
        /// <exception cref="ArgumentNullException">filename is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">filename is not in .BMP format.</exception>
        public SplashScreen(string fileName, TimeSpan delay)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), nameof(fileName) + " is a null reference or empty");
            }

            var extension = IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension) || !extension.Equals(".BMP", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException(nameof(fileName), "Only BMP files are supported. FileName:" + fileName);
            }

            FileName = fileName;
            Delay = delay;
        }

        /// <summary>
        /// Gets or sets the splash bitmap filename.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the length to show the screen for.
        /// </summary>
        public TimeSpan Delay { get; set; }
    }
}