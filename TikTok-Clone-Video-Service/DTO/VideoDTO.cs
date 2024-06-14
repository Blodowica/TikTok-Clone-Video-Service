using TikTok_Clone_Video_Service.Models;

namespace TikTok_Clone_Video_Service.DTO
{
    public class VideoDTO
    {
       public required IFormFile file { get; set; }

        public required string Caption { get; set; }

        public required bool IsCommentsDisabled { get; set; }
        public required string Audience { get; set; }
        public required int AuthorId { get; set; }
        public required string AuthorName { get; set; }

        public ICollection<UserLikedVideos>? LikedusersID { get; set; } 

    }
}
