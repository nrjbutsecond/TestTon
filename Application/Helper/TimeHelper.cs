using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public static class TimeHelper
    {
        public static string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime.ToUniversalTime();

            if (timeSpan.TotalSeconds < 60)
                return $"{(int)timeSpan.TotalSeconds} seconds ago";

            if (timeSpan.TotalMinutes < 60)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }

            if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }

            if (timeSpan.TotalDays < 30)
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }

            if (timeSpan.TotalDays < 365)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);
                if (weeks < 4)
                    return weeks == 1 ? "1 week ago" : $"{weeks} weeks ago";

                int months = (int)(timeSpan.TotalDays / 30);
                return months == 1 ? "1 month ago" : $"{months} months ago";
            }

            int years = (int)(timeSpan.TotalDays / 365);
            return years == 1 ? "1 year ago" : $"{years} years ago";
        }

        public static string FormatDateTime(DateTime dateTime, string format = "dd/MM/yyyy")
        {
            return dateTime.ToString(format);
        }

        public static string GetActivityStatus(DateTime? lastActivity, int onlineThresholdMinutes = 5)
        {
            if (!lastActivity.HasValue)
                return "Never";

            var timeSpan = DateTime.UtcNow - lastActivity.Value.ToUniversalTime();

            if (timeSpan.TotalMinutes <= onlineThresholdMinutes)
                return "Online";

            return GetRelativeTime(lastActivity.Value);
        }

        public static bool IsUserOnline(DateTime? lastActivity, int thresholdMinutes = 5)
        {
            if (!lastActivity.HasValue)
                return false;

            var timeSpan = DateTime.UtcNow - lastActivity.Value.ToUniversalTime();
            return timeSpan.TotalMinutes <= thresholdMinutes;
        }
    }
}
