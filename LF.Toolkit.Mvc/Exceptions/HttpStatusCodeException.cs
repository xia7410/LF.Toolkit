using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace LF.Toolkit.Mvc.Exceptions
{
    /// <summary>
    /// 表示自定义Http状态码异常类
    /// </summary>
    public class HttpStatusCodeException : Exception
    {
        /// <summary>
        /// 获取Http状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// 获取状态描述消息
        /// </summary>
        public string StatusDescription { get; private set; }

        public HttpStatusCodeException(HttpStatusCode statusCode)
            : this(statusCode, null)
        {

        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
    }
}