using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Commons
{
    public class IdWorkOptions
    {
        /// <summary>
        /// 机器码
        /// </summary>
        public long workId { get; set; } = 0;
        /// <summary>
        /// 数据中心ID
        /// </summary>
        public long datacenterId { get; set; } = 0;

    }
}
