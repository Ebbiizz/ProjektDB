using Microsoft.AspNetCore.SignalR;
using ProjektDB.Models;
using System.Threading.Tasks;

namespace ProjektDB.Hubs
{
    public class GameHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var gameId = Context.GetHttpContext().Request.Query["gameId"];
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid inlämning till grupp: {ex.Message}");
            }
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var gameId = Context.GetHttpContext().Request.Query["gameId"];
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid borttagning från grupp: {ex.Message}");
            }
            await base.OnDisconnectedAsync(exception);
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

        public async Task FireShot(int gameId, int userId, int targetX, int targetY, bool hit, bool gameOver)
        {
            await Clients.Group(gameId.ToString()).SendAsync("ShotFired", new
            {
                gameId,
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
    }
}
