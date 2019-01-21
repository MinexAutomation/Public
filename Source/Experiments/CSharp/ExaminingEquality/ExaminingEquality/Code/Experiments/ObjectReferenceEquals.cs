﻿using System;


namespace ExaminingEquality.Experiments
{
    /// <summary>
    /// Experiments with <see cref="Object.ReferenceEquals(object, object)"/> from most basic to most complex.
    /// </summary>
    public static class ObjectReferenceEquals
    {
        public static void SubMain()
        {
            ObjectReferenceEquals.NullObjectReferenceEqualsNull();
        }

        /// <summary>
        /// Result: True.
        /// Does null <see cref="Object.ReferenceEquals(object, object)"/> null?
        /// Expected: True, because null is a reference (and thus, if it had a type, would be a reference-type) and <see cref="Object.ReferenceEquals(object, object)"/> implements reference-equality.
        /// 
        /// 
        /// </summary>
        private static void NullObjectReferenceEqualsNull()
        {
            var isEqual = Object.Equals(null, null);

            Console.WriteLine($@"Object.ReferenceEquals(null, null): {isEqual}");
        }
    }
}
