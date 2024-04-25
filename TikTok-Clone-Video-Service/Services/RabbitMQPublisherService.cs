using Newtonsoft.Json;
using RabbitMQ.Client;
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
        var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration");

        _connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMQConfig["Hostname"],
            Port = Convert.ToInt32(rabbitMQConfig["Port"]),
            UserName = rabbitMQConfig["Username"],
            Password = rabbitMQConfig["Password"]
        };
    }

    public void PublishMessage<T>(string exchangeName, string routingKey,  T messageDto)
    {
        var message = JsonConvert.SerializeObject(messageDto);

        try
        {
            if (message == null) { Console.WriteLine("The content of the message seems to be empty"); }
            else
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: properties,
                                     body: body);

                Console.WriteLine($" [x] Sent {message}");
            }
        }
        catch
        {
            throw new Exception("Error something went wrong");
        }
    }
}