using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Commons.Rabbitmq;

namespace Todo.RabbitCustomer.simple
{
    public class simpleCustomer
    {

        public void ReciveMsg()
        {
            string queueName = "simple";

            using var con = RabbitmqHelper.GetConnection();

            using var channel = con.CreateModel();

            channel.QueueDeclare(queueName, false, false, false, null);

            while (true) {
                var customer = new EventingBasicConsumer(channel);
                customer.Received += (sender, e) =>
                {
                    string msg = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine(msg);
                };

                channel.BasicConsume(queueName, true, customer);
            }
        }
    }
}
