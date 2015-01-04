using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    /// <summary>
    /// 表示接口禁止访问异常
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message)
            : base(message)
        {

        }
    }
}