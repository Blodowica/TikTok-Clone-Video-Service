using Microsoft.AspNetCore.Mvc;
using TikTok_Clone_Video_Service.DTO;
using TikTok_Clone_Video_Service.Models;

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
            try
            {
                _rabbitMQService.PublishMessage("comment_exchange", "testing_comment_queue", comment);
                return Ok($"The message for comment '{comment.Content}' has been sent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("likeUserVideoRabbitMq")]
        public IActionResult UserLikedVideoAction([FromForm] UserLikedVideosDTO userLikedVideos)
        {
            try
            {

            _rabbitMQService.PublishMessage("video_exchange", "like_video_queue", userLikedVideos);
            return Ok($"The message for like videos for the video {userLikedVideos.VideoID} was sucessfully sent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
