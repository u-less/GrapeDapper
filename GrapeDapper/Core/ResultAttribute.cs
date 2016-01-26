using System;

namespace GrapeDapper.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultAttribute : ColumnAttribute
    {
        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultAttribute" />.
        /// </summary>
        public ResultAttribute()
        {
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultAttribute" />.
        /// </summary>
        /// <param name="name">The name of the DB column.</param>
        public ResultAttribute(string name)
            : base(name)
        {
        }
    }
}
