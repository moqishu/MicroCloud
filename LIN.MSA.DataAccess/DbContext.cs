using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LIN.MSA.DataAccess
{
    public class DbContext
    {
        // 数据连接池
        private static Dictionary<string, SqlSugarClient> dbPool = new Dictionary<string, SqlSugarClient>();

        public static string connectStr = "";
        public static DbType dbType = DbType.MySql;

        public static SqlSugarClient GetDb(string conn, DbType type)
        {
            connectStr = conn;
            dbType = type;
            var key = GetDbKey();
            return GetDb(key);
        }

        public static SqlSugarClient GetDb()
        {
            var key = GetDbKey();
            return GetDb(key);
        }

        private static string GetDbKey()
        {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }

        private static SqlSugarClient GetDb(string key)
        {
            if (dbPool.ContainsKey(key))
            {
                return dbPool[key];
            }
            else
            {
                var db = BuildSqlClient();
                dbPool.Add(key, db);
                return db;
            }
        }

        private static SqlSugarClient BuildSqlClient()
        {
            SqlSugarClient db = new SqlSugarClient(
                        new ConnectionConfig()
                        {
                            ConnectionString = connectStr,
                            DbType = dbType,//设置数据库类型
                            IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                            InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
                        });

            // 配置项管理是否打印SQL语句
            if (true)
            {
                //用来打印Sql方便你调式    
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql + "\r\n" +
                    db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    Console.WriteLine();
                };
            }

            return db;
        } 


    }
}
