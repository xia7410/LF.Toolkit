using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Web.Security
{
    public interface IExceptionHandler
    {
        /// <summary>
        /// 参数错误处理
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="e"></param>
        void OnArgumentException(string argument, Exception e);

        /// <summary>
        /// 参数为空的处理
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="e"></param>
        void OnArgumentNullException(string argument, Exception e);
    }
}
