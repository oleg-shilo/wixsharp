using System.ComponentModel;

namespace WixSharp.Nsis
{
    /// <summary>
    /// A class that describes SetCompressor Operation from NSIS
    /// This command sets the compression algorithm used to compress files/data in the installer.
    /// It can only be used outside of sections and functions and before any data is compressed.
    /// Different compression methods can not be used for different files in the same installer.
    /// It is recommended to use it on the very top of the script to avoid compilation errors.
    /// </summary>
    public class Compressor
    {
        private readonly bool _isSolid;
        private readonly bool _isFinal;
        private readonly CompressionMethod _compressionMethod;

        /// <summary>
        /// Creates an instance of Compressor class which is used for building SetCompressor command
        /// </summary>
        /// <param name="isSolid">If /SOLID is used, all of the installer data is compressed in one block. This results in greater compression ratios.</param>
        /// <param name="isFinal">If /FINAL is used, subsequent calls to SetCompressor will be ignored.</param>
        /// <param name="compressionMethod">Three compression methods are supported: ZLIB, BZIP2 and LZMA.</param>
        public Compressor(bool isSolid, bool isFinal, CompressionMethod compressionMethod)
        {
            _isSolid = isSolid;
            _isFinal = isFinal;
            _compressionMethod = compressionMethod;
        }

        /// <summary>
        /// Used to build SetCompressor command based on Compressor state
        /// </summary>
        /// <returns>Built SetCompressor command</returns>
        public override string ToString() => 
            "SetCompressor " + (_isSolid ? "/SOLID " : string.Empty) + (_isFinal ? "/FINAL " : string.Empty) + _compressionMethod.GetDescription();

        /// <summary>
        /// Supported compressor Types
        /// </summary>
        public enum CompressionMethod
        {
            /// <summary>
            /// ZLIB (the default) uses the deflate algorithm, it is a quick and simple method. With the default compression level it uses about 300 KB of memory.
            /// </summary>
            [Description("zlib")]
            Zlib = 0,
            /// <summary>
            /// BZIP2 usually gives better compression ratios than ZLIB, but it is a bit slower and uses more memory. With the default compression level it uses about 4 MB of memory.
            /// </summary>
            [Description("bzip2")]
            Bzip2,
            /// <summary>
            /// LZMA is a new compression method that gives very good compression ratios. The decompression speed is high (10-20 MB/s on a 2 GHz CPU), the compression speed is lower. The memory size that will be used for decompression is the dictionary size plus a few KBs, the default is 8 MB.
            /// </summary>
            [Description("lzma")]
            Lzma
        }
    }
}