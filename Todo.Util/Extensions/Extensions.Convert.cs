using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Todo.Commons
{
	public static partial class Extensions
    {
        /// <summary>
        /// 将集合转换为数据集
        /// </summary>
        /// <typeparam name="T">转换的元素类型。</typeparam>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToDataSet<T>(this IEnumerable<T> list, bool generic = true)
        {
            return ListToDataSet(list, generic);
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToDataSet(this IEnumerable list, bool generic = true)
        {
            return ListToDataSet(list, generic);
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <typeparam name="T">转换的元素类型。</typeparam>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToDataSet<T>(this IEnumerable list, bool generic = true)
        {
            return ListToDataSet(list, typeof(T), generic);
        }

        /// <summary>
        /// 将实例转换为集合数据集。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="o">实例。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToListSet<T>(this T o, bool generic = true)
        {
            if (o is IEnumerable)
            {
                return ListToDataSet(o as IEnumerable, generic);
            }
            else
            {
                return ListToDataSet(new T[] { o }, generic);
            }
        }

        /// <summary>
        /// 将可序列化实例转换为XmlDocument。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="o">实例。</param>
        /// <returns>XmlDocument。</returns>
        public static XmlDocument ToXmlDocument<T>(this T o)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.InnerXml = o.ToListSet().GetXml();
            return xmlDocument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertibleValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ChangeType(this object convertibleValue, Type type)
        {
            if (null == convertibleValue) return null;

            try
            {
                if (type == typeof(Guid) || type == typeof(Guid?))
                {
                    string value = convertibleValue.ToString();
                    if (value == "") return null;
                    return Guid.Parse(value);
                }

                if (!type.IsGenericType) return Convert.ChangeType(convertibleValue, type);
                if (type.ToString() == "System.Nullable`1[System.Boolean]" || type.ToString() == "System.Boolean")
                {
                    if (convertibleValue.ToString() == "0")
                        return false;
                    return true;
                }
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(type));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <param name="list">集合。</param>
        /// <param name="t">转换的元素类型。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>转换后的数据集。</returns>
        private static DataSet ListToDataSet(IEnumerable list, Type t, bool generic)
        {
            DataSet ds = new DataSet("Data");
            if (t == null)
            {
                if (list != null)
                {
                    foreach (var i in list)
                    {
                        if (i == null)
                        {
                            continue;
                        }
                        t = i.GetType();
                        break;
                    }
                }
                if (t == null)
                {
                    return ds;
                }
            }
            ds.Tables.Add(t.Name);
            //如果集合中元素为DataSet扩展涉及到的基本类型时，进行特殊转换。
            if (t.IsValueType || t == typeof(string))
            {
                ds.Tables[0].TableName = "Info";
                ds.Tables[0].Columns.Add(t.Name);
                if (list != null)
                {
                    foreach (var i in list)
                    {
                        DataRow addRow = ds.Tables[0].NewRow();
                        addRow[t.Name] = i;
                        ds.Tables[0].Rows.Add(addRow);
                    }
                }
                return ds;
            }
            //处理模型的字段和属性。
            var fields = t.GetFields();
            var properties = t.GetProperties();
            foreach (var j in fields)
            {
                if (!ds.Tables[0].Columns.Contains(j.Name))
                {
                    if (generic)
                    {
                        ds.Tables[0].Columns.Add(j.Name, j.FieldType);
                    }
                    else
                    {
                        ds.Tables[0].Columns.Add(j.Name);
                    }
                }
            }
            foreach (var j in properties)
            {
                if (!ds.Tables[0].Columns.Contains(j.Name))
                {
                    if (generic)
                    {
                        ds.Tables[0].Columns.Add(j.Name, j.PropertyType);
                    }
                    else
                    {
                        ds.Tables[0].Columns.Add(j.Name);
                    }
                }
            }
            if (list == null)
            {
                return ds;
            }
            //读取list中元素的值。
            foreach (var i in list)
            {
                if (i == null)
                {
                    continue;
                }
                DataRow addRow = ds.Tables[0].NewRow();
                foreach (var j in fields)
                {
                    MemberExpression field = Expression.Field(Expression.Constant(i), j.Name);
                    LambdaExpression lambda = Expression.Lambda(field, new ParameterExpression[] { });
                    Delegate func = lambda.Compile();
                    object value = func.DynamicInvoke();
                    addRow[j.Name] = value;
                }
                foreach (var j in properties)
                {
                    MemberExpression property = Expression.Property(Expression.Constant(i), j);
                    LambdaExpression lambda = Expression.Lambda(property, new ParameterExpression[] { });
                    Delegate func = lambda.Compile();
                    object value = func.DynamicInvoke();
                    addRow[j.Name] = value;
                }
                ds.Tables[0].Rows.Add(addRow);
            }
            return ds;
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <typeparam name="T">转换的元素类型。</typeparam>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        private static DataSet ListToDataSet<T>(IEnumerable<T> list, bool generic)
        {
            return ListToDataSet(list, typeof(T), generic);
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否转换为字符串形式。</param>
        /// <returns>转换后的数据集。</returns>
        private static DataSet ListToDataSet(IEnumerable list, bool generic)
        {
            return ListToDataSet(list, null, generic);
        }

        /// <summary>
        /// 获取DataSet第一表，第一行，第一列的值。
        /// </summary>
        /// <param name="ds">DataSet数据集。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataSet ds)
        {
            if (
                ds == null
                || ds.Tables.Count == 0
                )
            {
                return string.Empty;
            }
            else
            {
                return ds.Tables[0].GetData();
            }
        }

        /// <summary>
        /// 获取DataTable第一行，第一列的值。
        /// </summary>
        /// <param name="dt">DataTable数据集表。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataTable dt)
        {
            if (
                dt.Columns.Count == 0
                || dt.Rows.Count == 0
                )
            {
                return string.Empty;
            }
            else
            {
                return dt.Rows[0][0];
            }
        }

        /// <summary>
        /// 获取DataSet第一个匹配columnName的值。
        /// </summary>
        /// <param name="ds">数据集。</param>
        /// <param name="columnName">列名。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataSet ds, string columnName)
        {
            if (
                ds == null
                || ds.Tables.Count == 0
                )
            {
                return string.Empty;
            }
            foreach (DataTable dt in ds.Tables)
            {
                object o = dt.GetData(columnName);
                if (!string.IsNullOrEmpty(o.ToString()))
                {
                    return o;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取DataTable第一个匹配columnName的值。
        /// </summary>
        /// <param name="dt">数据表。</param>
        /// <param name="columnName">列名。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataTable dt, string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return GetData(dt);
            }
            if (
                dt.Columns.Count == 0
                || dt.Columns.IndexOf(columnName) == -1
                || dt.Rows.Count == 0
                )
            {
                return string.Empty;
            }
            return dt.Rows[0][columnName];
        }

        /// <summary>
        /// 将object转换为string类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>string。</returns>
        public static string ToString(this object o, string t)
        {
            string info = string.Empty;
            if (o == null)
            {
                info = t;
            }
            else
            {
                info = o.ToString();
            }
            return info;
        }

        /// <summary>
        /// 将DateTime?转换为string类型信息。
        /// </summary>
        /// <param name="o">DateTime?。</param>
        /// <param name="format">标准或自定义日期和时间格式的字符串。</param>
        /// <param name="t">默认值。</param>
        /// <returns>string。</returns>
        public static string ToString(this DateTime? o, string format, string t)
        {
            string info = string.Empty;
            if (o == null)
            {
                info = t;
            }
            else
            {
                info = o.Value.ToString(format);
            }
            return info;
        }

        /// <summary>
        /// 将TimeSpan?转换为string类型信息。
        /// </summary>
        /// <param name="o">TimeSpan?。</param>
        /// <param name="format">标准或自定义时间格式的字符串。</param>
        /// <param name="t">默认值。</param>
        /// <returns>string。</returns>
        public static string ToString(this TimeSpan? o, string format, string t)
        {
            string info = string.Empty;
            if (o == null)
            {
                info = t;
            }
            else
            {
                info = o.Value.ToString(format);
            }
            return info;
        }

        /// <summary>
        /// 将object转换为截取后的string类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="startIndex">此实例中子字符串的起始字符位置（从零开始）。</param>
        /// <param name="length">子字符串中的字符数。</param>
        /// <param name="suffix">后缀。如果没有截取则不添加。</param>
        /// <returns>截取后的string类型信息。</returns>
        public static string ToSubString(this object o, int startIndex, int length, string suffix = null)
        {
            string inputString = o.ToString(string.Empty);
            startIndex = Math.Max(startIndex, 0);
            startIndex = Math.Min(startIndex, (inputString.Length - 1));
            length = Math.Max(length, 1);
            if (startIndex + length > inputString.Length)
            {
                length = inputString.Length - startIndex;
            }
            if (inputString.Length == startIndex + length)
            {
                return inputString;
            }
            else
            {
                return inputString.Substring(startIndex, length) + suffix;
            }
        }

        /// <summary>
        /// 将object转换为byte类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>byte。</returns>
        public static byte ToByte(this object o, byte t = default(byte))
        {
            byte info;
            if (!byte.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为char类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>char。</returns>
        public static char ToChar(this object o, char t = default(char))
        {
            char info;
            if (!char.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为int类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>int。</returns>
        public static int ToInt(this object o, int t = default(int))
        {
            int info;
            if (!int.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为double类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>double。</returns>
        public static double ToDouble(this object o, double t = default(double))
        {
            double info;
            if (!double.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为decimal类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>decimal。</returns>
        public static decimal ToDecimal(this object o, decimal t = default(decimal))
        {
            decimal info;
            if (!decimal.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为float类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>float。</returns>
        public static float ToFloat(this object o, float t = default(float))
        {
            float info;
            if (!float.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为long类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>long。</returns>
        public static long ToLong(this object o, long t = default(long))
        {
            long info;
            if (!long.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为bool类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>bool。</returns>
        public static bool ToBool(this object o, bool t = default(bool))
        {
            bool info;
            if (!bool.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为sbyte类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>sbyte。</returns>
        public static sbyte ToSbyte(this object o, sbyte t = default(sbyte))
        {
            sbyte info;
            if (!sbyte.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为short类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>short。</returns>
        public static short ToShort(this object o, short t = default(short))
        {
            short info;
            if (!short.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为ushort类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>ushort。</returns>
        public static ushort ToUShort(this object o, ushort t = default(ushort))
        {
            ushort info;
            if (!ushort.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为ulong类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>ulong。</returns>
        public static ulong ToULong(this object o, ulong t = default(ulong))
        {
            ulong info;
            if (!ulong.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为Enum[T]类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>Enum[T]。</returns>
        public static T ToEnum<T>(this object o, T t = default(T))
            where T : struct
        {
            T info;
            if (!Enum.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为DateTime类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>DateTime。</returns>
        public static DateTime ToDateTime(this object o, DateTime t = default(DateTime))
        {
            if (t == default(DateTime))
            {
                t = new DateTime(1753, 1, 1);
            }
            DateTime info;
            if (!DateTime.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        /// <summary>
        /// 将object转换为TimeSpan类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <param name="t">默认值。</param>
        /// <returns>TimeSpan。</returns>
        public static TimeSpan ToTimeSpan(this object o, TimeSpan t = default(TimeSpan))
        {
            if (t == default(TimeSpan))
            {
                t = new TimeSpan(0, 0, 0);
            }
            TimeSpan info;
            if (!TimeSpan.TryParse(o.ToString(string.Empty), out info))
            {
                info = t;
            }
            return info;
        }

        private static Regex BoolRegex = new Regex("(?<info>(true|false))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 从object中获取bool类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>bool。</returns>
        public static bool? GetBool(this object o)
        {
            bool info;
            if (!bool.TryParse(BoolRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out info))
            {
                return null;
            }
            return info;
        }

        private static Regex IntRegex = new Regex("(?<info>-?\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 从object中获取int类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>int。</returns>
        public static int? GetInt(this object o)
        {
            int info;
            if (!int.TryParse(IntRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out info))
            {
                return null;
            }
            return info;
        }

        private static Regex DecimalRegex = new Regex("(?<info>-?\\d+(\\.\\d+)?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 从object中获取decimal类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>decimal。</returns>
        public static decimal? GetDecimal(this object o)
        {
            decimal info;
            if (!decimal.TryParse(DecimalRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out info))
            {
                return null;
            }
            return info;
        }

        /// <summary>
        /// 从object中获取double类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>double。</returns>
        public static double? GetDouble(this object o)
        {
            double info;
            if (!double.TryParse(DecimalRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out info))
            {
                return null;
            }
            return info;
        }

        /// <summary>
        /// 从object中获取正数信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>decimal。</returns>
        public static decimal? GetPositiveNumber(this object o)
        {
            decimal info;
            if (!decimal.TryParse(DecimalRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out info))
            {
                return null;
            }
            return Math.Abs(info);
        }

        private static Regex DateTimeRegex = new Regex("(?<info>(((\\d+)[/年-](0?[13578]|1[02])[/月-](3[01]|[12]\\d|0?\\d)[日]?)|((\\d+)[/年-](0?[469]|11)[/月-](30|[12]\\d|0?\\d)[日]?)|((\\d+)[/年-]0?2[/月-](2[0-8]|1\\d|0?\\d)[日]?))(\\s((2[0-3]|[0-1]\\d)):[0-5]\\d:[0-5]\\d)?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 从object中获取DateTime?类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>DateTime?。</returns>
        public static DateTime? GetDateTime(this object o)
        {
            DateTime info;
            if (!DateTime.TryParse(DateTimeRegex.Match(o.ToString(string.Empty)).Groups["info"].Value.Replace("年", "-").Replace("月", "-").Replace("/", "-").Replace("日", ""), out info))
            {
                return null;
            }
            return info;
        }

        private static Regex TimeSpanRegex = new Regex("(?<info>-?(\\d+\\.(([0-1]\\d)|(2[0-3])):[0-5]\\d:[0-5]\\d)|((([0-1]\\d)|(2[0-3])):[0-5]\\d:[0-5]\\d)|(\\d+))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 从object中获取TimeSpan?类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>TimeSpan?。</returns>
        public static TimeSpan? GetTimeSpan(this object o)
        {
            TimeSpan info;
            if (!TimeSpan.TryParse(TimeSpanRegex.Match(o.ToString(string.Empty)).Groups["info"].Value, out info))
            {
                return null;
            }
            return info;
        }

        private static Regex MobileRegex = new Regex("^1[3|4|5|7|8][0-9]\\d{4,8}$");
        private static Regex EmailRegex = new Regex("^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\\.[a-zA-Z0-9_-]{2,3}){1,2})$");

        /// <summary>
        /// 判断当前字符串是否是移动电话号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(this string mobile)
        {
            return MobileRegex.IsMatch(mobile);
        }

        /// <summary>
        /// 判断当前字符串是否为邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(this string email)
        {
            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// 把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
            {
                return null;
            }
            if (conversionType.IsNullableType())
            {
                conversionType = conversionType.GetUnNullableType();
            }
            if (conversionType.IsEnum)
            {
                return Enum.Parse(conversionType, value.ToString());
            }
            if (conversionType == typeof(Guid))
            {
                return Guid.Parse(value.ToString());
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T CastTo<T>(this object value)
        {
            if (value == null && default(T) == null)
            {
                return default(T);
            }
            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }
            object result = CastTo(value, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return CastTo<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将对象转为DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> source)
        {
            DataTable dtReturn = new DataTable();
            if (source == null) return dtReturn;
            dtReturn.TableName = source[0].GetType().Name; // 表名赋值
            PropertyInfo[] propertys = source[0].GetType().GetProperties();
            if (source.Count > 0)
            {
                foreach (PropertyInfo pi in propertys)
                {
                    Type colType = pi.PropertyType;
                    if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    dtReturn.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < source.Count; i++)
                {
                    ArrayList tempList = new();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(source[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dtReturn.LoadDataRow(array, true);
                }
            }
            return dtReturn;
        }
    }
}
