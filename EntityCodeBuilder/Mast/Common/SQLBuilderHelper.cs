using System;
using System.Collections.Generic;
using System.Text;
using Mast.DBUtility;
using System.Data;

namespace Mast.Common
{
    public class SQLBuilderHelper
    {
        private static string mssqlPageTemplate = "select * from (select ROW_NUMBER() OVER(order by {0}) AS RowNumber, {1}) as tmp_tbl where RowNumber BETWEEN @pageStart and @pageEnd ";
        private static string mysqlOrderPageTemplate = "{0} order by {1} limit ?offset,?limit";
        private static string mysqlPageTemplate = "{0} limit ?offset,?limit";
        private static string accessPageTemplate = "select * from (select top @limit * from (select top @offset {0} order by id desc) order by id) order by {1}";

        public static string fetchColumns(string strSQL)
        {
            String columns = string.Empty;
            try
            {
                columns = strSQL.Substring(6, strSQL.IndexOf("from") - 6);
            }
            catch (Exception)
            {
                columns = strSQL.Substring(6, strSQL.IndexOf("FROM") - 6);
            }
            return columns;
        }

        public static string fetchPageBody(string strSQL)
        {
            string body = strSQL.Substring(6, strSQL.Length - 6);
            return body;
        }

        public static string fetchWhere(string strSQL)
        {
            int index = strSQL.LastIndexOf("where");
            if (index == -1) return "";

            String where = strSQL.Substring(index, strSQL.Length - index);
            return where;
        }

        public static bool isPage(string strSQL)
        { 
            string strSql = strSQL.ToLower();

            if (AdoHelper.DbType == DatabaseType.ACCESS && strSql.IndexOf("top") == -1)
            {
                return false;
            }

            if (AdoHelper.DbType == DatabaseType.SQLSERVER && strSql.IndexOf("row_number()") == -1)
            {
                return false;
            }

            if(AdoHelper.DbType == DatabaseType.MYSQL && strSql.IndexOf("limit") == -1)
            {
                return false;
            }

            if (AdoHelper.DbType == DatabaseType.ORACLE && strSql.IndexOf("rowid") == -1)
            {
                return false;
            }

            return true;
        }

        public static string builderPageSQL(string strSql, string order, bool desc)
        {
            string columns = fetchColumns(strSql);
            string orderBy = order + (desc ? " desc " : " asc ");
            

            if (AdoHelper.DbType == DatabaseType.SQLSERVER && strSql.IndexOf("row_number()") == -1)
            {
                if (string.IsNullOrEmpty(order))
                {
                    throw new Exception(" SqlException: order field is null, you must support the order field for sqlserver page. ");
                }

                string pageBody = fetchPageBody(strSql);
                strSql = string.Format(mssqlPageTemplate, orderBy, pageBody);
            }

            if (AdoHelper.DbType == DatabaseType.ACCESS && strSql.IndexOf("top") == -1)
            {
                if (string.IsNullOrEmpty(order))
                {
                    throw new Exception(" SqlException: order field is null, you must support the order field for sqlserver page. ");
                }

                //select {0} from (select top @pageSize {1} from (select top @pageSize*@pageIndex {2} from {3} order by {4}) order by id) order by {5}
                string pageBody = fetchPageBody(strSql);
                strSql = string.Format(accessPageTemplate, pageBody, orderBy);
            }

            if (AdoHelper.DbType == DatabaseType.MYSQL)
            {
                if (!string.IsNullOrEmpty(order))
                {
                    strSql = string.Format(mysqlOrderPageTemplate, strSql, orderBy);
                }
                else
                {
                    strSql = string.Format(mysqlPageTemplate, strSql);
                }
            }
            
            return strSql;
        }

        public static string builderCountSQL(string strSQL)
        {
            int index = strSQL.IndexOf("from");
            string strFooter = strSQL.Substring(index, strSQL.Length - index);
            string strText = "select count(*) " + strFooter;

            return strText;
        }

        public static string builderAccessSQL(object entity, string strSql, IDbDataParameter[] parameters)
        {
            if (AdoHelper.DbType != DatabaseType.ACCESS)
            {
                return strSql;
            }

            foreach (IDbDataParameter param in parameters)
            {
                if (param.Value == null) continue;

                string paramName = param.ParameterName;
                string paramValue = param.Value.ToString();
                string type = ReflectionHelper.GetPropertyType(entity, paramName);
                
                if (type == "System.String" || type == "System.DateTime")
                { 
                    paramValue = "'" + paramValue + "'";
                }
                
                strSql = strSql.Replace("@"+paramName, paramValue);
            }

            return strSql;
        }

        public static string builderAccessSQL(string strSql, IDbDataParameter[] parameters)
        {
            if (AdoHelper.DbType != DatabaseType.ACCESS)
            {
                return strSql;
            }

            foreach (IDbDataParameter param in parameters)
            {
                if (param.Value == null) continue;

                string paramName = param.ParameterName;
                string paramValue = param.Value.ToString();

                /*if (type == "System.String" || type == "System.DateTime")
                {
                    paramValue = "'" + paramValue + "'";
                }*/

                strSql = strSql.Replace("@" + paramName, paramValue);
            }

            return strSql;
        }
    }
}
