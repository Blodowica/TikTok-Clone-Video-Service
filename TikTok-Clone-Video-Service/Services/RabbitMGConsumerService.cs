using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TikTok_Clone_User_Service.Services
{
    public interface IRabbitMQConsumerService
    {
        List<string> ReadAllMessages();
    }

    public class RabbitMQConsumerService : IRabbitMQConsumerService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _queueName;

        public RabbitMQConsumerService()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "172.17.0.1", // Replace with your RabbitMQ Docker container IP address
                Port = 5672,              // RabbitMQ default port
                UserName = "guest",
                Password = "guest"
            };
            _queueName = "UserPublishQueue";
        }

        public List<string> ReadAllMessages()
        {
            var messages = new List<string>();

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    messages.Add(message);
                };

                channel.BasicConsume(queue: _queueName,
                                     autoAck: true,
                                     consumer: consumer);

                // Wait for messages to be consumed
                System.Threading.Thread.Sleep(1000); // Add a delay to allow some time for messages to be consumed
            }

            return messages;
        }
    }
}
