using Microsoft.AspNetCore.Mvc;
using TikTok_Clone_Video_Service.DTO;
using TikTok_Clone_Video_Service;

namespace TikTok_Clone_Video_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQPublisherController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public RabbitMQPublisherController(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public IActionResult SendVideoMessage([FromBody] CommentDTO comment)
        {
            _rabbitMQService.PublishMessage("comment_exchange", "testing_comment_queue", comment);
            return Ok($"The message for comment {comment.Content} has been sent");
        }
    }
}
