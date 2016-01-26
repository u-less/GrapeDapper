using System;

namespace GrapeDapper.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        ///     The table nane of the database that this entity maps to.
        /// </summary>
        /// <returns>
        ///     The table nane of the database that this entity maps to.
        /// </returns>
        public string TableName { get; private set; }

        public bool ExplicitColumns { get; private set; }
        /// <summary>
        ///     A flag which specifies if the primary key is auto incrementing.
        /// </summary>
        /// <returns>
        ///     True if the primary key is auto incrementing; else, False.
        /// </returns>
        public bool AutoIncrement { get; set; }
        /// <summary>
        ///     Constructs a new instance of the <seealso cref="TableAttribute" />.
        /// </summary>
        /// <param name="tableName">The table nane of the database that this entity maps to.</param>
        public TableAttribute(string tableName = null, bool explicitColumns = false, bool autoIncrement=true)
        {
            TableName = tableName;
            ExplicitColumns = explicitColumns;
            AutoIncrement = autoIncrement;
        }
    }
}
