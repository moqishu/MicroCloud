using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace LIN.MSA.DataAccess
{
    public class BaseRepository<T> where T : class, new()
    {
        private SqlSugarClient db = DbContext.GetDb();

        private SimpleClient<T> CurrentDb
        {
            get
            {
                return new SimpleClient<T>(db);
            }
        }

        /// <summary>
        /// 根据主键获取所有
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual List<T> GetById(dynamic id)
        {
            return CurrentDb.GetById(id);
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(dynamic id)
        {
            return CurrentDb.Delete(id);
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteByIds(dynamic[] ids)
        {
            return CurrentDb.DeleteByIds(ids);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Insert(T obj)
        {
            return CurrentDb.Insert(obj);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Update(T obj)
        {
            return CurrentDb.Update(obj);
        }

    }
}
