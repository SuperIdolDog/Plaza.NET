using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Utility.Helper
{
    public class DateTimeHelper
    {
        /// <summary>
        /// 将Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp">Unix时间戳（秒）</param>
        /// <returns>DateTime对象</returns>
        public static DateTime UnixTimeStampToDateTime(long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// 将DateTime转换为Unix时间戳
        /// </summary>
        /// <param name="dateTime">要转换的DateTime</param>
        /// <returns>Unix时间戳（秒）</returns>
        public static long DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        /// <summary>
        /// 将字符串转换为DateTime，支持多种格式
        /// </summary>
        /// <param name="dateString">日期字符串</param>
        /// <returns>DateTime对象</returns>
        public static DateTime ParseDateTime(string dateString)
        {
            if (DateTime.TryParse(dateString, out var result))
            {
                return result;
            }

            // 尝试常见格式
            var formats = new[]
            {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss",
            "yyyyMMddHHmmss",
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "yyyyMMdd",
            "dd-MM-yyyy",
            "dd/MM/yyyy",
            "ddMMyyyy",
            "yyyy-MM-ddTHH:mm:ss", // ISO 8601格式
            "yyyy-MM-ddTHH:mm:ss.fff", // 带毫秒的ISO 8601格式
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyyMMddHHmmssfff",
            "yyyy-MM-dd HH:mm:ss.FFFFFFF", // 带更高精度的格式
            "yyyyMMddHHmmssFFFFFFF"
        };

            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            throw new FormatException($"无法解析日期时间字符串: {dateString}");
        }

        /// <summary>
        /// 将DateTime转换为指定格式的字符串
        /// </summary>
        /// <param name="dateTime">DateTime对象</param>
        /// <param name="format">格式字符串</param>
        /// <returns>格式化后的日期时间字符串</returns>
        public static string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 获取当前时间的Unix时间戳
        /// </summary>
        /// <returns>当前时间的Unix时间戳（秒）</returns>
        public static long GetCurrentUnixTimeStamp()
        {
            return DateTimeToUnixTimeStamp(DateTime.UtcNow);
        }

        /// <summary>
        /// 将时间字符串转换为Unix时间戳
        /// </summary>
        /// <param name="dateString">日期字符串</param>
        /// <returns>Unix时间戳（秒）</returns>
        public static long ParseToUnixTimeStamp(string dateString)
        {
            return DateTimeToUnixTimeStamp(ParseDateTime(dateString));
        }

        /// <summary>
        /// 将时间字符串从一种格式转换为另一种格式
        /// </summary>
        /// <param name="dateString">原始日期字符串</param>
        /// <param name="sourceFormat">原始格式</param>
        /// <param name="targetFormat">目标格式</param>
        /// <returns>转换后的日期字符串</returns>
        public static string ConvertDateTimeFormat(string dateString, string sourceFormat, string targetFormat)
        {
            var dateTime = DateTime.ParseExact(dateString, sourceFormat, CultureInfo.InvariantCulture);
            return dateTime.ToString(targetFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 获取两个日期之间的工作日天数（不包括周末）
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>工作日天数</returns>
        public static int GetWorkingDays(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("结束日期不能早于开始日期");
            }

            int totalDays = (int)(endDate - startDate).TotalDays + 1;
            int workingDays = 0;

            for (int i = 0; i < totalDays; i++)
            {
                DateTime currentDate = startDate.AddDays(i);
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        /// <summary>
        /// 获取当前时间在中国时区（UTC+8）的表示
        /// </summary>
        /// <returns>中国时区的当前时间</returns>
        public static DateTime GetChinaTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
        }

        /// <summary>
        /// 将时间转换为中国时区时间
        /// </summary>
        /// <param name="dateTime">要转换的时间</param>
        /// <returns>中国时区的时间</returns>
        public static DateTime ConvertToChinaTime(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            }
            else if (dateTime.Kind == DateTimeKind.Local)
            {
                return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            }
            else
            {
                // 假设未指定时区的时间是UTC时间
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            }
        }
    }
}
