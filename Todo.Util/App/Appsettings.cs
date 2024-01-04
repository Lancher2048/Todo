using System;
using Microsoft.Extensions.Configuration;

namespace Todo.Commons.App
{
    public class Appsettings
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public Appsettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 全局配置选项
        /// </summary>
        public static IConfiguration Configuration { get; set; }
    }
}
