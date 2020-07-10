using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LIN.MSA.Infrastructure
{
    public enum ResultEnum
    {
        [Description("操作成功")]
        SUCCESS = 200,

        [Description("操作失败")]
        FAILED = 500,


    }
}
