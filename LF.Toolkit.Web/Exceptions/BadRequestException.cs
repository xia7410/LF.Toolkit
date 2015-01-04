using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    /// <summary>
    /// 错误请求异常类40000系列
    /// </summary>
    public class BadRequestException : Exception
    {
        public int Status { get; private set; }

        public BadRequestException()
        {

        }

        public BadRequestException(int status)
            : this("", status)
        {
        }

        public BadRequestException(string message, int status)
            : base(message)
        {
            this.Status = status;
        }

    }
}