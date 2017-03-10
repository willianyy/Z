using Mast.DBUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mast.Common
{
    public class DbKeywords
    {
        private static Dictionary<string, string> m_MySQL = new Dictionary<string, string>();
        private static Dictionary<string, string> m_MSSQL = new Dictionary<string, string>();

        private static void InitMySQL()
        {
            if (m_MySQL.Count == 0)
            {
                m_MySQL.Add("order", "`order`");
                m_MySQL.Add("desc", "`desc`");
                m_MySQL.Add("key", "`key`");
            }
        }

        private static void InitMSSQL()
        {
            if (m_MSSQL.Count == 0)
            {
                m_MSSQL.Add("order", "[order]");
                m_MSSQL.Add("desc", "[desc]");
                m_MSSQL.Add("key", "[key]");
                m_MSSQL.Add("text", "[text]");
            }
        }

        public static string FormatColumnName(string colounName)
        {
            InitMySQL();
            InitMSSQL();

            string colName = colounName.ToLower();
            if (AdoHelper.DbType == DatabaseType.MYSQL && m_MySQL.ContainsKey(colName))
            {
                return m_MySQL[colName];
            }

            if (AdoHelper.DbType == DatabaseType.SQLSERVER && m_MSSQL.ContainsKey(colName))
            {
                return m_MSSQL[colName];
            }

            return colounName;
        }

    }
}
