using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Web.Metadata
{
    public class BaseJsonResult
    {
        /// <summary>
        /// 返回请求的状态码
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 返回是否请求成功标识( "1" 成功 "0" 失败)
        /// </summary>
        public string success { get; set; }

        /// <summary>
        /// 返回请求的数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 返回错误消息
        /// </summary>
        public string message { get; set; }
    }
}
