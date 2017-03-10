//******************************************************** 
//单元描述： XXXXXXXXXXXXXXX
//编辑内容：
//2016/2/4 15:05:14 新增  刘挺  创建文件
//******************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Z.Utilities
{
    /// <summary>
    /// 服务参数类（实现IComparable接口）
    /// </summary>
    public class HBParam : IComparable
    {
        #region
        /// <summary>
        /// 声明name私有变量，为Name属性提供操作数据
        /// </summary>
        private string name;

        /// <summary>
        /// 声明value私有变量，为value属性提供操作数据,其中object类型主要是为存储各种类型数据而设置
        /// </summary>
        private object value;

        /// <summary>
        /// 参数名：Name，只读属性
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 参数值：Value，只读属性
        /// </summary>
        public string Value
        {
            get
            {
                if (value is Array)
                    return ConvertArrayToString(value as Array);
                else
                    return value.ToString();
            }
        }

        /// <summary>
        /// 构造函数初始化参数名和参数值
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        protected HBParam(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }

        /// <summary>
        /// 创建参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public static HBParam Create(string name, object value)
        {
            return new HBParam(name, value);
        }

        /// <summary>
        /// 判断参数是否有效
        /// </summary>
        /// <param name="obj">参数对象类型</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (!(obj is HBParam))
                return -1;
            if (this.name == null)
            {
                return -1;
            }
            return this.name.CompareTo((obj as HBParam).name);
        }

        /// <summary>
        /// 将数组转为字符串
        /// </summary>
        /// <param name="a">数组对象</param>
        /// <returns></returns>
        private static string ConvertArrayToString(Array a)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0)
                    builder.Append(",");

                builder.Append(a.GetValue(i).ToString());
            }

            return builder.ToString();
        }

        #endregion
    }
}
