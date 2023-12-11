using System.Collections;

namespace Todo.Util
{
    /// <summary>
    /// 非空验证
    /// </summary>
    public class IsNotNullAttribute : Attribute, IVerifyAttribute
    {
        public bool IsNotNull { get; set; } = true;

        public string Message { get; set; } = "不能为空";

        public IsNotNullAttribute() { }

        public IsNotNullAttribute(bool isNotNull)
        {
            IsNotNull = isNotNull;
        }

        public IsNotNullAttribute(bool isNull, string message)
        {
            IsNotNull = isNull;
            Message = message;
        }
        public bool Verify(Type type, object obj, out string message)
        {
            message = "";

            if (IsNotNull == false)
            {
                return true;
            }

            if (obj == null || (obj is IList list && list.Count <= 0))
            {
                message = Message;
                return false;
            }

            return true;
        }
    }
}
