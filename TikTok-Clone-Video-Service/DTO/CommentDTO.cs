namespace TikTok_Clone_Video_Service.DTO
{
    public class CommentDTO
    {
     
        public required string Author { get; set; }
        public required string Content { get; set; }
        public int UserId { get; set; }

        public required DateTime Created { get; set; }
        = DateTime.Now;

        public required int VideoId { get; set; }
   
    }
}
