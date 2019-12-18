using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIN.MSA.Infrastructure
{
    public class RabbitUtil
    {
        //public Dictionary<string, IConnectionFactory> rabbitPool = new Dictionary<string, IConnectionFactory>();

        //public IConnectionFactory GetRabbitFactory(RabbitEntity model)
        //{
        //    if (rabbitPool.ContainsKey(model.Name))
        //    {
        //        return rabbitPool[model.Name];
        //    }
        //    else
        //    {
        //        IConnectionFactory conFactory = new ConnectionFactory//创建连接工厂对象
        //        {
        //            HostName = model.HostName,//IP地址
        //            Port = model.Port,//端口号
        //            UserName = model.UserName,//用户账号
        //            Password = model.Password //用户密码
        //        };

        //        rabbitPool.Add(model.Name, conFactory);
        //        return conFactory;
        //    }
        //}

        public IConnectionFactory conFactory;
        private string rabbitName;

        public RabbitUtil(RabbitEntity model)
        {
            this.rabbitName = model.Name;

            conFactory = new ConnectionFactory//创建连接工厂对象
            {
                HostName = model.HostName,//IP地址
                Port = model.Port,//端口号
                UserName = model.UserName,//用户账号
                Password = model.Password //用户密码
            };
        }

        public void Publish(string qname,string message)
        {
            try
            {
                using (IConnection con = conFactory.CreateConnection())//创建连接对象
                {
                    using (IModel channel = con.CreateModel())//创建连接会话对象
                    {
                        var queueName = string.Empty;
                        if (qname.Length > 0)
                            queueName = qname;
                        else
                            queueName = "LINQUEUE";
                        //声明一个队列
                        channel.QueueDeclare(
                          queue: queueName,//消息队列名称
                          durable: false,//是否缓存
                          exclusive: false,
                          autoDelete: false,
                          arguments: null
                           );

                        //消息内容
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        //发送消息
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string Receive(string qname)
        {
            var result = "";
            try
            {
                using (IConnection conn = conFactory.CreateConnection())
                {
                    using (IModel channel = conn.CreateModel())
                    {
                        String queueName = String.Empty;
                        if (qname.Length > 0)
                            queueName = qname;
                        else
                            queueName = "LINQUEUE";
                        //声明一个队列
                        channel.QueueDeclare(
                          queue: queueName,//消息队列名称
                          durable: false,//是否缓存
                          exclusive: false,
                          autoDelete: false,
                          arguments: null
                           );
                        //创建消费者对象
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            byte[] message = ea.Body;//接收到的消息
                            result = Encoding.UTF8.GetString(message);
                            Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message));
                        };
                        //消费者开启监听
                        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }


    }

    public class RabbitEntity
    {
        public string Name { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
