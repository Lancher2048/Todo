using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Todo.Commons.Extend
{
    /// <summary>
    /// 字符串处理
    /// </summary>
    public static class ExtString
    {
        /// <summary>
        /// 转换为字节流
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByte(this string value)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// 转换为UrlEncode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(this string value)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(value);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }
            return (sb.ToString());
        }

        /// <summary>
        /// 转换为ToUnicode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUnicode(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                builder.Append("\\u" + ((int)value[i]).ToString("x"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 是否字母和数字
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(this string instance)
        {
            Regex reg = new Regex("^[a-zA-Z0-9]+$");
            return reg.IsMatch(instance);
        }

        #region 文件路径转换
        /// <summary>
        /// 文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToFilePath(this string path)
        {
            return string.Join(Path.DirectorySeparatorChar.ToString(), path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        /// <summary>
        /// 文件路径转换
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReplacePath(this string path)
        {
            bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");

        }

        /// <summary>
        /// 文件路径拼接
        /// </summary>
        /// <param name="p">原有文件路径</param>
        /// <param name="path">新文件路径</param>
        /// <returns></returns>
        public static string CombinePath(this string p, string path)
        {
            return p + Path.DirectorySeparatorChar + path;
        }
        #endregion

        #region Base64加密解密
        /// <summary>
        /// Base64加密，采用指定字符编码方式加密。
        /// </summary>
        /// <param name="input">待加密的明文</param>
        /// <param name="encode">字符编码</param>
        /// <returns></returns>
        public static string Base64Encrypt(this string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }

        /// <summary>
        /// Base64加密，采用UTF8编码方式加密。
        /// </summary>
        /// <param name="input">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encrypt(this string input)
        {
            return Base64Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密，采用UTF8编码方式解密。
        /// </summary>
        /// <param name="input">待解密的秘文</param>
        /// <returns></returns>
        public static string Base64Decrypt(this string input)
        {
            return Base64Decrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密，采用指定字符编码方式解密。
        /// </summary>
        /// <param name="input">待解密的秘文</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Base64Decrypt(this string input, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(input));
        }
        #endregion
    }
}
