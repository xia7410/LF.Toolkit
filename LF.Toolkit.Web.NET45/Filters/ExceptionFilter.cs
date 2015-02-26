using LF.Toolkit.Web.Exceptions;
using LF.Toolkit.Web.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            ActionResult result = null;

            if (filterContext.Exception is BadRequestException)
            {
                var ex = filterContext.Exception as BadRequestException;
                var json = new JsonResult
                {
                    Data = new ExceptionResult
                    {
                        error_code = ex.SubStatusCode,
                        error = ex.StatusDescription
                    }
                };
                //若访问方式为HTTPGET则允许GET方式获取JSON错误信息字符串
                if (filterContext.HttpContext.Request.HttpMethod == "GET")
                {
                    json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                }

                result = json;
            }
            else if (filterContext.Exception is HttpStatusCodeException)
            {
                var ex = filterContext.Exception as HttpStatusCodeException;
                result = new HttpStatusCodeResult(ex.StatusCode, "");
            }
            else
            {
                result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            filterContext.Result = result;
        }
    }
}