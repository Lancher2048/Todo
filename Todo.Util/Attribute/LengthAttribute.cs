using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Util
{
    /// <summary>
    /// 字符串长度验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class LengthAttribute : Attribute, IVerifyAttribute
    {
        public int MinLength { get; }
        public int MaxLength { get; }
        public string Message { get; }

        public LengthAttribute(int minLength, int maxLength, string message = null)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            Message = message ?? $"字符串长度应在{minLength}到{maxLength}之间";
        }

        public bool Verify(Type type, object obj, out string message)
        {
            message = "";

            if (type == typeof(string) && obj is string str)
            {
                if (str.Length > MaxLength || str.Length < MinLength)
                {
                    message = Message;
                    return false;
                }
            }

            return true;
        }
    }
}
