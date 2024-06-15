namespace TikTok_Clone_Video_Service.Interfaces
{
    public interface IRabbitMQService
    {
        void PublishMessage<T>(string exchangeName, string routingKey, T messageDto);
    }
}
