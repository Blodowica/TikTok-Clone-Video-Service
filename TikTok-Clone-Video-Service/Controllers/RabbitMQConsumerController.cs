using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TikTok_Clone_User_Service.Services;

namespace TikTok_Clone_Video_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQConsumerController : ControllerBase
    {
        private readonly IRabbitMQConsumerService _consumerService;
        private readonly ILogger _logger;

        public RabbitMQConsumerController(ILogger<RabbitMQConsumerController> logger, IRabbitMQConsumerService consumerService)
        {
            _logger = logger;
            _consumerService = consumerService;
        }

        [HttpGet]
        public IActionResult readAllMessages()
        {
            var messages = _consumerService.ReadAllMessages();
            return Ok(messages);
        }
    }
}
