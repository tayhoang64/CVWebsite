using Newtonsoft.Json;
using WebCV.Models;

namespace WebCV.Helpers
{
    public class JsonHelper
    {
        /// <summary>
        /// Serialize an object to JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representing the object.</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Deserialize a JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Deserialize a JSON string to an object of the specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The type of object to deserialize to.</param>
        /// <returns>The deserialized object.</returns>
        public static object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static object ToJson<T>(object data)
        {
            string json = JsonHelper.Serialize(data);
            T obj = JsonHelper.Deserialize<T>(json);
            return obj;
        }
    }
}
