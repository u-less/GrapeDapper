using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunDapper.Core;

namespace DapperT.Test
{
    [Table(AutoIncrement = true)]
    public class sys_module
    { 
       /// <summary>
      /// 编号
      /// </summary>
        [PrimaryKey]
        public int ModuleId
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 上级模块
        /// </summary>
        public int ParentId
        {
            get;
            set;
        }
        /// <summary>
        /// 模块手动标识
        /// </summary>
        public string ModuleKey
        {
            get;
            set;
        }
        /// <summary>
        /// 模块类别
        /// </summary>
        public int TypeId
        {
            get;
            set;
        }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon
        {
            get;
            set;
        }
        /// <summary>
        /// 顺序
        /// </summary>
        public int SortIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 关联值
        /// </summary>
        public string ModuleValue
        {
            get;
            set;
        }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsUsable
        {
            get;
            set;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords
        {
            get;
            set;
        }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
