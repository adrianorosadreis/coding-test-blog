using Microsoft.AspNetCore.SignalR;
using SimpleBlog.Hubs;
using SimpleBlog.Models;

namespace SimpleBlog.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<PostHub> _hubContext;

        public NotificationService(IHubContext<PostHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SendPostCreatedNotification(Post post)
        {
            // Envia a notificação para todos os clientes conectados
            _hubContext.Clients.All.SendAsync("ReceivePostCreatedNotification", post);
        }
    }
}