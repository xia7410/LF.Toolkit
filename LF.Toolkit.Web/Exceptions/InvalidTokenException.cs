using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    /// <summary>
    /// 表示token验证失败异常
    /// </summary>
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message)
            : base(message)
        {

        }
    }
}