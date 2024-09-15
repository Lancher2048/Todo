using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Commons.Rabbitmq;

namespace Todo.RabbitProvider.simple
{
    public class simpleProvider
    {

        public void sendMsg()
        {
            string queueName = "simple";

            using var con = RabbitmqHelper.GetConnection();

            using var channel = con.CreateModel();

            channel.QueueDeclare(queueName, false, false, false, null);

            string msg = "ello world";
            var body = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(string.Empty, queueName, false, null, body);

            Console.WriteLine("发送成功");

            con.Close();
        }
    }
}
