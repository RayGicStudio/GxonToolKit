using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GxonToolKit.Core.Helpers;

public static class Json
{
    /// <summary>
    /// Turn json into object.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="value">Json string.</param>
    /// <returns>Object.</returns>
    public static async Task<T> ToObjectAsync<T>(string value)
    {
        return await Task.Run(() =>
        {
            return JsonConvert.DeserializeObject<T>(value);
        });
    }

    /// <summary>
    /// Turn json in JArray.
    /// </summary>
    /// <param name="value">Json string.</param>
    /// <returns>JArray.</returns>
    public static async Task<JArray> ToJArrayAsync(string value)
    {
        return await Task.Run(() =>
        {
            return (JArray)JsonConvert.DeserializeObject(value);
        });
    }

    /// <summary>
    /// Turn object into json.
    /// </summary>
    /// <param name="value">Object.</param>
    /// <returns>Json string.</returns>
    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(value);
        });
    }
}
