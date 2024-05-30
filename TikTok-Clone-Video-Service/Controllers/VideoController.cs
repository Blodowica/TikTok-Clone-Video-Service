﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
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
        private readonly IConfiguration _configuration;

        public VideoController(Cloudinary cloudinary, VideoDatabaseContext dbContext, IConfiguration configuration)
        {
            _cloudinary = cloudinary;
            _dbContext = dbContext;
            _configuration = configuration;
            var cloudinarySettings = _configuration.GetSection("CloudinarySettings");
            var cloudName = cloudinarySettings["CloudName"];
            var apiKey = cloudinarySettings["ApiKey"];
            var apiSecret = cloudinarySettings["ApiSecret"];

            // Initialize Cloudinary instance with configuration from appsettings.json
            var account = new Account(cloudName, apiKey, apiSecret);

            _cloudinary = new Cloudinary(account);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile file, [FromForm] string caption, [FromForm]bool isCommentsDisabled, [FromForm] string audience, [FromForm]int authorId)

        { 
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Upload video to Cloudinary
                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Check if upload was successful
                if (uploadResult.Error != null)
                {
                    return StatusCode(500, $"Failed to upload: {uploadResult.Error.Message}");
                }

                // Save other data to your database
                var video = new Models.Video
                {
                    Caption = caption,
                    VideoURL = uploadResult.SecureUri.AbsoluteUri,
                    IsCommentsDisabled = isCommentsDisabled,
                    Audience = audience,
                    AuthorId = authorId,
                    CloudinaryVideoId = uploadResult.PublicId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,


                };

                // Save video object to database
                _dbContext.Add(video);
                await _dbContext.SaveChangesAsync();

                return Ok(video);
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("GetvideoById")]
        public async Task<IActionResult> getVideoById(string publicVideoId)
        {
            try
            {
                if(string.IsNullOrEmpty(publicVideoId))
                {
                    return BadRequest("The video publish id was empty");
                }
                var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.CloudinaryVideoId == publicVideoId);

                if(video == null )
                {
                    return NotFound("The video could not be found");
                }

                return Ok(video);
            }
            catch(Exception ex){
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }
        [HttpGet("GetAllVideos")]
        public async Task<IActionResult> GetAllVideos()
        {
            try
            {
                // Retrieve all videos from your database (assuming you have a Videos DbSet)
                var videos = await _dbContext.Videos
                    .OrderByDescending(v => v.CreatedAt) // Sort by upload date (descending order)
                    .ToListAsync();

                if (videos == null || videos.Count == 0)
                {
                    return NotFound("No videos found.");
                }

                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("GetAllVideosPaginated")]
        public async Task<IActionResult> GetAllVideosPaginated(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Calculate the number of items to skip based on the page number and page size
                int skip = (pageNumber - 1) * pageSize;

                // Retrieve a paginated list of videos from your database
                var videos = await _dbContext.Videos
                    .OrderByDescending(v => v.CreatedAt) // Sort by upload date (descending order)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                if (videos == null || videos.Count == 0)
                {
                    return NotFound("No videos found.");
                }

                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("likeVideo")]

        public async Task<IActionResult> likeVideoById(int videoId)
        {
            try {
            
                if(videoId <= 0)  { return BadRequest("Error something went wrong: the video value was not correctly provided"); }
                
                //find video 
                var video = await _dbContext.Videos.FindAsync(videoId);
                if(video == null) { return NotFound("The video could not be found"); }

                //find the user or sent the data to the user service TO DOOO LATER 

                //check if the video is already like by the user 

                //update the like 
                video.Likes++;

                _dbContext.SaveChanges();
                return Ok("The video's like was successfully updated!");
            
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        



        [HttpPut("UpdateVideo")]
        public async Task<IActionResult> updateVideoMetaData(string publishId, string caption = null, bool isCommentsDisabled = false, string audience = null   )
        {

            try
            {

                var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.CloudinaryVideoId == publishId);
                if (video == null) { return NotFound("The video could not be found"); };

                video.UpdatedAt = DateTime.UtcNow;
                
                if(caption != null)
                {
                    video.Caption = caption;
                }

                if(isCommentsDisabled)
                {
                    video.IsCommentsDisabled = true;
                }
                if (audience != null)
                {
                    video.Audience = audience;
                }

                _dbContext.Update(video);

              await  _dbContext.SaveChangesAsync();

                return Ok(video);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpDelete("DeleteVideo")]
        public async Task<IActionResult> DeleteVideo(string publicVideoId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicVideoId))
                {
                    return BadRequest("The video public ID is required.");
                }

                // Find video by publicVideoId
                var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.CloudinaryVideoId == publicVideoId);

                if (video == null)
                {
                    return NotFound("Video not found.");
                }

                // Delete video from Cloudinary
                var deletionParams = new DeletionParams(publicVideoId)
                {
                    ResourceType = ResourceType.Video
                };

                var deleteResult = await _cloudinary.DestroyAsync(deletionParams);

                if (deleteResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Remove video from database
                    _dbContext.Videos.Remove(video);
                    await _dbContext.SaveChangesAsync();

                    return Ok("Video deleted successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to delete video from Cloudinary.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}