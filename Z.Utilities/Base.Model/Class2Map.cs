using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Z.Utilities.Base.Model
{
    public class Class2Map
    {
        /// <summary>
        /// 将对象属性转换为key-value对
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<String, String> ToMap(Object o)
        {
            Dictionary<String, String> map = new Dictionary<string, String>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(o, new Object[] { }).ToString());
                }
            }

            return map;

        }
    }
}
