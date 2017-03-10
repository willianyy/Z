// -----------------------------------------------------------------------
// <copyright file="DescartesHelper.cs" company="HuiBing">
//     Copyright HuiBing. All rights reserved.
// </copyright>
// <author>JiangWeiPeng</author>
// <date>2016-10-27</date>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Utilities.Base.Common
{
    /// <summary>
    /// 笛卡尔乘积
    /// </summary>
    public static class DescartesHelper
    {
        /// <summary>
        /// 泛型转换 笛卡尔乘积
        /// 姜玮鹏
        /// <param name="lstSplit">将每个维度的集合的元素视为List＜T＞,多个集合构成List＜List＜T＞＞ lstSplit作为输入</param> 
        /// </summary>
        public static List<List<T>> ConvertDescartesList<T>(this List<List<T>> lstSplit)
        {
            int count = 1;
            lstSplit.ForEach(item => count *= item.Count);
            //count = lstSplit.Aggregate(1, (result, next) => result * next.Count);
            var lstResult = new List<List<T>>();
            for (int i = 0; i < count; ++i)
            {
                var lstTemp = new List<T>();
                int j = 1;
                lstSplit.ForEach(item =>
                {
                    j *= item.Count;
                    lstTemp.Add(item[(i / (count / j)) % item.Count]);
                });
                lstResult.Add(lstTemp);
            }
            return lstResult;
        }
    }
}
