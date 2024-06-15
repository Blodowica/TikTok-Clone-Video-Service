using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TikTok_Clone_Video_Service.DTO;
using TikTok_Clone_Video_Service.Models;
using TikTok_Clone_Video_Service.DatabaseContext;
using TikTok_Clone_Video_Service.Interfaces;

namespace TikTok_Clone_User_Service.Services
{
  

    public class UploadVideoService : IUploadVideoService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly VideoDatabaseContext _dbContext;
        private readonly Cloudinary _cloudinary;


        public UploadVideoService(IRabbitMQService rabbitMQService, VideoDatabaseContext dbContext, Cloudinary cloudinary)
        {
            _rabbitMQService = rabbitMQService ?? throw new ArgumentNullException(nameof(rabbitMQService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }

        public async Task<bool> SendVideoToQueueAsync(VideoDTO videoDTO)
        {
            try
            {
                _rabbitMQService.PublishMessage("video_exchange", "video_upload_queue", videoDTO);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading video: {ex.Message}");
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> UploadVideo(VideoEncodedDTO videoDTO)
        {
            try
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(videoDTO.Filetype, new MemoryStream(videoDTO.FileBytes))
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Failed to upload: {uploadResult.Error.Message}");
                }

                var video = new TikTok_Clone_Video_Service.Models.Video
                {
                    Caption = videoDTO.Caption,
                    VideoURL = uploadResult.SecureUri.AbsoluteUri,
                    IsCommentsDisabled = videoDTO.IsCommentsDisabled,
                    Audience = videoDTO.Audience,
                    AuthorId = videoDTO.AuthorId,
                    AuthorName = videoDTO.AuthorName,
                    CloudinaryVideoId = uploadResult.PublicId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Videos.Add(video);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading video: {ex.Message}");
                return false;
            }
        }
    }
}
