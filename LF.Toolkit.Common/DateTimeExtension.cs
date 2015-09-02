using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Common
{
    public class DateTimeExtension
    {
        /// <summary>
        /// 获取Sql数据库支持的最小时间点
        /// </summary>
        public static readonly DateTime MinSqlDateTime;

        /// <summary>
        /// 获取Sql数据库支持的最大时间点
        /// </summary>
        public static readonly DateTime MaxSqlDateTime;

        /// <summary>
        /// 获取Unix新纪元开始日期
        /// </summary>
        public static readonly DateTime UnixEpoch;

        static DateTimeExtension()
        {
            MinSqlDateTime = SqlDateTime.MinValue.Value;
            MaxSqlDateTime = SqlDateTime.MaxValue.Value;
            UnixEpoch = new DateTime(1970, 1, 1);
        }

        /// <summary>
        /// 获取当前时间对应的时间戳
        /// </summary>
        public static long CurrentTimestamp { get { return (long)(DateTime.Now - UnixEpoch).TotalMilliseconds; } }

        /// <summary>
        /// 转换指定时间的时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertToTimestamp(DateTime time)
        {
            if (time < UnixEpoch) throw new ArgumentException("time");

            return (long)(time - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// 转换时间戳为日期
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            if (timestamp < 0) throw new ArgumentException("timestamp");

            return UnixEpoch.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// 获取当前日期的星期的开始日期和结束日期
        /// </summary>
        /// <param name="weekStart"></param>
        /// <param name="weekEnd"></param>
        public static void GetCurrentWeekRange(out DateTime weekStart, out DateTime weekEnd)
        {
            GetWeekRange(DateTime.Now, out weekStart, out weekEnd);
        }

        /// <summary>
        /// 获取指定日期的星期的开始日期和结束日期
        /// </summary>
        /// <param name="weekday"></param>
        /// <param name="weekStart"></param>
        /// <param name="weekEnd"></param>
        public static void GetWeekRange(DateTime weekday, out DateTime weekStart, out DateTime weekEnd)
        {
            int range = (int)weekday.DayOfWeek;
            if (range == 0)
            {
                weekStart = weekday.AddDays(-6).Date;
            }
            else
            {
                weekStart = weekday.AddDays((range - 1) * -1).Date;
            }
            weekEnd = weekStart.AddDays(6).Date;
        }

        /// <summary>
        /// 获取当前日期所在月份的开始日期和结束日期
        /// </summary>
        /// <param name="weekStart"></param>
        /// <param name="weekEnd"></param>
        public static void GetCurrentMonthRange(out DateTime weekStart, out DateTime weekEnd)
        {
            GetMonthRange(DateTime.Now, out weekStart, out weekEnd);
        }

        /// <summary>
        /// 获取指定日期所在月份的开始日期和结束日期
        /// </summary>
        /// <param name="monthday"></param>
        /// <param name="monthStart"></param>
        /// <param name="monthEnd"></param>
        public static void GetMonthRange(DateTime monthday, out DateTime monthStart, out DateTime monthEnd)
        {
            monthStart = monthday.AddDays(1 - monthday.Day);
            monthEnd = monthStart.AddMonths(1).AddDays(-1);
        }

    }
}
