﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Util
{
    /// <summary>
    /// 验证扩展
    /// </summary>
    public static partial class Extensions
    {
        public static bool Verify(this object obj, out string message)
        {
            message = "";
            foreach (PropertyInfo pro in obj.GetType().GetProperties())
            {
                foreach (IVerifyAttribute verify in pro.GetCustomAttributes(typeof(IVerifyAttribute), true))
                {
                    if (!verify.Verify(pro.PropertyType, pro.GetValue(obj), out message))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
