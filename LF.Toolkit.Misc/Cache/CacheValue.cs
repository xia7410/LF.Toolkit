using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Misc.Cache
{
    public class CacheValue
    {
        /// <summary>
        /// 获取或设置值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取或设置值的过期时间
        /// </summary>
        public DateTime? Expiry { get; set; }
    }
}
