using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Metadata
{
    public class HttpContextItemKeys
    {
        /// <summary>
        /// 跳过加密标识关键字
        /// </summary>
        public const string SKIPENCRYPTION = "SkipEncryption";

        /// <summary>
        /// 加密密钥关标识关键字
        /// </summary>
        public const string ENCRYPTIONKEY = "EncryptionKey";

        /// <summary>
        /// Action参数集合标识关键字
        /// </summary>
        public const string ACTIONPARAMETERS = "ActionParameters";

        /// <summary>
        /// 认证信息缓存标识关键字
        /// </summary>
        public const string AUTHENTICATIONCACHE = "AuthenticationCache";
    }
}