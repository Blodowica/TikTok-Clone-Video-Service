using TikTok_Clone_Video_Service.DTO;

namespace TikTok_Clone_Video_Service.Interfaces
{
    public interface IUploadVideoService
    {
        Task<bool> SendVideoToQueueAsync(VideoDTO videoDTO);
        Task<bool> UploadVideo(VideoEncodedDTO videoDTO);
    }
}
