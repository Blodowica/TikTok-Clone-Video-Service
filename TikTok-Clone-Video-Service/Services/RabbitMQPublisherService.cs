using RabbitMQ.Client;
using System.Text;

namespace TikTok_Clone_Video_Service.Services
{

    public interface IRabbitMQPublisherService
    {
        void sendMessage(string message);
    }

    public class RabbitMQPublisherService : IRabbitMQPublisherService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly string _queueName;

        public RabbitMQPublisherService()
        {
            _connectionFactory = new ConnectionFactory{
                HostName = "172.17.0.5", // Docker container IP address
                Port = 5672,              // RabbitMQ default port
                UserName = "guest",
                Password = "guest"
            };

            _queueName = "VideoPublishQueue";
            
        }

        public void sendMessage(string message)
        {
            try
            {
                if(message == null) { Console.WriteLine("The content of the message seems to be empty"); }
                else
                {

               
                    //establish connection
                    using var connection  = _connectionFactory.CreateConnection();
                    using var channel = connection.CreateModel();
            
                    channel.QueueDeclare(queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
           
                    var body = Encoding.UTF8.GetBytes(message);


                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: _queueName,
                                         basicProperties: properties,
                                         body: body);

                    Console.WriteLine($" [x] Sent {message}");
                }
                    }
            catch {
                throw new Exception("Error something went wrong");
            }


        }
    }
}
