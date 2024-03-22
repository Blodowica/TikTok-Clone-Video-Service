using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TikTok_Clone_Video_Service.DatabaseContext;
using TikTok_Clone_Video_Service.Models;


namespace TikTok_Clone_Video_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly VideoDatabaseContext _dbContext;

        public VideoController(Cloudinary cloudinary, VideoDatabaseContext dbContext)
        {
            _cloudinary = cloudinary;
            _dbContext = dbContext;
        }



        [HttpPost]
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile file, [FromForm] string caption, [FromForm] bool isCommentDisabled, [FromForm] string audience)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Initialize Cloudinary configuration
            Account account = new Account(
                "your_cloud_name",
                "your_api_key",
                "your_api_secret"
            );
            Cloudinary cloudinary = new Cloudinary(account);

            // Upload video to Cloudinary
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                PublicId = "unique_id_for_video" // Provide a unique identifier for the video
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            // Check if upload was successful
            if (uploadResult.Error != null)
            {
                return StatusCode(500, $"Failed to upload: {uploadResult.Error.Message}");
            }

            // Save other data to your database
            var video = new Models.Video
            {
                Caption = caption,
                VideoURL = uploadResult.SecureUri.AbsoluteUri, // Use SecureUri instead of SecureUrl
                IsCommentsDisabled = isCommentDisabled,
                Audience = audience
            };

            // Save video object to database
            _dbContext.Add(video);
            await _dbContext.SaveChangesAsync();

            return Ok(video);
        }
    }
}
