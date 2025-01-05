using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ProjektDB.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGame(int gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
        }

        public async Task LeaveGame(int gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
        }

        //Ships - placera skotten - ändra vy genom .js

        //fireShot - placera skott - ändra vy via .js
    }
}
