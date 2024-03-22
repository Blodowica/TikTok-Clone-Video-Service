namespace TikTok_Clone_Video_Service.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        = DateTime.Now;

       public int VideoId { get; set; } 
      public Video Video { get; set; }      
                  
    }
}
