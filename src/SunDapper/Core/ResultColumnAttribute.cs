using System;

namespace SunDapper.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultColumnAttribute" />.
        /// </summary>
        public ResultColumnAttribute()
        {
        }

        /// <summary>
        ///     Constructs a new instance of the <seealso cref="ResultColumnAttribute" />.
        /// </summary>
        /// <param name="name">The name of the DB column.</param>
        public ResultColumnAttribute(string name)
            : base(name)
        {
        }
    }
}
