using System;
using System.Collections.Generic;
using System.Text;

namespace Mast.Common
{
    public class PageResult<T>
    {
        /// <summary>
        /// 分页查询中总记录数
        /// </summary>
        public int Total {get; set;}

        /// <summary>
        /// 分页查询中结果集合
        /// </summary>
        public List<T> DataList {get; set;}
    }
}
