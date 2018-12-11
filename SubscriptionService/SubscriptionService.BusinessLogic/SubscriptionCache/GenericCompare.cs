using System;
using System.Collections.Generic;

namespace SubscriptionService.BusinessLogic.SubscriptionCache
{
    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, Object> expr { get; set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCompare{T}"/> class.
        /// </summary>
        /// <param name="expr">The expr.</param>
        public GenericCompare(Func<T, Object> expr)
        {
            this.expr = expr;
        }
        #endregion

        #region public Boolean Equals(T x, T y)
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public Boolean Equals(T x, T y)
        {
            var first = expr.Invoke(x);
            var sec = expr.Invoke(y);
            if (first != null && first.Equals(sec))
                return true;
            else
                return false;
        }
        #endregion

        #region public Int32 GetHashCode(T obj)
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public Int32 GetHashCode(T obj)
        {
            return expr.Invoke(obj).GetHashCode();
        }
        #endregion
    }
}