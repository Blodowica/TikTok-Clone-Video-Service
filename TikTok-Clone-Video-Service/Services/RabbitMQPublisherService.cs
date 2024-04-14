using Microsoft.Extensions.Configuration;
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

        public RabbitMQPublisherService(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("CloudinarySettings");

            _connectionFactory = new ConnectionFactory{
                HostName = rabbitMQConfig["Hostname"], // Replace with your RabbitMQ Docker container IP address
                Port = Convert.ToInt32(rabbitMQConfig["Port"]),       // RabbitMQ default port
                UserName = rabbitMQConfig["Username"],
                Password = rabbitMQConfig["Password"]
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
