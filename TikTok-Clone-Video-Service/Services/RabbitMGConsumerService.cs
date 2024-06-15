using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TikTok_Clone_Video_Service.DTO;
using TikTok_Clone_Video_Service.DatabaseContext;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using TikTok_Clone_Video_Service.Interfaces;

namespace TikTok_Clone_User_Service.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _exchangeName;
        private readonly string _queueName;
    
        private IModel _channel;

        public RabbitMQConsumer(ConnectionFactory connectionFactory, IServiceProvider serviceProvider, string exchangeName, string queueName)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _exchangeName = exchangeName;
            _queueName = queueName;
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var connection = _connectionFactory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _queueName);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Received: {0}");

                bool success = await HandleMessageAsync(message, _queueName);

                if (success)
                {
                   _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            Console.WriteLine($"Consuming messages from queue '{_queueName}'...");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }

        private async Task<bool> HandleMessageAsync(string message, string queueName)
        {
            try
            {
                if (queueName == "upload_video_queue")
                {
                    var videoDTO = JsonConvert.DeserializeObject<VideoEncodedDTO>(message);

                    if (videoDTO != null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var uploadVideoService = scope.ServiceProvider.GetRequiredService<IUploadVideoService>();
                            try
                            {

                            return await uploadVideoService.UploadVideo(videoDTO);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing like action: {ex.Message}");
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                return false;
            }
        }

    }
}
