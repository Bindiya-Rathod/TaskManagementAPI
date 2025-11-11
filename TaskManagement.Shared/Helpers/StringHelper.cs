using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Shared.Helpers
{
    /// <summary>
    /// Helper class for string operations
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Check if string is null or whitespace
        /// </summary>
        public static bool IsNullOrWhiteSpace(string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Truncate string to maximum length
        /// </summary>
        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Generate a unique identifier
        /// </summary>
        public static string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
