using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Commons.Rabbitmq
{
    public class RabbitmqHelper
    {
        /// <summary>
        /// 获取链接工厂
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()
        {
            var fac = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
            };
            return fac.CreateConnection();
        }
    }
}
