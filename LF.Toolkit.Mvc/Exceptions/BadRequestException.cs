using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace LF.Toolkit.Mvc.Exceptions
{
    /// <summary>
    /// 表示自定义的 400 http状态码的异常类
    /// </summary>
    public class BadRequestException : Exception
    {
        /// <summary>
        /// 获取自定义的错误码
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// 获取错误消息描述
        /// </summary>
        public string Error { get; private set; }

        public BadRequestException(string errorCode)
            : this(errorCode, null)
        {

        }

        public BadRequestException(string errorCode, string error)
        {
            ErrorCode = errorCode;
            Error = error;
        }
    }
}