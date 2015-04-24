using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LF.Toolkit.Web.Exceptions;

namespace LF.Toolkit.Web.Controllers
{
    public abstract class BasicSecurityController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;
            bool skipAuthorization = actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                            || actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipAuthorization) return;

            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// 设置上下文共享项
        /// </summary>
        /// <param name="itemKey"></param>
        /// <param name="value"></param>
        public virtual void SetContextItem(string itemKey, object value)
        {
            if (!string.IsNullOrEmpty(itemKey) && value != null)
            {
                var items = HttpContext.Items;
                items[itemKey] = value;
            }
        }

        /// <summary>
        /// 获取指定类型的上下文共享项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        public virtual T GetContextItem<T>(string itemKey)
        {
            T obj = default(T);

            if (!string.IsNullOrEmpty(itemKey))
            {
                var items = HttpContext.Items;
                if (items.Contains(itemKey))
                {
                    obj = (T)items[itemKey];
                }
            }

            return obj;
        }
    }
}