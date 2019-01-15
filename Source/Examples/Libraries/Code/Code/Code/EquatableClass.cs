﻿using System;

using Public.Common.Lib;


namespace Public.Examples.Code
{
    /// <summary>
    /// An example equatable class. It shows several options to choose from:
    /// * DEBUG pragma behavior.
    /// </summary>
    public class EquatableClass : IEquatable<EquatableClass>
    {
        #region Static

        public static bool operator ==(EquatableClass lhs, EquatableClass rhs)
        {
            bool output;
            if(lhs is null)
            {
                output = rhs is null;
            }
            else
            {
                output = lhs.Equals(rhs); // Equals(EquatableClass) handles a null right side.
            }

            return output;
        }

        public static bool operator !=(EquatableClass lhs, EquatableClass rhs)
        {
            bool output = !(lhs == rhs);
            return output;
        }

        #endregion

        #region IEquatable<EqualsClass> Members

        public bool Equals(EquatableClass other)
        {
            // Perform the same object test first as this is more likely to be true than null.
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false; // We're inside the 'this' instance, so it can't be null!
            }

            // Compare exact types. Instances of derived types can have the same A and B property values, and yet are NOT equal!
            if(this.GetType() != other.GetType())
            {
                return false;
            }

#if DEBUG // Allow for line-by-line debugging to see where output changes.
            bool output = true;
            output = output && (this.A == other.A);
            output = output && (this.B == other.B);

            return output;
#else
            return
                (this.A == other.A) &&
                (this.B == other.B);
#endif
        }

        #endregion


        public string A { get; set; }
        public string B { get; set; }


        public EquatableClass() { }

        public EquatableClass(string a, string b)
        {
            this.A = a;
            this.B = b;
        }

        public override bool Equals(object obj)
        {
            bool output = false;
            // Check type to ensure we are not comparing an object of a derived type to an object of the base type.
            // This 'is' operator will return true for derived types since an instance of a derived type is an instance of the base type.
            // The 'as' operator will return a reference to the derived instance as a base type instance, and the properties of the two base-type instances might be the same, but obviously the two objects are not equal since one of them is actually an instance of the derived type!
            if (obj.GetType() == typeof(EquatableClass))
            {
                output = this.Equals(obj as EquatableClass);
            }

            return output;
        }

        public override int GetHashCode()
        {
            int output = HashHelper.GetHashCode(this.A, this.B);
            return output;
        }
    }


    public class EqualsClassDescendant : EquatableClass, IEquatable<EqualsClassDescendant>
    {
        #region Static

        public static bool operator ==(EqualsClassDescendant lhs, EqualsClassDescendant rhs)
        {
            bool output;
            if (object.ReferenceEquals(null, lhs))
            {
                output = (object.ReferenceEquals(null, rhs)) ;
            }
            else
            {
                output = lhs.Equals(rhs); // Equals() handles a null right side.;
            }

            return output;
        }

        public static bool operator !=(EqualsClassDescendant lhs, EqualsClassDescendant rhs)
        {
            return !(lhs == rhs);
        }

        #endregion

        #region IEquatable<EqualsClassDescendant> Members

        public bool Equals(EqualsClassDescendant other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            // Compare exact types. Instances of derived types can have the same A and B property values, and yet are NOT equal!
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            if (this.C == other.C)
            {
                return base.Equals((EquatableClass)other); // Problem? Won't the base always return false due to exact type-check in base?
            }
            
            return false;
        }

        #endregion


        public string C { get; set; }


        public EqualsClassDescendant() : base() { }

        public EqualsClassDescendant(string a, string b, string c)
            : base(a, b)
        {
            this.C = c;
        }

        public override bool Equals(object obj)
        {
            bool output = false;
            if (obj.GetType() == typeof(EqualsClassDescendant))
            {
                output = this.Equals(obj as EqualsClassDescendant);
            }

            return output;
        }

        public override int GetHashCode()
        {
            int output = HashHelper.GetHashCode(base.GetHashCode(), this.C);
            return output;
        }
    }
}
