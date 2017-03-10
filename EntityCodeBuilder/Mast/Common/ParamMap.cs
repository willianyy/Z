using System;
using System.Collections.Generic;
using System.Text;
using Mast.Common;
using System.Data;
using Mast.DBUtility;

namespace Mast.Common
{
    public class ParamMap : Map
    {
        private bool isPage;
        private int pageOffset;
        private int pageLimit;
        private string orderFields;
        private bool isDesc = true;

        private ParamMap() { }

        public string OrderFields
        {
            get { return orderFields; }
            set { orderFields = value; }
        }

        public bool IsDesc
        {
            get { return isDesc; }
            set { isDesc = value; }
        }

        public static ParamMap newMap()
        {
            return new ParamMap();
        }

        public bool IsPage
        {
          get 
          {
              return isPage;
          }
        }

        public int PageOffset
        {
            get
            {
                if (this.ContainsKey("pageIndex") && this.ContainsKey("pageSize"))
                {
                    int pageIndex = this.getInt("pageIndex");
                    int pageSize = this.getInt("pageSize");
                    if (pageIndex <= 0) pageIndex = 1;
                    if (pageSize <= 0) pageSize = 1;

                    return (pageIndex - 1) * pageSize;
                }

                return 0;
            }
        }

        public int PageLimit
        {
            get
            {
                if (this.ContainsKey("pageSize"))
                {
                    return this.getInt("pageSize");
                }

                return 0;
            }
        }

        public int getInt(string key) 
        {
            var value = this[key];
            return Convert.ToInt32(value);
        }

        public String getString(string key)
        {
            var value = this[key];
            return Convert.ToString(value);
        }

        public Double toDouble(string key)
        {
            var value = this[key];
            return Convert.ToDouble(value);
        }

        public Int64 toLong(string key)
        {
            var value = this[key];
            return Convert.ToInt64(value); 
        }

        public Decimal toDecimal(string key)
        {
            var value = this[key];
            return Convert.ToDecimal(value);
        }

        public DateTime toDateTime(string key)
        {
            var value = this[key];
            return Convert.ToDateTime(value);
        }

        public void setOrderFields(string orderFields, bool isDesc)
        {
            this.orderFields = orderFields;
            this.isDesc = isDesc;
        }

        
        /// <summary>
        /// 此方法已过时，请使用 setPageParamters方法分页
        /// </summary>
        /// <param name="pageIndex"></param>
        private void setPageIndex(int pageIndex) 
        {
            this["pageIndex"] = pageIndex;
            setPages();
        }

        /// <summary>
        /// 此方法已过时，请使用 setPageParamters方法分页
        /// </summary>
        /// <param name="pageSize"></param>
        private void setPageSize(int pageSize)
        {
            this["pageSize"] = pageSize;
            setPages();
        }

        /// <summary>
        /// 分页参数设置
        /// </summary>
        /// <param name="page">第几页，从0开始</param>
        /// <param name="limit">每页最多显示几条数据</param>
        public void setPageParamters(int page, int limit)
        {
            this["pageIndex"] = page;
            this["pageSize"] = limit;
            setPages();
        }

       private void setPages() 
        {
            if (this.ContainsKey("pageIndex") && this.ContainsKey("pageSize"))
            {
                this.isPage = true;
                if (AdoHelper.DbType == DatabaseType.MYSQL)
                {
                    this["offset"] = this.PageOffset;
                    this["limit"] = this.PageLimit;

                    this.Remove("pageIndex");
                    this.Remove("pageSize");
                }

                 //int start = (pageIndex-1) * pageSize + 1;
                //int end = pageIndex * pageSize;

                if (AdoHelper.DbType == DatabaseType.SQLSERVER)
                {
                    int pageIndex = this.getInt("pageIndex");
                    int pageSize = this.getInt("pageSize");
                    if (pageIndex <= 0) pageIndex = 1;
                    if (pageSize <= 0) pageSize = 1;

                    this["pageStart"] = (pageIndex - 1) * pageSize + 1;
                    this["pageEnd"] = pageIndex * pageSize;

                    this.Remove("pageIndex");
                    this.Remove("pageSize");
                }

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    int pageIndex = this.getInt("pageIndex");
                    int pageSize = this.getInt("pageSize");

                    this["offset"] = pageIndex * pageSize;
                    this["limit"] = pageSize;

                    this.Remove("pageIndex");
                    this.Remove("pageSize");
                }
            }
        }

        public IDbDataParameter[] toDbParameters()
        {
            int i = 0;
            IDbDataParameter[] paramArr = DbFactory.CreateDbParameters(this.Keys.Count);
            foreach(string key in this.Keys) 
            {
                if (!string.IsNullOrEmpty(key.Trim()))
                {
                    object value = this[key];
                    if (value == null) value = DBNull.Value;
                    paramArr[i].ParameterName = key;
                    paramArr[i].Value = value;
                    i++;
                }
            }

            return paramArr;
        }
    }
}
