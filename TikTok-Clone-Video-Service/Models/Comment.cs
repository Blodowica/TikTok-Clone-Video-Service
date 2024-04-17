using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TikTok_Clone_Video_Service.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }
        public string Author { get; set; }

        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        = DateTime.Now;

       public int VideoId { get; set; }


        [JsonIgnore]
        public Video Video { get; set; }      
                  
    }
}
