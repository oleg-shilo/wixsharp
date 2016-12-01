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

namespace WixSharp
{
    /// <summary>
    /// The unique string identifier of the Wix# project item. The <c>Id</c> is automatically generated 
    /// by Wix# engine during the MSI build unless it is explicitly specified by a project item 
    /// constructor parameter.
    /// <para><c>Id</c> is used to "stamp" every XML element of the WiX source file produced by Wix#.</para>
    /// </summary>
    public partial class Id
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> class and assigns the constructor parameter
        /// specified to <see cref="Id.Value"/> .
        /// </summary>
        /// <param name="value">The string Id value.</param>
        public Id(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The string Id value.
        /// </summary>
        public string Value = "";

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Id id1, Id id2)
        {
            return string.Compare(id1.Value, id2.Value) == 0;
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Id id1, Id id2)
        {
            return string.Compare(id1.Value, id2.Value) != 0;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Id"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator System.String(Id id)
        {   return id.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {

            if (!(obj is Id)) return false;

            return this == (Id)obj;

        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }
}
