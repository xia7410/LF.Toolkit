using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Misc
{
    public class ActionParameters
    {
        public static void Serialize(ActionExecutingContext filterContext, IDictionary<string, string> paramters)
        {
            if (filterContext == null || paramters == null) return;

            var paramKeys = filterContext.ActionParameters.Keys.ToArray();
            if (paramKeys.Length > 0)
            {
                var paramTypes = filterContext.ActionDescriptor.GetParameters().ToDictionary(k => k.ParameterName, k => k.ParameterType);

                foreach (var key in paramKeys)
                {
                    if (paramters.ContainsKey(key))
                    {
                        string value = paramters[key];
                        if (value != null)
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
                                        throw new ArgumentException(key);
                                    }
                                }

                                filterContext.ActionParameters[key] = b;
                            }
                            //Convert.ChangeType 在转换TimeSpan时候会报错。使用TypeConvert替换
                            else
                            {
                                var converter = TypeDescriptor.GetConverter(nullable);
                                filterContext.ActionParameters[key] = converter.ConvertFrom(value);
                            }
                        }
                    }
                }
            }
        }
    }
}