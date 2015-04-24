using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Misc
{
    public class HttpContextItems
    {
        public static void Set(HttpContextBase context, string itemKey, object value)
        {
            if (context != null && !string.IsNullOrEmpty(itemKey) && value != null)
            {
                context.Items[itemKey] = value;
            }
        }

        public static T GetAs<T>(HttpContextBase context, string itemKey)
        {
            T obj = default(T);

            if (context != null && !string.IsNullOrEmpty(itemKey))
            {
                if (context.Items.Contains(itemKey))
                {
                    obj = (T)context.Items[itemKey];
                }
            }

            return obj;
        }
    }
}