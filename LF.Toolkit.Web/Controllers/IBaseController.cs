using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Controllers
{
    public interface IBaseController
    {
        /// <summary>
        /// 在验证执行前调用
        /// </summary>
        /// <param name="filterContext"></param>
        void BeginAuthorization(AuthorizationContext filterContext);

        /// <summary>
        /// 在Action执行完成后调用
        /// </summary>
        /// <param name="filterContext"></param>
        void EndActionExecuted(ActionExecutedContext filterContext);

        /// <summary>
        /// 在异常处理完成后调用
        /// </summary>
        /// <param name="filterContext"></param>
        void OnExceptionExecuted(ExceptionContext filterContext);

        /// <summary>
        /// 数据加密处理
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        object Encrypt(JsonResult result);
    }
}
