using System.Text.Json.Serialization;

namespace TikTok_Clone_Video_Service.Models
{
    public class UserLikedVideos
    {
        public int Id { get; set; }

        public required int VideoID { get; set; }
        public required string authID { get; set; }

        [JsonIgnore]
        public Video Video { get; set; }
    }
}
