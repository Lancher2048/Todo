using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Util
{
    public static class SnowflakeHelper
    {
        /// <summary>
        /// 获取雪花ID
        /// </summary>
        /// <returns></returns>
        public static long GetId()
        {
            var worker = new IdWorker(1, 1);
            return worker.NextId();
        }
    }
}
