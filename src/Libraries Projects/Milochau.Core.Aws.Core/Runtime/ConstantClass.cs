using System;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Base class for constant class that holds the value that will be sent to AWS for the static constants.
    /// </summary>
    public class ConstantClass
    {
        protected ConstantClass(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value that needs to be used when send the value to AWS
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                // If obj is null, return false.
                return false;
            }

            // If both are the same instance, return true.
            if (System.Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var objConstantClass = obj as ConstantClass;
            if (Equals(objConstantClass))
            {
                return true;
            }

            if (obj is string objString)
            {
                return Equals(objString);
            }

            // obj is of an incompatible type, return false.
            return false;
        }

        public virtual bool Equals(ConstantClass? obj)
        {
            if (obj is null)
            {
                // If obj is null, return false.
                return false;
            }
            return StringComparer.OrdinalIgnoreCase.Equals(Value, obj.Value);
        }

        protected virtual bool Equals(string value)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(Value, value);
        }

        public static bool operator ==(ConstantClass? a, ConstantClass? b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                // If both are null, or both are the same instance, return true.
                return true;
            }

            if (a is null)
            {
                // If either is null, return false.
                return false;
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static bool operator !=(ConstantClass? a, ConstantClass? b)
        {
            return !(a == b);
        }
    }
}
