using AspectCore.DynamicProxy;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIN.MSA.DataAccess
{
    /// <summary>
    /// 拦截需要实现接口或者virtual方法
    /// </summary>
    public class TransactionAspect : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            SqlSugarClient db = DbContext.GetDb();
            db.BeginTran();//开启事务

            try
            {
                await next(context);//执行被拦截的方法

                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();//回滚
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}
