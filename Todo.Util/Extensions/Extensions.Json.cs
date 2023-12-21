using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Todo.Commons
{
    public static partial class Extensions
    {
        /// <summary>
        /// 转成json对象
        /// </summary>
        /// <param name="json">json字串</param>
        /// <returns></returns>
        public static object? ToJson(this string json)
        {
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Serialize(json);
        }

        public static object? ToJson(this object obj)
        {
            var option = new JsonSerializerOptions();
            option.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            return JsonSerializer.Serialize(obj);
        }
    }
}
