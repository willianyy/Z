using Mast.Common;
using Mast.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mast.Session
{
    public class Session
    {
        private IDbTransaction m_Transaction = null;

        private Session() { }

        public static Session PriviteInstance()
        {
            Session session = new Session();
            return session;
        }
        public static Session GetCurrentSession()
        {
            Session session = SessionFactory.GetSession();
            return session;
        }

        public static Session NewSession()
        {
            Session session = new Session();
            return session;
        }

        public void BeginTransaction()
        {
            m_Transaction = DbFactory.CreateDbTransaction();
        }

        public void Commit()
        {
            if (m_Transaction != null)
            {
                if (m_Transaction.Connection.State != ConnectionState.Closed)
                {
                    m_Transaction.Commit();
                    m_Transaction.Connection.Close();
                }
            }
        }

        public void Rollback()
        {
            if (m_Transaction != null)
            {
                if (m_Transaction.Connection.State != ConnectionState.Closed)
                {
                    m_Transaction.Rollback();
                    m_Transaction.Connection.Close();
                }
            }
        }

        private IDbTransaction GetTransaction()
        {
            if (m_Transaction != null) return m_Transaction;

            return DbFactory.CreateDbTransaction();
        }

        private void Commit(IDbTransaction transaction)
        {
            if (m_Transaction == null && transaction != null)
            {
                if (transaction.Connection.State != ConnectionState.Closed)
                {
                    transaction.Commit();
                    transaction.Connection.Close();
                }
            }
        }

        private void Rollback(IDbTransaction transaction)
        {
            if (transaction != null)
            {
                if (transaction.Connection.State != ConnectionState.Closed)
                {
                    transaction.Rollback();
                    transaction.Connection.Close();
                }
            }
        }

        #region 将实体数据保存到数据库
        public int Insert<T>(T entity)
        {
            if (entity == null) return 0;

            object val = 0;

            IDbTransaction transaction = null;
            //IDbConnection connection = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                //从实体对象的属性配置上获取对应的表信息
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.INSERT, properties);

                //获取SQL语句
                String strSql = EntityHelper.GetInsertSql(tableInfo);

                //获取参数
                IDbDataParameter[] parms = tableInfo.GetParameters();

                //如果是Access数据库，直接根据参数拼接最终的SQL语句
                strSql = SQLBuilderHelper.builderAccessSQL(entity, strSql, parms);

                //Access数据库执行不需要命名参数
                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    //执行Insert命令
                    val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, strSql);

                    //如果是Access数据库，另外执行获取自动生成的ID
                    String autoSql = EntityHelper.GetAutoSql();
                    val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, autoSql);
                }
                else
                {
                    //执行Insert命令
                    val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, strSql, parms);
                }

                //把自动生成的主键ID赋值给返回的对象
                if (AdoHelper.DbType == DatabaseType.SQLSERVER || AdoHelper.DbType == DatabaseType.MYSQL || AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    PropertyInfo propertyInfo = EntityHelper.GetPrimaryKeyPropertyInfo(entity, properties);
                    ReflectionHelper.SetPropertyValue(entity, propertyInfo, val);
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量保存
        public int Insert<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0) return 0;

            object val = 0;

            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                //从实体对象的属性配置上获取对应的表信息
                T firstEntity = entityList[0];
                PropertyInfo[] properties = ReflectionHelper.GetProperties(firstEntity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(firstEntity, DbOperateType.INSERT, properties);

                //获取SQL语句
                String strSQL = EntityHelper.GetInsertSql(tableInfo);
                foreach (T entity in entityList)
                {
                    //从实体对象的属性配置上获取对应的表信息
                    tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.INSERT, properties);

                    //获取参数
                    IDbDataParameter[] parms = tableInfo.GetParameters();

                    //如果是Access数据库，直接根据参数拼接最终的SQL语句
                    strSQL = SQLBuilderHelper.builderAccessSQL(entity, strSQL, parms);

                    //Access数据库执行不需要命名参数
                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        //执行Insert命令
                        val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, strSQL);

                        //如果是Access数据库，另外执行获取自动生成的ID
                        String autoSql = EntityHelper.GetAutoSql();
                        val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, autoSql);
                    }
                    else
                    {
                        //执行Insert命令
                        val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, strSQL, parms);
                    }

                    //把自动生成的主键ID赋值给返回的对象
                    if (AdoHelper.DbType == DatabaseType.SQLSERVER || AdoHelper.DbType == DatabaseType.MYSQL || AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        PropertyInfo propertyInfo = EntityHelper.GetPrimaryKeyPropertyInfo(entity, properties);
                        ReflectionHelper.SetPropertyValue(entity, propertyInfo, val);
                    }

                    Commit(transaction);
                }
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 将实体数据修改到数据库
        public int Update<T>(T entity)
        {
            if (entity == null) return 0;

            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.UPDATE, properties);

                String strSql = EntityHelper.GetUpdateSql(tableInfo);
                IDbDataParameter[] parms = tableInfo.GetParameters();

                strSql = SQLBuilderHelper.builderAccessSQL(entity, strSql, parms);

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSql);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSql, parms);
                }

                Commit(transaction);

            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量更新
        public int Update<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0) return 0;

            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                T firstEntity = entityList[0];
                PropertyInfo[] properties = ReflectionHelper.GetProperties(firstEntity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(firstEntity, DbOperateType.UPDATE, properties);
                String strSQL = EntityHelper.GetUpdateSql(tableInfo);

                foreach (T entity in entityList)
                {
                    TableInfo table = EntityHelper.GetTableInfo(entity, DbOperateType.UPDATE, properties);
                    IDbDataParameter[] parms = table.GetParameters();

                    strSQL = SQLBuilderHelper.builderAccessSQL(entity, strSQL, parms);

                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL);
                    }
                    else
                    {
                        val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL, parms);
                    }
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 将实体数据修改到数据库
        public int ExcuteSQL(string strSQL, ParamMap param)
        {
            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                IDbDataParameter[] parms = param.toDbParameters();
                strSQL = SQLBuilderHelper.builderAccessSQL(strSQL, parms);

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL, parms);
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 删除实体对应数据库中的数据
        public int Delete<T>(T entity)
        {
            if (entity == null) return 0;

            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = tableInfo.Id.Value;

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL, parms);
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量删除
        public int Delete<T>(List<T> entityList)
        {
            if (entityList == null || entityList.Count == 0) return 0;

            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                T firstEntity = entityList[0];
                PropertyInfo[] properties = ReflectionHelper.GetProperties(firstEntity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(firstEntity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                foreach (T entity in entityList)
                {
                    tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                    IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                    parms[0].ParameterName = tableInfo.Id.Key;
                    parms[0].Value = tableInfo.Id.Value;

                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL);
                    }
                    else
                    {
                        val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL, parms);
                    }
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 根据主键id删除实体对应数据库中的数据
        public int Delete<T>(object id) where T : new()
        {
            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = id;

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL);
                }
                else
                {
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL, parms);
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 批量根据主键id删除数据
        public int Delete<T>(object[] ids) where T : new()
        {
            if (ids == null || ids.Length == 0) return 0;

            object val = 0;
            //IDbConnection connection = null;
            IDbTransaction transaction = null;
            try
            {
                //获取数据库连接，如果开启了事务，从事务中获取
                //connection = GetConnection();
                transaction = GetTransaction();

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                String strSQL = EntityHelper.GetDeleteByIdSql(tableInfo);

                foreach (object id in ids)
                {
                    tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE, properties);

                    IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                    parms[0].ParameterName = tableInfo.Id.Key;
                    parms[0].Value = id;

                    if (AdoHelper.DbType == DatabaseType.ACCESS)
                    {
                        val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL);
                    }
                    else
                    {
                        val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSQL, parms);
                    }
                }

                Commit(transaction);
            }
            catch (Exception e)
            {
                Rollback(transaction);
                throw e;
            }
            finally
            {
                /*if (transaction == null)
                {
                    connection.Close();
                    connection.Dispose();
                }*/
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 通过自定义SQL语句查询记录数
        public int Count(string strSQL)
        {
            int count = 0;
            try
            {
                count = Convert.ToInt32(AdoHelper.ExecuteScalar(AdoHelper.ConnectionString, CommandType.Text, strSQL));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        #endregion

        #region 通过自定义SQL语句查询记录数
        public int Count(string strSql, ParamMap param)
        {
            int count = 0;
            try
            {
                strSql = strSql.ToLower();
                String columns = SQLBuilderHelper.fetchColumns(strSql);

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSql = SQLBuilderHelper.builderAccessSQL(strSql, param.toDbParameters());
                }

                count = Convert.ToInt32(AdoHelper.ExecuteScalar(AdoHelper.ConnectionString, CommandType.Text, strSql, param.toDbParameters()));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        #endregion

        #region 通过自定义SQL语句查询数据
        public List<T> Find<T>(string strSql) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                strSql = strSql.ToUpper();
                String columns = SQLBuilderHelper.fetchColumns(strSql);

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);

                sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSql, null);
                list = EntityHelper.toList<T>(sdr, tableInfo, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return list;
        }
        #endregion

        #region 通过自定义SQL语句查询数据
        public List<T> Find<T>(string strSQL, ParamMap param) where T : new()
        {
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                strSQL = strSQL.ToLower();
                String columns = SQLBuilderHelper.fetchColumns(strSQL);

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);
                if (param.IsPage && !SQLBuilderHelper.isPage(strSQL))
                {
                    strSQL = SQLBuilderHelper.builderPageSQL(strSQL, param.OrderFields, param.IsDesc);
                }

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(strSQL, param.toDbParameters());
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL);
                }
                else
                {
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, param.toDbParameters());
                }

                list = EntityHelper.toList<T>(sdr, tableInfo, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return list;
        }
        #endregion

        #region 分页查询返回分页结果
        public PageResult<T> FindPage<T>(string strSQL) where T : new()
        {
            PageResult<T> pageResult = new PageResult<T>();
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                strSQL = strSQL.ToLower();
                String countSQL = SQLBuilderHelper.builderCountSQL(strSQL);
                String columns = SQLBuilderHelper.fetchColumns(strSQL);

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);

                sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, null);

                int count = this.Count(countSQL);
                list = EntityHelper.toList<T>(sdr, tableInfo, properties);

                pageResult.Total = count;
                pageResult.DataList = list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return pageResult;
        }
        #endregion

        #region 分页查询返回分页结果
        public PageResult<T> FindPage<T>(string strSQL, ParamMap param) where T : new()
        {
            PageResult<T> pageResult = new PageResult<T>();
            List<T> list = new List<T>();
            IDataReader sdr = null;
            IDbConnection connection = null;
            try
            {
                connection = GetConnection();
                bool closeConnection = GetWillConnectionState();

                strSQL = strSQL.ToLower();
                String countSQL = SQLBuilderHelper.builderCountSQL(strSQL);
                String columns = SQLBuilderHelper.fetchColumns(strSQL);

                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);
                if (param.IsPage && !SQLBuilderHelper.isPage(strSQL))
                {
                    strSQL = SQLBuilderHelper.builderPageSQL(strSQL, param.OrderFields, param.IsDesc);
                }

                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(strSQL, param.toDbParameters());
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL);
                }
                else
                {
                    sdr = AdoHelper.ExecuteReader(closeConnection, connection, CommandType.Text, strSQL, param.toDbParameters());
                }

                int count = this.Count(countSQL, param);
                list = EntityHelper.toList<T>(sdr, tableInfo, properties);

                pageResult.Total = count;
                pageResult.DataList = list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return pageResult;
        }
        #endregion

        #region 通过主键ID查询数据
        public T Get<T>(object id) where T : new()
        {
            List<T> list = new List<T>();

            IDataReader sdr = null;
            try
            {
                T entity = new T();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(entity.GetType());

                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.SELECT, properties);
                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = id;

                String strSQL = EntityHelper.GetFindByIdSql(tableInfo);
                if (AdoHelper.DbType == DatabaseType.ACCESS)
                {
                    strSQL = SQLBuilderHelper.builderAccessSQL(strSQL, parms);
                    sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSQL);
                }
                else
                {
                    sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSQL, parms);
                }

                list = EntityHelper.toList<T>(sdr, tableInfo, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return list.FirstOrDefault();
        }
        #endregion

        private IDbConnection GetConnection()
        {
            //获取数据库连接，如果开启了事务，从事务中获取
            IDbConnection connection = null;
            if (m_Transaction != null)
            {
                connection = m_Transaction.Connection;
            }
            else
            {
                connection = DbFactory.CreateDbConnection(AdoHelper.ConnectionString);
            }

            return connection;
        }

        private bool GetWillConnectionState()
        {
            return m_Transaction == null;
        }

    }
}
