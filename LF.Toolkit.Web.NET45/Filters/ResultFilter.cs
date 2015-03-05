using LF.Toolkit.Web.Metadata;
using LF.Toolkit.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Filters
{
    public class ResultFilter : IResultFilter, IResultHandler
    {
        public virtual void Encrypt(string encryptionKey, ResultExecutingContext filterContext)
        {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            if (filterContext.Result is JsonResult)
            {
                var result = filterContext.Result as JsonResult;
                if (filterContext.HttpContext.Request.HttpMethod == "GET")
                {
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                }

                if (result.Data != null)
                {
                    bool skipEncryption = true;
                    if (httpContext.Items.Contains(HttpContextItemKeys.SKIPENCRYPTION))
                    {
                        skipEncryption = (bool)httpContext.Items[HttpContextItemKeys.SKIPENCRYPTION];
                        if (!skipEncryption)
                        {
                            if (httpContext.Items.Contains(HttpContextItemKeys.ENCRYPTIONKEY))
                            {
                                string encryptionKey = (string)httpContext.Items[HttpContextItemKeys.ENCRYPTIONKEY];
                                if (!string.IsNullOrEmpty(encryptionKey))
                                {
                                    Encrypt(encryptionKey, filterContext);
                                }
                            }
                        }
                    }
                }

                if (result.Data == null)
                {
                    result.Data = "";
                }

                filterContext.Result = result;
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }
    }
}