using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TikTok_Clone_Video_Service.DatabaseContext;
using TikTok_Clone_Video_Service.DTO;
using TikTok_Clone_Video_Service.Models;

namespace TikTok_Clone_Video_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly VideoDatabaseContext _dbContext;
        public CommentController(VideoDatabaseContext videoDatabaseContext)
        {
            _dbContext = videoDatabaseContext; 
        }

        [HttpPost("SendVideoComment")]
        public async Task<IActionResult> sendComment([FromBody] CommentDTO commentDTO)
        {
            try
            {
                //check if video exist
                var video = await _dbContext.Videos.FindAsync(commentDTO.VideoId);
                if (video == null) { return BadRequest("ERROR:the video could not be found"); }


            if(commentDTO == null) { return BadRequest("the comment was empty"); }
            var comment =  new Comment
            {
                Author = commentDTO.Author,
                Content = commentDTO.Content,
                Created =  commentDTO.Created,
                VideoId = commentDTO.VideoId,
                
            };

                //save to db

                _dbContext.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();

            return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }

        }

        [HttpGet("GetAllVideoComments")]
        public IActionResult GetAllVideoComments(int videoId)
        {
            try
            {
                if (videoId == 0)
                {
                    return BadRequest("ERROR: The video ID is not provided");
                }

                var video = _dbContext.Videos.Include(v => v.Comments).FirstOrDefault(v => v.Id == videoId);

                if (video == null)
                {
                    return NotFound("ERROR: The video could not be found");
                }

                var comments = video.Comments.ToList();

                if (comments.Count == 0)
                {
                    return NoContent(); // 204 No Content
                }

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest($"ERROR: Something went wrong {ex.Message}");
            }
        }

        [HttpDelete("delteComment")]
        public async Task<IActionResult> deleteComment([FromBody] int commentId)
        {

            if (commentId == 0) { return BadRequest("ERROR: There was no comment specified for deletion"); }
            var comment = await _dbContext.Comments.FindAsync(commentId);
            if(comment == null) { return NotFound("ERROR: the comment cannot be found, it might already be deleted"); }

            _dbContext.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return Ok($"the comment was succesffuly deleted {comment}");


        }
    }
}
