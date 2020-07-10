using LIN.MSA.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIN.MSA.Infrastructure.Model
{
    /// <summary>
    /// 结果模型
    /// </summary>
    public class ResultInfo<T>
    {
        public string Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public ResultInfo()
        {
            this.Code = ResultEnum.SUCCESS.ToString();
            this.Msg = ResultEnum.FAILED.ToString();
        }
    }
}
