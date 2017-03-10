using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using EntityCodeBuilder.Entity;
using Mast.DBUtility;
using Mast.Session;

namespace WindowsDemo
{
    public class TableHelper
    {
        /// <summary>  
        /// 获取局域网内的所有数据库服务器名称  
        /// </summary>  
        /// <returns>服务器名称数组</returns>  
        public static List<string> GetSqlServerNames()
        {
            DataTable dataSources = SqlClientFactory.Instance.CreateDataSourceEnumerator().GetDataSources();

            DataColumn column = dataSources.Columns["InstanceName"];
            DataColumn column2 = dataSources.Columns["ServerName"];

            DataRowCollection rows = dataSources.Rows;
            List<string> Serverlist = new List<string>();
            string array = string.Empty;
            for (int i = 0; i < rows.Count; i++)
            {
                string str2 = rows[i][column2] as string;
                string str = rows[i][column] as string;
                if (((str == null) || (str.Length == 0)) || ("MSSQLSERVER" == str))
                {
                    array = str2;
                }
                else
                {
                    array = str2 + @"/" + str;
                }

                Serverlist.Add(array);
            }

            Serverlist.Sort();

            return Serverlist;
        }

        /// <summary>  
        /// 查询sql中的非系统库  
        /// </summary>  
        /// <param name="connection"></param>  
        /// <returns></returns>  
        public static List<string> databaseList(string connection)
        {
            List<string> getCataList = new List<string>();
            string cmdStirng = "select name from sys.databases where database_id > 4";
            SqlConnection connect = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(cmdStirng, connect);
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                    IDataReader dr = cmd.ExecuteReader();
                    getCataList.Clear();
                    while (dr.Read())
                    {
                        getCataList.Add(dr["name"].ToString());
                    }
                    dr.Close();
                }

            }
            catch (SqlException e)
            {
                //MessageBox.Show(e.Message);  
            }
            finally
            {
                if (connect != null && connect.State == ConnectionState.Open)
                {
                    connect.Dispose();
                }
            }
            return getCataList;
        }

        public static List<TableName> GetTables()
        {
            if (AdoHelper.DbType == DatabaseType.SQLSERVER)
            {
                return GetMSSQLTables();
            }
            else if (AdoHelper.DbType == DatabaseType.MYSQL)
            {
                return GetMySQLTables();
            }
            else if (AdoHelper.DbType == DatabaseType.ORACLE)
            {
                return GetOracleTables();
            }
            else
            {
                throw new Exception("暂时不支持其它数据库类型");
            }
        }

        /// <summary>  
        /// 获取列名  
        /// </summary>  
        /// <param name="connection"></param>  
        /// <returns></returns>  
        public static List<TableName> GetMSSQLTables()
        {
            SqlConnection connection = (SqlConnection)DbFactory.CreateDbConnection(AdoHelper.ConnectionString);
            List<TableName> tablelist = new List<TableName>();
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    DataTable objTable = connection.GetSchema("Tables");
                    foreach (DataRow row in objTable.Rows)
                    {
                        TableName tb = new TableName();
                        tb.Name = row[2].ToString();
                        tablelist.Add(tb);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Closed)
                {
                    connection.Dispose();
                }
            }

            return tablelist;
        }

        /// <summary>  
        /// 获取列名  
        /// </summary>  
        /// <param name="connection"></param>  
        /// <returns></returns>  
        public static List<TableName> GetMySQLTables()
        {
            String sql = "select TABLE_NAME as name from INFORMATION_SCHEMA.`TABLES` WHERE TABLE_SCHEMA = '" + AdoHelper.DbName + "'";

            Session session = SessionFactory.GetSession();
            List<TableName> tablelist = session.Find<TableName>(sql);

            return tablelist;
        }

        public static List<TableName> GetOracleTables()
        {
            String sql = "SELECT t.TABLE_NAME as NAME,t.* from all_tables t WHERE OWNER='" + AdoHelper.DbUser + "' ORDER BY  t.TABLE_NAME";

            Session session = SessionFactory.GetSession();
            List<TableName> tablelist = session.Find<TableName>(sql);

            return tablelist;
        }

        public static List<TableColumn> GetColumnField(string TableName)
        {
            if (AdoHelper.DbType == DatabaseType.SQLSERVER)
            {
                return GetMSSQLColumnField(TableName);
            }
            else if (AdoHelper.DbType == DatabaseType.MYSQL)
            {
                return GetMySQLColumnField(TableName);
            }
            else if (AdoHelper.DbType == DatabaseType.ORACLE)
            {
                return GetOracleColumnField(TableName);
            }
            else
            {
                throw new Exception("暂时不支持其它数据库类型");
            }
        }

        /// <summary>  
        /// 获取字段  
        /// </summary>  
        /// <param name="connection"></param>  
        /// <param name="TableName"></param>  
        /// <returns></returns>  
        public static List<TableColumn> GetMSSQLColumnField(string TableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT a.name,");
            sb.Append(" b.name as type,");
            sb.Append(" CASE COLUMNPROPERTY(a.id,a.name,'IsIdentity') WHEN 1 THEN '√' ELSE '' END as IsIdentity, ");
            sb.Append(" CASE WHEN EXISTS ( SELECT * FROM sysobjects WHERE xtype='PK' AND name IN ( SELECT name FROM sysindexes WHERE id=a.id AND indid IN ( SELECT indid FROM sysindexkeys ");
            sb.Append(" WHERE id=a.id AND colid IN ( SELECT colid FROM syscolumns WHERE id=a.id AND name=a.name ) ) ) ) THEN '√' ELSE '' END as IsPrimaryKey,");
            sb.Append(" CASE a.isnullable WHEN 1 THEN '√' ELSE '' END as IsNull ");
            sb.Append(" FROM syscolumns a ");
            sb.Append(" LEFT  JOIN systypes      b ON a.xtype=b.xusertype ");
            sb.Append(" INNER JOIN sysobjects    c ON a.id=c.id AND c.xtype='U' AND c.name<>'dtproperties' ");
            sb.Append(" LEFT  JOIN syscomments   d ON a.cdefault=d.id ");
            sb.Append(" WHERE c.name = '").Append(TableName).Append("' ");
            sb.Append(" ORDER BY c.name, a.colorder");

            //使用Mast框架查询数据
            Session session = SessionFactory.GetSession();
            List<TableColumn> list = session.Find<TableColumn>(sb.ToString());
            return list;
        }

        public static List<TableColumn> GetOracleColumnField(string TableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select t.COLUMN_NAME as NAME, ");
            sb.Append(" t.DATA_TYPE as Type, ");
            sb.Append(" (case t.NULLABLE ");
            sb.Append(" when 'Y' THEN ");
            sb.Append(" '√' ");
            sb.Append(" ELSE ");
            sb.Append(" '' ");
            sb.Append(" END) as IsNull, ");
            sb.Append(" (case t.NULLABLE ");
            sb.Append(" when 'Y' THEN ");
            sb.Append(" '' ");
            sb.Append(" ELSE ");
            sb.Append(" '√' ");
            sb.Append(" END) as IsPrimaryKey, ");
            sb.Append(" '' as IsIdentity ");
            sb.Append(" from user_tab_columns t ");
            sb.Append(" inner join user_col_comments c ");
            sb.Append(" on c.table_name = t.table_name ");
            sb.Append(" and c.column_name = t.column_name ");
            sb.Append(" where t.TABLE_NAME = '" + TableName + "' ");


            //使用Mast框架查询数据
            Session session = SessionFactory.GetSession();
            List<TableColumn> list = session.Find<TableColumn>(sb.ToString());
            return list;
        }
        /// <summary>  
        /// 获取字段  
        /// </summary>  
        /// <param name="connection"></param>  
        /// <param name="TableName"></param>  
        /// <returns></returns>  
        public static List<TableColumn> GetMySQLColumnField(string TableName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT COLUMN_NAME as name,");
            sb.Append(" DATA_TYPE as type,");
            sb.Append(" CASE EXTRA WHEN 'auto_increment' THEN '√' ELSE '' END as IsIdentity, ");
            sb.Append(" CASE COLUMN_KEY WHEN 'PRI' THEN '√' ELSE '' END as IsPrimaryKey, ");
            sb.Append(" CASE IS_NULLABLE WHEN 'YES' THEN '√' ELSE '' END as IsNull ");
            sb.Append(" from INFORMATION_SCHEMA.COLUMNS ");
            sb.Append(" Where table_name = '").Append(TableName).Append("' ");
            sb.Append(" AND table_schema = '").Append(AdoHelper.DbName).Append("' ");

            //使用Mast框架查询数据
            Session session = SessionFactory.GetSession();
            List<TableColumn> list = session.Find<TableColumn>(sb.ToString());
            return list;
        }
    }
}
