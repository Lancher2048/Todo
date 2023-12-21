using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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
        public static string ToJson(this object obj)
        {
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.AllowTrailingCommas = true;

            options.Converters.Add(new DateTimeJsonConverter());
            options.Converters.Add(new DateTimeNullableConverter());

            options.Converters.Add(new BooleanJsonConverter());
            options.PropertyNameCaseInsensitive = true;

            return JsonSerializer.Serialize(obj, options);
        }
        /// <summary>
        /// JSON字符串序列化成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.AllowTrailingCommas = true;
            //设置时间格式
            options.Converters.Add(new DateTimeJsonConverter());
            options.Converters.Add(new DateTimeNullableConverter());
            //设置bool获取格式
            options.Converters.Add(new BooleanJsonConverter());
            options.PropertyNameCaseInsensitive = true;                     //忽略大小写
            return string.IsNullOrEmpty(json) ? default(T) : JsonSerializer.Deserialize<T>(json, options);
        }
        /// <summary>
        /// JSON字符串序列化成集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string json)
        {
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.AllowTrailingCommas = true;
            //设置时间格式
            options.Converters.Add(new DateTimeJsonConverter());
            options.Converters.Add(new DateTimeNullableConverter());
            //设置bool获取格式
            options.Converters.Add(new BooleanJsonConverter());
            options.PropertyNameCaseInsensitive = true;                     //忽略大小写
            return json == null ? null : JsonSerializer.Deserialize<List<T>>(json, options);
        }
        /// <summary>
        /// JSON字符串序列化成DataTable
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static DataTable ToTable(this string json)
        {
            return json == null ? null : JsonSerializer.Deserialize<DataTable>(json);
        }
    }
}
