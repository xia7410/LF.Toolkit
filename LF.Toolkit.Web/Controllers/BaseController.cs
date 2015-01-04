
using LF.Toolkit.Web.Exceptions;
using LF.Toolkit.Web.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace LF.Toolkit.Web.Controllers
{
    /// <summary>
    /// 表示Http POST from类型请求的基本输入输出控制器基类
    /// </summary>
    public abstract class BaseController : Controller, IBaseController
    {
        /// <summary>
        /// 请求参数字典
        /// </summary>
        protected IDictionary<string, string> ActionParameters { get; set; }

        /// <summary>
        /// 在验证执行前调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected virtual void BeginAuthorization(AuthorizationContext filterContext) { }
        /// <summary>
        /// 在Action执行完成后调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected virtual void EndActionExecuted(ActionExecutedContext filterContext) { }
        /// <summary>
        /// 在异常处理完成后调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected virtual void OnExceptionExecuted(ExceptionContext filterContext) { }
        /// <summary>
        /// 数据加密处理
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual object Encrypt(JsonResult result)
        {
            if (result != null)
            {
                return result.Data;
            }

            return null;
        }

        /// <summary>
        /// 在验证时候调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            this.BeginAuthorization(filterContext);
        }

        /// <summary>
        /// 在Action执行前调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.ActionParameters != null)
            {
                var paramKeys = filterContext.ActionParameters.Keys.ToArray();

                if (paramKeys.Length > 0)
                {
                    var paramTypes = filterContext.ActionDescriptor.GetParameters().ToDictionary(k => k.ParameterName, k => k.ParameterType);

                    foreach (var key in paramKeys)
                    {
                        if (this.ActionParameters.ContainsKey(key))
                        {
                            string value = this.ActionParameters[key];
                            if (value != null)
                            {
                                try
                                {
                                    Type nullable = Nullable.GetUnderlyingType(paramTypes[key]);
                                    if (nullable == null)
                                    {
                                        nullable = paramTypes[key];
                                    }
                                    else
                                    {
                                        //如果value为空字符串，则将value的值置空
                                        value = string.IsNullOrEmpty(value.ToString()) ? null : value;
                                    }
                                    //bool 类型特殊处理
                                    if (nullable == typeof(bool))
                                    {
                                        bool b = false;
                                        if (!bool.TryParse(value, out b))
                                        {
                                            if (value == "1")
                                            {
                                                b = true;
                                            }
                                            else if (value == "0")
                                            {
                                                b = false;
                                            }
                                            else
                                            {
                                                throw new BadRequestException(key, BaseResultStatus.BADREQUEST_40002);
                                            }
                                        }

                                        filterContext.ActionParameters[key] = b;
                                    }
                                    //Convert.ChangeType 在转换TimeSpan时候会报错。使用TypeConvert替换
                                    else
                                    {
                                        var converter = TypeDescriptor.GetConverter(nullable);
                                        filterContext.ActionParameters[key] = value == null ? null : converter.ConvertFrom(value);
                                    }
                                }
                                catch
                                {
                                    throw new BadRequestException(key, BaseResultStatus.BADREQUEST_40002);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 在Action执行后调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //此处不处理action抛出的异常，统一由OnException处理
            if (filterContext.Exception == null)
            {
                if (filterContext.Result is JsonResult)
                {
                    JsonResult result = filterContext.Result as JsonResult;
                    object cipherData = Encrypt(result);

                    result.Data = new BaseJsonResult
                    {
                        success = "1",
                        status = BaseResultStatus.SUCCESS,
                        message = "",
                        data = cipherData == null ? "" : cipherData
                    };
                }
            }

            this.EndActionExecuted(filterContext);
        }

        /// <summary>
        /// 在抛出未处理异常时候调用
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;
            var jsonResult = new BaseJsonResult { success = "0", data = "", message = "" };


            if (ex is InvalidTokenException)
            {
                jsonResult.status = BaseResultStatus.UNAUTHORIZED;
            }
            else if (ex is ForbiddenException)
            {
                jsonResult.status = BaseResultStatus.FORBIDDEN;
            }
            else if (ex is BadRequestException)
            {
                BadRequestException bre = ex as BadRequestException;
                jsonResult.status = bre.Status;
                jsonResult.message = bre.Message;
            }
            else if (ex is InternalServerException || ex is InternalStorageException || ex is TargetInvocationException)
            {
                jsonResult.status = BaseResultStatus.INTRNAL_SERVER_ERROR;
            }
            else
            {
                jsonResult.status = BaseResultStatus.UNDEFINED;
            }

            //触发异常监控
            this.OnExceptionExecuted(filterContext);

            filterContext.ExceptionHandled = true;
            filterContext.Result = Json(jsonResult);
        }


        #region IBaseController

        void IBaseController.BeginAuthorization(AuthorizationContext filterContext)
        {
            BeginAuthorization(filterContext);
        }

        void IBaseController.EndActionExecuted(ActionExecutedContext filterContext)
        {
            EndActionExecuted(filterContext);
        }

        void IBaseController.OnExceptionExecuted(ExceptionContext filterContext)
        {
            OnExceptionExecuted(filterContext);
        }

        object IBaseController.Encrypt(JsonResult result)
        {
            return Encrypt(result);
        }

        #endregion
    }
}
