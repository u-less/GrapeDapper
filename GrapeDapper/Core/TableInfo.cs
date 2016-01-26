using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace GrapeDapper.Core
{
    public class TableInfo
    {
        /// <summary>
        ///     The database table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     The name of the primary key column of the table
        /// </summary>
        public ColumnInfo PrimaryColumn { get; set; }

        /// <summary>
        ///     True if the primary key column is an auto-incrementing
        /// </summary>
        public bool AutoIncrement { get; set; }

        public List<ColumnInfo> Columns { get; set; }
        private static ConcurrentDictionary<Type, TableInfo> tableDict = new ConcurrentDictionary<Type, TableInfo>();
        /// <summary>
        ///     Creates and populates a TableInfo from the attributes of a POCO
        /// </summary>
        /// <param name="t">The POCO type</param>
        /// <returns>A TableInfo instance</returns>
        public static TableInfo FromType(Type t)
        {
            TableInfo ti;
            if (!tableDict.TryGetValue(t, out ti))
            {
                ti = new TableInfo();
                ti.Columns = new List<ColumnInfo>();
                var a = t.GetCustomAttributes(typeof(TableAttribute), true);
                var tb = a[0] as TableAttribute;
                ti.TableName = a.Length == 0||string.IsNullOrEmpty(tb.TableName) ? t.Name : tb.TableName;
                ti.AutoIncrement = a.Length == 0 ? true : tb.AutoIncrement;
                var props = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (var p in props)
                {
                    var column = ColumnInfo.FromProperty(p);
                    if (column != null)
                    {
                        if (column.IsPrimaryKey) ti.PrimaryColumn = column;
                        ti.Columns.Add(column);
                    }
                }
                if (ti.PrimaryColumn == null)
                {
                    var prop = ti.Columns.FirstOrDefault(p =>
                    {
                        if (p.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (p.Name.Equals(t.Name + "id", StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (p.Name.Equals(t.Name + "_id", StringComparison.OrdinalIgnoreCase))
                            return true;
                        return false;
                    });

                    if (prop != null)
                    {
                        ti.PrimaryColumn = prop;
                        ti.AutoIncrement = true;
                    }
                }
                tableDict.TryAdd(t, ti);
            }
            return ti;
        }
    }
}
