
namespace WixSharp
{
    /// <summary>
    /// 
    /// </summary>
    public class StringEnum<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringEnum{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected StringEnum(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The value
        /// </summary>
        protected string Value = "";

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(StringEnum<T> obj1, StringEnum<T> obj2)
        {
            if ((object)obj1 != null && (object)obj2 != null)
                return string.Compare(obj1.Value, obj2.Value) == 0;
            else if ((object)obj1 == null && (object)obj2 == null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(StringEnum<T> obj1, StringEnum<T> obj2)
        {
            if ((object)obj1 != null && (object)obj2 != null)
                return string.Compare(obj1.Value, obj2.Value) != 0;
            else if ((object)obj1 == null && (object)obj2 == null)
                return false;
            else 
                return true;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Id"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="obj">The identifier.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator System.String(StringEnum<T> obj)
        {
            return obj.Value;
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

            if (!(obj is StringEnum<T>))
            {
                if (obj is string)
                    return (obj as string) == this;
                else
                    return false;
            }
            return this == (StringEnum<T>)obj;

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