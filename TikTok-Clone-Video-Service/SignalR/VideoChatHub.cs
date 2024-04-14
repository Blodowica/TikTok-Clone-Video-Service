using Microsoft.AspNetCore.SignalR;
using TikTok_Clone_Video_Service.DatabaseContext;
using TikTok_Clone_Video_Service.DTO;
using TikTok_Clone_Video_Service.Models;

namespace TikTok_Clone_Video_Service.SignalR
{
    public class VideoChatHub : Hub
    {
        private readonly VideoDatabaseContext _videoDatabaseContext;
        public async Task SendMessage(CommentDTO commentDto)
        {
      
            
            // Broadcast the message to clients connected to the hub associated with the videoId
            await Clients.Group(commentDto.VideoId.ToString()).SendAsync("ReceiveMessage", commentDto);
        }

        public async Task JoinVideoChat(CommentDTO commentDto)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, commentDto.VideoId.ToString());
        }

        public async Task LeaveVideoChat(string videoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, videoId);
        }
    }
}
