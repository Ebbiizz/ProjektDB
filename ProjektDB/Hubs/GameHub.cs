using Microsoft.AspNetCore.SignalR;
using ProjektDB.Models;
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

        public async Task ShipPlaced(int gameId, int userId, int startX, int startY, int endX, int endY, string shipType)
        {
            await Clients.Group(gameId.ToString()).SendAsync("ShipPlaced", new
            {
                userId,
                startX,
                startY,
                endX,
                endY,
                shipType
            });
        }

        public async Task FireShot(int gameId, int userId, int targetX, int targetY)
        {
            // Här kan vi implementera logik för att avgöra om skottet träffade
            bool hit = CheckIfHit(targetX, targetY);
            bool gameOver = CheckIfGameOver(gameId);

            await Clients.Group(gameId.ToString()).SendAsync("ShotFired", new
            {
                userId,
                targetX,
                targetY,
                hit,
                gameOver
            });
        }

        public async Task PlayerJoined(int gameId, int userId)
        {
            await Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", userId);
        }

        public async Task StatisticsUpdated(int userId)
        {
            StatisticsMethods statisticsMethods = new StatisticsMethods();
            var userStats = statisticsMethods.GetUserStatistics(userId, out string error);

            if (error != null)
            {
                await Clients.User(userId.ToString()).SendAsync("StatisticsUpdated", new
                {
                    MatchesPlayed = 0,
                    MatchesWon = 0,
                    MatchesLost = 0,
                    WinPercentage = 0.0
                });
                return;
            }

            await Clients.User(userId.ToString()).SendAsync("StatisticsUpdated", new
            {
                MatchesPlayed = userStats.MatchesPlayed,
                MatchesWon = userStats.MatchesWon,
                MatchesLost = userStats.MatchesLost,
                WinPercentage = userStats.WinPercentage
            });
        }

        // Hjälpmetoder för att kontrollera om skottet träffade eller om spelet är över
        private bool CheckIfHit(int targetX, int targetY)
        {
            // Här kan du implementera logik för att avgöra om skottet träffade
            return false;
        }

        private bool CheckIfGameOver(int gameId)
        {
            // Här kan du implementera logik för att avgöra om spelet är över
            return false;
        }
    }
}
