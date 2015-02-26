using System;

namespace LF.Toolkit.Util
{
    public class Timestamp
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeMillis(DateTime dt)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(dt - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimeMillis()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(DateTime.Now - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 将时间戳转换为日期
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        public static DateTime ParseTimeMillis(long millis)
        {
            TimeSpan ts = new TimeSpan(millis * 10000);
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            return startTime.Add(ts);
        }
    }
}
