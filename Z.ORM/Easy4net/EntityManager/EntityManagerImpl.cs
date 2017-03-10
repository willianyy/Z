using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Easy4net.CustomAttributes;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Linq;
using Easy4net.DBUtility;
using Easy4net.Common;
using System.Text.RegularExpressions;

namespace Easy4net.EntityManager
{
    public class EntityManagerImpl : EntityManager
    {
        IDbTransaction transaction = null;

        public void BeginTransaction()
        {
            transaction = DbFactory.CreateDbTransaction();
        }

        public void Commit()
        {
            if (transaction != null)
            {
                if (transaction.Connection != null && transaction.Connection.State != ConnectionState.Closed)
                {
                    transaction.Commit();
                    transaction.Connection.Close();
                }
            }
        }

        public void Rollback()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                if (transaction.Connection != null && transaction.Connection.State != ConnectionState.Closed)
                {
                    transaction.Connection.Close();
                }
            }
        }

        private IDbTransaction GetTransaction()
        {
            if (transaction != null) return transaction;

            return DbFactory.CreateDbTransaction();
        }

        private void Commit(IDbTransaction _transaction)
        {
            if (transaction == null && _transaction != null)
            {
                _transaction.Commit();
                if (_transaction.Connection != null && _transaction.Connection.State != ConnectionState.Closed)
                {
                    _transaction.Connection.Close();
                }
            }
        }

        private void Rollback(IDbTransaction _transaction)
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                if (_transaction.Connection != null && _transaction.Connection.State != ConnectionState.Closed)
                {
                    _transaction.Connection.Close();
                }
            }
        }

        #region 将实体数据保存到数据库
        public int Save<T>(T entity)
        {
            object val = 0;
            try
            {
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.INSERT);

                String strSql = EntityHelper.GetInsertSql(tableInfo);
                strSql += EntityHelper.GetAutoSql();

                IDbDataParameter[] parms = tableInfo.GetParameters();

                if (transaction != null)
                    val = AdoHelper.ExecuteScalar(transaction, CommandType.Text, strSql, parms);
                else
                    val = AdoHelper.ExecuteScalar(AdoHelper.ConnectionString, CommandType.Text, strSql, parms);

                if (Convert.ToInt32(val) > 0 && (AdoHelper.DbType == DatabaseType.MYSQL || AdoHelper.DbType == DatabaseType.SQLSERVER))
                {
                    PropertyInfo propertyInfo = EntityHelper.GetPrimaryKeyPropertyInfo(entity);
                    ReflectionHelper.SetPropertyValue(entity, propertyInfo, val);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 将实体数据修改到数据库
        public int Update<T>(T entity)
        {
            object val = 0;
            try
            {
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.UPDATE);

                String strSql = EntityHelper.GetUpdateSql(tableInfo);

                IDbDataParameter[] parms = tableInfo.GetParameters();

                if (transaction != null)
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSql, parms);
                else
                    val = AdoHelper.ExecuteNonQuery(AdoHelper.ConnectionString, CommandType.Text, strSql, parms);
            }
            catch (Exception e)
            {
                throw e;
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 删除实体对应数据库中的数据
        public int Remove<T>(T entity)
        {
            object val = 0;
            try
            {
                TableInfo tableInfo = EntityHelper.GetTableInfo(entity, DbOperateType.DELETE);

                String strSql = EntityHelper.GetDeleteByIdSql(tableInfo);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = tableInfo.Id.Value;

                if (transaction != null)
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSql, parms);
                else
                    val = AdoHelper.ExecuteNonQuery(AdoHelper.ConnectionString, CommandType.Text, strSql, parms);
            }
            catch (Exception e)
            {
                throw e;
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 根据主键id删除实体对应数据库中的数据
        public int Remove<T>(object id) where T : new()
        {
            object val = 0;
            try
            {
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.DELETE);

                String strSql = EntityHelper.GetDeleteByIdSql(tableInfo);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = id;

                if (transaction != null)
                    val = AdoHelper.ExecuteNonQuery(transaction, CommandType.Text, strSql, parms);
                else
                    val = AdoHelper.ExecuteNonQuery(AdoHelper.ConnectionString, CommandType.Text, strSql, parms);
            }
            catch (Exception e)
            {
                throw e;
            }

            return Convert.ToInt32(val);
        }
        #endregion

        #region 查询实体类对应的表中所有的记录数
        public int FindCount<T>() where T : new()
        {
            int count = 0;
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.COUNT);
                string strSql = EntityHelper.GetFindCountSql(tableInfo);

                count = Convert.ToInt32(AdoHelper.ExecuteScalar(AdoHelper.ConnectionString, CommandType.Text, strSql));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        #endregion

        #region 根据查询条件查询实体类对应的表中的记录数
        public int FindCount<T>(DbCondition condition) where T : new()
        {
            int count = 0;
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.COUNT);
                tableInfo.Columns = condition.Columns;

                string strSql = EntityHelper.GetFindCountSql(tableInfo, condition);

                count = Convert.ToInt32(AdoHelper.ExecuteScalar(AdoHelper.ConnectionString, CommandType.Text, strSql, tableInfo.GetParameters()));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        #endregion

        #region 根据一个查询条件查询实体类对应的表中所有的记录数
        public int FindCount<T>(string propertyName, object propertyValue) where T : new()
        {
            int count = 0;
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.COUNT);

                string strSql = EntityHelper.GetFindCountSql(tableInfo);
                strSql += string.Format(" WHERE {0} = @{1}", propertyName, propertyName);

                ColumnInfo columnInfo = new ColumnInfo();
                columnInfo.Add(propertyName, propertyValue);
                IDbDataParameter[] parameters = DbFactory.CreateDbParameters(1);
                EntityHelper.SetParameters(columnInfo, parameters);

                count = Convert.ToInt32(AdoHelper.ExecuteScalar(AdoHelper.ConnectionString, CommandType.Text, strSql, parameters));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        #endregion

        #region 查询实体对应表的所有数据
        public List<T> FindAll<T>() where T : new()
        {
            IDataReader sdr = null;
            List<T> listArr = new List<T>();
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());

                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.SELECT);
                String strSql = EntityHelper.GetFindAllSql(tableInfo).ToUpper();

                sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSql);
                while (sdr.Read())
                {
                    T entity = new T();
                    foreach (PropertyInfo property in properties)
                    {
                        if (EntityHelper.IsCaseColumn(property, DbOperateType.SELECT)) continue;

                        String name = tableInfo.PropToColumn[property.Name].ToString();

                        String columns = strSql.Substring(0, strSql.IndexOf("FROM"));
                        if (columns.Contains(name.ToUpper()) || columns.Contains("*") || columns.Contains("top"))
                        {
                            ReflectionHelper.SetPropertyValue(entity, property, sdr[name]);
                        }
                    }

                    listArr.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return listArr;
        }
        #endregion

        #region 通过自定义条件查询数据
        public List<T> Find<T>(DbCondition condition) where T : new()
        {
            List<T> listArr = new List<T>();
            IDataReader sdr = null;
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.SELECT);

                String strSql = EntityHelper.GetFindSql(tableInfo, condition);

                tableInfo.Columns = condition.Columns;

                IDbDataParameter[] parameters = tableInfo.GetParameters();

                sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSql, parameters);
                while (sdr.Read())
                {
                    T entity = new T();
                    foreach (PropertyInfo property in properties)
                    {
                        if (EntityHelper.IsCaseColumn(property, DbOperateType.SELECT)) continue;

                        String name = tableInfo.PropToColumn[property.Name].ToString();
                        ReflectionHelper.SetPropertyValue(entity, property, sdr[name]);
                    }

                    listArr.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return listArr;
        }
        #endregion

        #region 通过自定义SQL语句查询数据
        public List<T> FindBySql<T>(string strSql) where T : new()
        {
            List<T> listArr = new List<T>();
            IDataReader sdr = null;
            try
            {
                strSql = strSql.ToUpper();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.SELECT);

                String columns = strSql.Substring(0, strSql.IndexOf("FROM"));

                sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSql);
                while (sdr.Read())
                {
                    T entity = new T();
                    foreach (PropertyInfo property in properties)
                    {
                        if (EntityHelper.IsCaseColumn(property, DbOperateType.SELECT)) continue;

                        String name = tableInfo.PropToColumn[property.Name].ToString();
                        if (columns.Contains(name.ToUpper()) || columns.Contains("*"))
                        {
                            ReflectionHelper.SetPropertyValue(entity, property, sdr[name]);
                        }
                    }

                    listArr.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return listArr;
        }
        #endregion

        #region 通过自定义SQL语句查询数据
        public List<T> FindBySql<T>(string strSql, IDbDataParameter[] parameters) where T : new()
        {
            List<T> listArr = new List<T>();
            IDataReader sdr = null;
            try
            {
                strSql = strSql.ToUpper();
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.SELECT);

                String columns = strSql.Substring(0, strSql.IndexOf("FROM"));

                sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSql, parameters);
                while (sdr.Read())
                {
                    T entity = new T();
                    foreach (PropertyInfo property in properties)
                    {
                        if (EntityHelper.IsCaseColumn(property, DbOperateType.SELECT)) continue;

                        String name = tableInfo.PropToColumn[property.Name].ToString();
                        if (columns.Contains(name.ToUpper()) || columns.Contains("*"))
                        {
                            ReflectionHelper.SetPropertyValue(entity, property, sdr[name]);
                        }
                    }

                    listArr.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return listArr;
        }
        #endregion

        #region 根据一个查询条件查询数据
        public List<T> FindByProperty<T>(string propertyName, object propertyValue) where T : new()
        {
            List<T> listArr = new List<T>();
            IDataReader sdr = null;
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());
                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.SELECT);

                String strSql = EntityHelper.GetFindAllSql(tableInfo);
                if (DatabaseType.ORACLE==AdoHelper.DbType)
                {
                    strSql += string.Format(" WHERE {0} = :{1}", propertyName, propertyName);
                }
                else
                {
                    strSql += string.Format(" WHERE {0} = :{1}", propertyName, propertyName);
                }
                
                strSql = strSql.ToUpper();

                String columns = strSql.Substring(0, strSql.IndexOf("FROM"));

                ColumnInfo columnInfo = new ColumnInfo();
                columnInfo.Add(propertyName, propertyValue);
                IDbDataParameter[] parameters = DbFactory.CreateDbParameters(1);
                EntityHelper.SetParameters(columnInfo, parameters);

                sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSql, parameters);
                while (sdr.Read())
                {
                    T entity = new T();
                    foreach (PropertyInfo property in properties)
                    {
                        if (EntityHelper.IsCaseColumn(property, DbOperateType.SELECT)) continue;

                        String name = tableInfo.PropToColumn[property.Name].ToString();
                        if (columns.Contains(name.ToUpper()) || columns.Contains("*"))
                        {
                            ReflectionHelper.SetPropertyValue(entity, property, sdr[name]);
                        }
                    }

                    listArr.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            return listArr;
        }
        #endregion

        #region 通过主键ID查询数据
        public T FindById<T>(object id) where T : new()
        {
            List<T> list = new List<T>();

            IDataReader sdr = null;
            try
            {
                PropertyInfo[] properties = ReflectionHelper.GetProperties(new T().GetType());

                TableInfo tableInfo = EntityHelper.GetTableInfo(new T(), DbOperateType.SELECT);

                String strSql = EntityHelper.GetFindByIdSql(tableInfo);

                IDbDataParameter[] parms = DbFactory.CreateDbParameters(1);
                parms[0].ParameterName = tableInfo.Id.Key;
                parms[0].Value = id;

                sdr = AdoHelper.ExecuteReader(AdoHelper.ConnectionString, CommandType.Text, strSql, parms);
                while (sdr.Read())
                {
                    T entity = new T();
                    foreach (PropertyInfo property in properties)
                    {
                        if (EntityHelper.IsCaseColumn(property, DbOperateType.SELECT)) continue;

                        String name = tableInfo.PropToColumn[property.Name].ToString();
                        ReflectionHelper.SetPropertyValue(entity, property, sdr[name]);
                    }
                    list.Add(entity);
                }
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

        #region Transaction 注入事物对象属性
        public IDbTransaction Transaction
        {
            get
            {
                return transaction;
            }
            set
            {
                transaction = value;
            }
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
    }
}
