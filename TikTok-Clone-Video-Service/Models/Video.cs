namespace TikTok_Clone_Video_Service.Models
{
    public class Video
    {
          

        public int Id { get; set; }
        
        public string Caption { get; set; }
        public string VideoURL { get; set; } 
        public string Audience { get; set; }

        public bool IsCommentsDisabled { get; set;}

        public ICollection<Comment> Comments { get; set; }
       

    }
}
