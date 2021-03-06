﻿using System;

using Public.Common.Lib;


namespace Public.Examples.Code
{
    /// <summary>
    /// An example equatable class.
    /// When you implement IEquatable and its Equals() method, you should implement an override of Object.Equals().
    /// And if you override Object.Equals(), you should override Object.GetHashCode() and the equality operators '==' and '!='. If you don't, and you use the '==' operator, you will get the version of the operator that uses the base Object.Equals() method.
    /// 
    /// It shows several options to choose from:
    /// * DEBUG pragma behavior.
    /// 
    /// * Purports to show the "right" way. Simple IEquatable.Equals() method, complex Object.Equals() method. http://www.aaronstannard.com/overriding-equality-in-dotnet/
    /// </summary>
    public class EquatableClass : IEquatable<EquatableClass>
    {
        #region Static

        public static bool operator ==(EquatableClass lhs, EquatableClass rhs)
        {
            if(lhs is null)
            {
                var output = rhs is null;
                return output;
            }
            else
            {
                var output = lhs.Equals(rhs); // Equals(EquatableClass) handles a null right side.
                return output;
            }
        }

        public static bool operator !=(EquatableClass lhs, EquatableClass rhs)
        {
            var output = !(lhs == rhs);
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

            // Null check and compare exact types.
            // We're inside the 'this' instance, so it can't be null!
            // Instances of derived types can have the same A and B property values, and yet are NOT equal!
            if (other is null || !this.GetType().Equals(other.GetType()))
            {
                return false;
            }

#if DEBUG // Allow for line-by-line debugging to see where output changes.
            bool output = true;
            output = output && (this.A.Equals(other.A));
            output = output && (this.B.Equals(other.B));

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
            // Check type to ensure we are not comparing an object of a derived type to an object of the base type.
            // This 'is' operator will return true for derived types since an instance of a derived type is an instance of the base type.
            // The 'as' operator will return a reference to the derived instance as a base type instance, and the properties of the two base-type instances might be the same, but obviously the two objects are not equal since one of them is actually an instance of the derived type!

            var objIsNull = obj == null;

            // Handles non-sealed (inheritable) types.
            // To make sure that this double type-lookup and comparison is only performed once in an inheritance hierarchy, consider implementing an "Equals_Internal()" method that assumes this type check has already passed.
            var objTypeIsThisType = obj ?? obj.GetType().Equals(this.GetType());

            // Note, if class is sealed (non-inheritable), could also test whether the object type is the specific class type.
            var objTypeIsSpecificType = obj ?? obj.GetType().Equals(typeof(EquatableClass));

            // Use short-circuited or-operator to not evaluate second clause if obj is null (and thus avoid null-reference exception).
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            var objAsType = obj as EquatableClass;

            var isEqual = this.Equals(objAsType);
            return isEqual;
        }

        //// Would this work?
        //public bool Equals(TelephoneNumber other)
        //{
        //    if (object.ReferenceEquals(this, other))
        //    {
        //        return true;
        //    }

        //    if (other == null || !other.GetType().Equals(this.GetType()))
        //    {
        //        return false;
        //    }

        //    var output = this.Digits.Equals(other.Digits);
        //    return output;
        //}

        //public override bool Equals(object obj)
        //{
        //    var objAsType = obj as TelephoneNumber;

        //    var output = this.Equals(objAsType);
        //    return output;
        //}

        ///// Would this work given the explicit type check in <see cref="EquatableClass.Equals(EquatableClass)"/>?
        //public override bool Equals(object obj)
        //{
        //    if (obj is EnumerationBase objAsEnumerationBase)
        //    {
        //        var output = this.Equals(objAsEnumerationBase);
        //        return output;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        // This is the expansion of the Visual Studio equals snippet. Note that the links go nowhere!
        //public override bool Equals(object obj)
        //{
        //    //       
        //    // See the full list of guidelines at
        //    //   http://go.microsoft.com/fwlink/?LinkID=85237  // Goes nowhere!
        //    // and also the guidance for operator== at
        //    //   http://go.microsoft.com/fwlink/?LinkId=85238  // Goes nowhere!
        //    //

        //    if (obj == null || !GetType().Equals(obj.GetType()))
        //    {
        //        return false;
        //    }

        //    // TODO: write your implementation of Equals() here
        //    throw new NotImplementedException();
        //    return base.Equals(obj);
        //}
        //
        //// override object.GetHashCode
        //public override int GetHashCode()
        //{
        //    // TODO: write your implementation of GetHashCode() here
        //    throw new NotImplementedException();
        //    return base.GetHashCode();
        //}

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
            if (lhs is null)
            {
                var output = rhs is null;
                return output;
            }
            else
            {
                var output = lhs.Equals(rhs); // Equals() handles a null right side.;
                return output;
            }
        }

        public static bool operator !=(EqualsClassDescendant lhs, EqualsClassDescendant rhs)
        {
            var output = !(lhs == rhs);
            return output;
        }

        #endregion

        #region IEquatable<EqualsClassDescendant> Members

        public bool Equals(EqualsClassDescendant other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            // Compare exact types. Instances of derived types can have the same A and B property values, and yet are NOT equal!
            if (other is null || !this.GetType().Equals(other.GetType()))
            {
                return false;
            }

            if (this.C.Equals(other.C))
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
            if (obj.GetType().Equals(typeof(EqualsClassDescendant)))
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
