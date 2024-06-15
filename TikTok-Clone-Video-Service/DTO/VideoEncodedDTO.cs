namespace TikTok_Clone_Video_Service.DTO
{
    public class VideoEncodedDTO
    {
        public byte[]? FileBytes { get; set; }
        public string? Filetype { get; set; }  

        public required string Caption { get; set; }

        public required bool IsCommentsDisabled { get; set; }
        public required string Audience { get; set; }
        public required int AuthorId { get; set; }
        public required string AuthorName { get; set; }
    }
}
