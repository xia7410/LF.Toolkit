using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Metadata
{
    public class ExceptionResult
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string error_code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error { get; set; }
    }
}