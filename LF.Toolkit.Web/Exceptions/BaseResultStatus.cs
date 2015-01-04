using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    /// <summary>
    /// 表示返回结果的状态码定义基类，子类要继承只需实现 40101以后定义的错误即可
    /// </summary>
    public class BaseResultStatus
    {
        /// <summary>
        /// 表示请求成功 200
        /// </summary>
        public const int SUCCESS = 200;

        /// <summary>
        /// 表示未认证 401 
        /// </summary>
        public const int UNAUTHORIZED = 401;

        /// <summary>
        /// 表示访问请求被禁止 403 
        /// </summary>
        public const int FORBIDDEN = 403;

        /// <summary>
        /// 表示未定义的错误 499
        /// </summary>
        public const int UNDEFINED = 499;

        /// <summary>
        /// 表示服务器内部错误 500
        /// </summary>
        public const int INTRNAL_SERVER_ERROR = 500;


        #region 400 Series --- 40001 - 40100 （此框架预留的错误码段）参数相关错误码段

        /// <summary>
        /// 表示请求参数缺失 40001
        /// </summary>
        public const int BADREQUEST_40001 = 40001;

        /// <summary>
        /// 表示请求参数错误 40002
        /// </summary>
        public const int BADREQUEST_40002 = 40002;

        /// <summary>
        /// 表示请求参数无效 40003
        /// </summary>
        public const int BADREQUEST_40003 = 40003;

        #endregion

    }
}