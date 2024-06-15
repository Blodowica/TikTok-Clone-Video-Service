using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;

public interface IRabbitMQService
{
    void PublishMessage<T>(string exchangeName, string routingKey, T messageDto);
}

public class RabbitMQService : IRabbitMQService
{
    private readonly ConnectionFactory _connectionFactory;

    public RabbitMQService(IConfiguration configuration)
    {
        try
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration");

            _connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMQConfig["Hostname"],
                Port = Convert.ToInt32(rabbitMQConfig["Port"]),
                UserName = rabbitMQConfig["Username"],
                Password = rabbitMQConfig["Password"]
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing RabbitMQ connection: {ex.Message}");
            throw;
        }
    }

    public void PublishMessage<T>(string exchangeName, string queueName, T contentDto)
    {
        var message = JsonConvert.SerializeObject(contentDto);

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the exchange if it doesn't exist with the desired properties
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            //Declare the queue to ensure the queque exist 
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);


            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: queueName);


            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Ensure messages are persistent

            // Publish the message to the exchange
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: queueName,
                                 basicProperties: properties,
                                 body: body);

            Console.WriteLine($" [x] Sent {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing message to RabbitMQ: {ex.Message}");
            throw;
        }
    }
}
