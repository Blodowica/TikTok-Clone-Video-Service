using System.Text.Json.Serialization;

namespace TikTok_Clone_Video_Service.Models
{
    public class Video
    {
          

        public int Id { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }
        
        public string Caption { get; set; }
        public string VideoURL { get; set; } 

        public int Likes { get; set; } = 0;
        public string CloudinaryVideoId { get; set; }
        public string Audience { get; set; }

        public bool IsCommentsDisabled { get; set;}
        
         public DateTime CreatedAt {  get; set; }
         public DateTime UpdatedAt {  get; set; }


        public ICollection<Comment>? Comments { get; set; }

        public ICollection<UserLikedVideos>? UserLikedVideos { get; set; } 

    }
}
