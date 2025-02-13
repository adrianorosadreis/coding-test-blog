using Microsoft.AspNetCore.SignalR;

namespace SimpleBlog.Hubs
{
    public class PostHub : Hub
    {
        public async Task SendNewPostNotification(string postTitle, string userName)
        {
            // Envia a notificação para todos os clientes conectados
            await Clients.All.SendAsync("ReceiveNewPostNotification", postTitle, userName);
        }
    }
}