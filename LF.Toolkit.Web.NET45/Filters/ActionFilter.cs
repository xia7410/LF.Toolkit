using LF.Toolkit.Web.Attributes;
using LF.Toolkit.Web.Metadata;
using LF.Toolkit.Web.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Filters
{
    public class ActionFilter : IActionFilter, IExceptionHandler
    {
        public virtual void OnArgumentException(string argument, Exception e)
        {

        }

        public virtual void OnArgumentNullException(string argument, Exception e)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (httpContext.Items.Contains(HttpContextItemKeys.ACTIONPARAMETERS))
            {
                var parameters = httpContext.Items[HttpContextItemKeys.ACTIONPARAMETERS];
                if (parameters is Dictionary<string, string>)
                {
                    var dict = parameters as Dictionary<string, string>;
                    var paramKeys = filterContext.ActionParameters.Keys.ToArray();

                    if (paramKeys.Length > 0)
                    {
                        var paramTypes = filterContext.ActionDescriptor.GetParameters().ToDictionary(k => k.ParameterName, k => k.ParameterType);

                        foreach (var key in paramKeys)
                        {
                            if (dict.ContainsKey(key))
                            {
                                string value = dict[key];
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
                                                    OnArgumentException(key, new ArgumentException(key));
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
                                    catch (Exception e)
                                    {
                                        OnArgumentException(key, e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is JsonResult)
            {
                var actionDescriptor = filterContext.ActionDescriptor;
                bool skipEncryption = actionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipEncryptionAttribute), true)
                    || actionDescriptor.IsDefined(typeof(SkipEncryptionAttribute), true);

                filterContext.HttpContext.Items.Add(HttpContextItemKeys.SKIPENCRYPTION, skipEncryption);
            }
        }
    }
}