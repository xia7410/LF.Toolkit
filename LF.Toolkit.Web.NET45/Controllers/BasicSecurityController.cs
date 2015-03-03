using LF.Toolkit.Web.Attributes;
using LF.Toolkit.Web.Metadata;
using LF.Toolkit.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Controllers
{
    public abstract class BasicSecurityController : Controller
    {
        /// <summary>
        /// 设置加密密钥
        /// </summary>
        /// <param name="encryptionKey"></param>
        public void SetEncryptionKey(string encryptionKey)
        {
            if (HttpContext != null && string.IsNullOrEmpty(encryptionKey))
            {
                HttpContext.Items[HttpContextItemKeys.ENCRYPTIONKEY] = encryptionKey;
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;
            bool skipAuthorization = actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                            || actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipAuthorization) return;

            base.OnAuthorization(filterContext);
        }
    }

    public abstract class BasicSecurityController<TCache, TSession> : BasicSecurityController
        where TCache : ICache<TSession>
        where TSession : class , new()
    {
        /// <summary>
        /// 从HttpContext共享项目中获取认证信息缓存
        /// </summary>
        /// <returns></returns>
        public virtual TCache GetAuthenticationCache()
        {
            if (HttpContext != null)
            {
                if (HttpContext.Items.Contains(HttpContextItemKeys.AUTHENTICATIONCACHE))
                {
                    var cache = HttpContext.Items[HttpContextItemKeys.AUTHENTICATIONCACHE];
                    return (TCache)cache;
                }
            }

            return default(TCache);
        }
    }
}