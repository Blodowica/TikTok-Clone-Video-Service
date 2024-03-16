using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TikTok_Clone_Video_Service.Services;

namespace TikTok_Clone_Video_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQPublisherController : ControllerBase
    {
        private readonly IRabbitMQPublisherService _rabbitMQPublisherService;
        

        public RabbitMQPublisherController( IRabbitMQPublisherService rabbitMQPublisherController)
        {

            
            _rabbitMQPublisherService = rabbitMQPublisherController;

        }

        [HttpPost]
        public  IActionResult SendVideoMessage([FromBody] string message)
        {
             _rabbitMQPublisherService.sendMessage(message);
            return Ok($"the message {message} has been sent");

        }
    }
}
