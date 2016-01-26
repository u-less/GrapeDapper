using System;
using GrapeDapper.Core;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GrapeDapper.Core
{
    public class ColumnInfo
    {
       
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsResult { get; set; }
        public PropertyInfo PropInfo { get; private set; }
        public virtual object GetValue(object target)
        {
            return PropInfo.GetValue(target, null);
        }

        public bool ForceToUtc { get; set; }

        public static ColumnInfo FromProperty(PropertyInfo propertyInfo,bool explicitColumn=false)
        {
            var colAttrs = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
            var keyAttrs= propertyInfo.GetCustomAttributes(typeof(KeyAttribute), true);
            if (explicitColumn)
            {
                if (colAttrs.Length == 0&& keyAttrs.Length==0)
                    return null;
            }
            else
            {
                if (propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), true).Length != 0)
                    return null;
            }

            var ci = new ColumnInfo();

            // Read attribute
            if (colAttrs.Length > 0)
            {
                var colattr = (ColumnAttribute)colAttrs[0];

                ci.Name = colattr.Name == null ? propertyInfo.Name : colattr.Name;
                ci.ForceToUtc = colattr.ForceToUtc;
                ci.IsPrimaryKey = false;
                if ((colattr as ResultAttribute) != null)
                    ci.IsResult = true;
            }
            else
            {
                if (keyAttrs.Length > 0)
                {
                    var keyAttr = (KeyAttribute)keyAttrs[0];
                    ci.Name = keyAttr.Name == null ? propertyInfo.Name : keyAttr.Name;
                    ci.IsPrimaryKey = true;
                    ci.PropInfo = propertyInfo;
                    ci.ForceToUtc = false;
                    ci.IsResult = false;
                }
                else
                {
                    ci.Name = propertyInfo.Name;
                    ci.ForceToUtc = false;
                    ci.IsResult = false;
                    ci.IsPrimaryKey = false;
                }
            }
            return ci;
        }
    }
}
