using Microsoft.AspNetCore.SignalR;
using TikTok_Clone_Video_Service.DTO;
using System.Threading.Tasks;

namespace TikTok_Clone_Video_Service.SignalR
{
    public class CommentHub : Hub
    {
        public async Task PostComment(CommentDTO commentDto)
        {
            // Add the connection to the group corresponding to the video
            await Groups.AddToGroupAsync(Context.ConnectionId, commentDto.VideoId.ToString());

            Console.WriteLine(commentDto);

            // Broadcast the comment to all clients viewing the video
            await Clients.Group(commentDto.VideoId.ToString()).SendAsync("RecieveComment", commentDto);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        

    }
}
