#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted, 
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using System.Diagnostics;

namespace WixSharp
{
    /// <summary>
    /// Wix# wrapper around <see cref="T:System.Guid"/>. <see cref="WixGuid"/> allows generation of continuous
    /// GUIDs for for composing reproducible WiX source files.
    /// </summary>
    public class WixGuid : WixObject
    {
        /// <summary>
        /// GUID value of the <see cref="WixGuid"/> instance.
        /// </summary>
        public Guid Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="WixGuid"/> class.
        /// </summary>
        /// <param name="guid">The GUID value of the instance to be created.</param>
        public WixGuid(string guid)
        {
            Value = new Guid(guid);
        }

        #region SequentialGuid
        /// <summary>
        /// Class for generation of sequential <see cref="T:System.Guid"/>. 
        /// <para>Based on http://developmenttips.blogspot.com/2008/03/generate-sequential-guids-for-sql.html</para> 
        /// <para>This class is used by Wix# engine internally to emit reproducible WiX code.</para>
        /// </summary>
        public class SequentialGuid
        {
            /// <summary>
            /// Gets or sets the current GUID.
            /// </summary>
            /// <value>The current GUID.</value>
            public Guid CurrentGuid { get; private set; }

            /// <summary>
            /// Increments and returns the current GUID.
            /// </summary>
            /// <value>The current GUID.</value>
            public Guid Next()
            {
                SequentialGuid s = this;
                return (s++).CurrentGuid;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SequentialGuid"/> class.
            /// </summary>
            public SequentialGuid()
            {
                CurrentGuid = Guid.NewGuid();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SequentialGuid"/> class.
            /// </summary>
            /// <param name="previousGuid">The previous GUID.</param>
            public SequentialGuid(Guid previousGuid)
            {
                CurrentGuid = previousGuid;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="SequentialGuid"/> class.
            /// </summary>
            /// <param name="previousGuid">The previous GUID.</param>
            public SequentialGuid(Guid? previousGuid)
            {
                CurrentGuid = previousGuid.Value;
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="SequentialGuid"/> to <see cref="Guid"/>.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator Guid(SequentialGuid value)
            {
                return value.CurrentGuid;
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="Guid"/> to <see cref="SequentialGuid"/>.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator SequentialGuid(Guid value)
            {
                return new SequentialGuid(value);
            }

            /// <summary>
            /// Implements the operator ++.
            /// </summary>
            /// <param name="sequentialGuid">The sequential GUID.</param>
            /// <returns>The result of the operator.</returns>
            public static SequentialGuid operator ++(SequentialGuid sequentialGuid)
            {
                byte[] bytes = sequentialGuid.CurrentGuid.ToByteArray();
                for (int mapIndex = 0; mapIndex < 16; mapIndex++)
                {
                    int bytesIndex = orderMap[mapIndex];
                    bytes[bytesIndex]++;
                    if (bytes[bytesIndex] != 0)
                    {
                        break; // No need to increment more significant bytes
                    }
                }
                sequentialGuid.CurrentGuid = new Guid(bytes);
                return sequentialGuid;
            }

            // 3 - the least significant byte in Guid ByteArray 
            // 15 - the most significant byte in Guid ByteArray
            private static readonly int[] orderMap = new int[16] { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };
            static void PrintOrderMap()
            {
                Action<string> print = (str) =>
                {
                    var bytes = new Guid(str).ToByteArray();
                    for (int i = 0; i < 16; i++)
                        if (bytes[i] != 0)
                        {
                            Trace.Write(i + ", ");
                            break;
                        }
                };

                Trace.WriteLine("");
                print("00000000-0000-0000-0000-000000000001");
                print("00000000-0000-0000-0000-000000000100");
                print("00000000-0000-0000-0000-000000010000");
                print("00000000-0000-0000-0000-000001000000");
                print("00000000-0000-0000-0000-000100000000");
                print("00000000-0000-0000-0000-010000000000");
                print("00000000-0000-0000-0001-000000000000");
                print("00000000-0000-0000-0100-000000000000");
                print("00000000-0000-0001-0000-000000000000");
                print("00000000-0000-0100-0000-000000000000");
                print("00000000-0001-0000-0000-000000000000");
                print("00000000-0100-0000-0000-000000000000");
                print("00000001-0000-0000-0000-000000000000");
                print("00000100-0000-0000-0000-000000000000");
                print("00010000-0000-0000-0000-000000000000");
                print("01000000-0000-0000-0000-000000000000");
                Trace.WriteLine("");
            }
        }
        #endregion

        /// <summary>
        /// Initial value to be used as a seed for Guid generation. If you want the Wix# project items to contain 
        /// not random but reproducible sequential Guids you should initialize this field.
        /// </summary>
        static public SequentialGuid ConsistentGenerationStartValue = new SequentialGuid();

        /// <summary>
        /// Returns new GUID.
        /// </summary>
        /// <returns></returns>
        static public Guid NewGuid()
        {
            //if (ConsistentGenerationStartValue != null)
            //    return ConsistentGenerationStartValue++;
            //else
            //    return Guid.NewGuid();
            var seed = rnd.Next(int.MaxValue);
            return Generator(seed);
        }
        static Random rnd = new Random(1);

        /// <summary>
        /// Returns new GUID.
        /// </summary>
        /// <returns></returns>
        static public Guid NewGuid(object seed)
        {
            return Generator(seed);
        }

        static Func<object, Guid> generator = null;

        /// <summary>
        /// Gets or sets the GUID generation algorithm.
        /// </summary>
        /// <value>
        /// The generator.
        /// </value>
        static public Func<object, Guid> Generator
        {
            get
            {
                if (generator == null)
                {
                    generator = GuidGenerators.Default;
                }
                return generator;
            }
            set { generator = value; }
        }



        static int[] orderMap = new int[16] { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };
        /// <summary>
        /// Hashes the Guid by the specified integer hash value.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="hashValue">The hash value.</param>
        /// <returns>GUID value.</returns>
        public static Guid HashGuidByInteger(Guid guid, int hashValue)
        {
            byte[] bytes = guid.ToByteArray();
            byte[] hashBytes = BitConverter.GetBytes(hashValue);
            //Debug.Assert(bytes.Length > hashBytes.Length);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                int bytesIndex = orderMap[i];
                bytes[bytesIndex] = (byte)(bytes[bytesIndex] + hashBytes[i]);
            }
            return new Guid(bytes);
        }
    }

    /// <summary>
    /// Collection of the deterministic GUID generation algorithms
    /// </summary>
    public static class GuidGenerators
    {
        /// <summary>
        /// Returns sequentially incremented GUID. The specified seed is ignored. Every consecutive 
        /// call to this method will increase the last returned GUID by 1 and return its value. 
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public static Guid Sequential(object seed)
        {
            if (WixGuid.ConsistentGenerationStartValue != null)
            {
                //seed is ignored
                WixGuid.ConsistentGenerationStartValue++;
                return WixGuid.ConsistentGenerationStartValue;
            }
            else
                return Guid.NewGuid();
        }

        /// <summary>
        /// Default GUID generation algorithm.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public static Guid Default(object seed)
        {
            return WixGuid.HashGuidByInteger(WixGuid.ConsistentGenerationStartValue.CurrentGuid, seed.ToString().GetHashCode32());
        }
    }
}
