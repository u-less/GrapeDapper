using System;

namespace GrapeDapper.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        /// <summary>
        ///     The column name.
        /// </summary>
        /// <returns>
        ///     The column name.
        /// </returns>
        public string Name { get; private set; }


        /// <summary>
        ///     Constructs a new instance of the <seealso cref="KeyAttribute" />.
        /// </summary>
        /// <param name="primaryKey">The name of the primary key column.</param>
        public KeyAttribute(string columnName=null)
        {
            Name = columnName;
        }
    }
}
