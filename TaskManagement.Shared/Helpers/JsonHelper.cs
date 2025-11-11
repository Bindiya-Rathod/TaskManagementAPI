using System.Text.Json;

namespace TaskManagement.Shared.Helpers
{
    /// <summary>
    /// Helper class for JSON serialization and deserialization
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        /// <summary>
        /// Serialize object to JSON string
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON string</returns>
        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        /// <summary>
        /// Deserialize JSON string to object
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="json">JSON string</param>
        /// <returns>Deserialized object</returns>
        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
    }
}
