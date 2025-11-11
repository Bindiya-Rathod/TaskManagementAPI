namespace TaskManagement.Shared.Helpers
{
    /// <summary>
    /// Helper class for DateTime operations
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Get current UTC datetime
        /// </summary>
        public static DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Check if date is in the past
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if date is in the past</returns>
        public static bool IsInPast(DateTime date)
        {
            return date < DateTime.UtcNow;
        }

        /// <summary>
        /// Check if date is in the future
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if date is in the future</returns>
        public static bool IsInFuture(DateTime date)
        {
            return date > DateTime.UtcNow;
        }

        /// <summary>
        /// Format datetime to ISO 8601 string
        /// </summary>
        /// <param name="date">Date to format</param>
        /// <returns>ISO 8601 formatted string</returns>
        public static string ToIso8601String(DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        /// <summary>
        /// Get start of day (midnight)
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>Start of day datetime</returns>
        public static DateTime StartOfDay(DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// Get end of day (23:59:59)
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>End of day datetime</returns>
        public static DateTime EndOfDay(DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }
    }
}
