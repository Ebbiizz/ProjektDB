using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Cmp;
using ProjektDB.Hubs;
using ProjektDB.Models;

////Lägg in i alla vyer för att signal r ska funka:
//<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/5.0.9/signalr.min.js"></script>
//<script src="~/wwwroot/js/gameHub.js"></script>


namespace ProjektDB.Controllers
{
    public class GameController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext; //Signal-R

        public GameController(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Lobby()
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            GamesMethods gamesMethods = new GamesMethods();
            var activeGames = gamesMethods.GetActiveGames(out string error);

            if (error != null)
            {
                ViewBag.ErrorMessage = "Spelet kunde inte hämtas: " + error;
            } // Eller, inga aktiva spel. 

            return View(activeGames);
        }

        [HttpGet]
        public IActionResult CreateGame()
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            GamesMethods gamesMethods = new GamesMethods();

            bool success = gamesMethods.CreateNewGame(userId, out string error); 

            if (!success)
            {
                ViewBag.ErrorMessage = "Spelet kunde inte skapas: " + error;
                return View("Lobby"); 
            }

            // Skapar bräde
            BoardsMethods boardMethods = new BoardsMethods();
            int recentGameId = gamesMethods.GetRecentGameId(out string errormsg);


            bool boardCreated = boardMethods.CreateBoard(recentGameId, userId, out string boardError);

            if (!boardCreated)
            {
                ViewBag.ErrorMessage = "Brädet kunde inte skapas: " + boardError;
                return View("Lobby");
            }

            return RedirectToAction("Game", new { gameId = recentGameId });
        }


        [HttpGet]
        public IActionResult JoinGame()
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            GamesMethods gamesMethods = new GamesMethods();
            List<Games> availableGamesList = gamesMethods.GetAvailableGame(out string error);
            Random random = new Random();
            Games randomGame = availableGamesList[random.Next(availableGamesList.Count)];

            if (availableGamesList == null)
            {
                ViewBag.ErrorMessage = "Inga tillgängliga spel.";
                return View("Lobby");
            }

            bool success = gamesMethods.JoinGame(randomGame.Id, userId, out error);

            if (!success)
            {
                ViewBag.ErrorMessage = "Kunde inte ansluta till spelet: " + error;
                return View("Lobby");
            }

            BoardsMethods boardMethods = new BoardsMethods();

            bool boardCreated = boardMethods.CreateBoard(randomGame.Id, userId, out string boardError);

            if (!boardCreated)
            {
                ViewBag.ErrorMessage = "Brädet kunde inte skapas: " + boardError;
                return View("Lobby");
            }
            _hubContext.Clients.Group(randomGame.Id.ToString()).SendAsync("PlayerJoined", userId);

            return RedirectToAction("Game", new { gameId = randomGame.Id });
        }

        public IActionResult Game(int gameId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult PlaceShip(int gameId, int startX, int startY, int endX, int endY, string shipType)
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }
            string error = "";
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            ShipsMethods shipsMethods = new ShipsMethods();
            BoardsMethods boardMethods = new BoardsMethods();
            Boards board = boardMethods.GetBoard(gameId, userId, out error);

            if (!Enum.TryParse(shipType, out ShipType parsedShipType))
            {
                return Json(new { success = false, message = "Ogiltig skeppstyp." });
            }
            List<Ships> shipsOnBoard = shipsMethods.GetShipsOnBoard(board.Id, out error);
            if (shipsOnBoard == null || shipsOnBoard.Count < 5)
            {
                bool success = shipsMethods.PlaceShip(board.Id, startX, startY, endX, endY, parsedShipType, out error);
                //Validera placering:, Skeppen får inte överlappa varandra, skeppen måste ligga inom brädets gränser.


                if (!success)
                {
                    return Json(new { success = false, message = error });
                }

                _hubContext.Clients.Group(gameId.ToString()).SendAsync("ShipPlaced", new { userId, startX, startY, endX, endY, shipType });

                return Json(new { success = true });
            }
            else
            {
                //skrev detta bara för att kunna testa
                return RedirectToAction("Game"); 
            }
        }

        [HttpPost]
        public IActionResult FireShot(int gameId, int targetX, int targetY)
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            string errormsg = "";
            GamesMethods gamesMethods = new GamesMethods();
            Games game = gamesMethods.GetGameById(gameId, out errormsg);
            BoardsMethods boardsMethods = new BoardsMethods();
            Boards board = new Boards();

            if (userId == game.Player1Id)
            {
                board = boardsMethods.GetBoard(gameId, game.Player2Id, out errormsg);
            }
            else
            {
                board = boardsMethods.GetBoard(gameId, game.Player1Id, out errormsg);
            };
            ShipsMethods shipsMethods = new ShipsMethods();
            List<Ships> shipsOnOpponentsBoard = shipsMethods.GetShipsOnBoard(board.Id, out errormsg);

            if (shipsOnOpponentsBoard.Count == 5)
            {

                ShotsMethods shotsMethods = new ShotsMethods();

                bool hit = shotsMethods.FireShot(gameId, userId, targetX, targetY, out string error);
                // Validera om skottet är på en giltig position.
                // Kontrollera om skottet träffar ett skepp = markera vilka delar som är träffade, om alla delar av ett skepp är träffade, markera det som sänkt.

                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { success = false, message = error });
                }

                //ändrat till från gameId till userId då det bara är den som just sköt som kan ha vunnit
                bool gameOver = shotsMethods.CheckIfGameOver(userId, out error);

                if (gameOver == true)
                {
                    bool win = gamesMethods.SetWinner(gameId, userId, out error);
                    int i = shotsMethods.ClearShots(userId, out error);
                    int j = boardsMethods.RemoveBoard(userId, out error);
                } 

                _hubContext.Clients.Group(gameId.ToString()).SendAsync("ShotFired", new { userId, targetX, targetY, hit, gameOver });

                return Json(new { success = true, hit, gameOver }); 
            }
            else
            {
                return null;
            }
        }

        public IActionResult ResumeGame(int gameId) //Lagra gameID i vyn för ouppklarade games
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            GamesMethods gamesMethods = new GamesMethods();
            Games game = gamesMethods.GetGameById(gameId, out string error);
            // Ladda tidigare spelstatus, inklusive: skeppens placering, skott, vems tur det är.
            // spelstatus hämtas i samband med GameID men hur hämtas skeppens placering, skott och vems tur det är? Kan ej hitta att det är PK/FK någonstans till gameID

            if (game == null || !string.IsNullOrEmpty(error))
            {
                ViewBag.ErrorMessage = "Spelet kunde inte laddas: " + error;
                return View("Lobby");
            }

            return View("Game", game);
        }


        private void UpdateStatistics(int userId)
        {
            StatisticsMethods statisticsMethods = new StatisticsMethods();
            var userStats = statisticsMethods.GetUserStatistics(userId, out string error);

            if (error != null)
            {
                _hubContext.Clients.User(userId.ToString()).SendAsync("StatisticsUpdated", new
                {
                    MatchesPlayed = 0,
                    MatchesWon = 0,
                    MatchesLost = 0,
                    WinPercentage = 0.0
                });
            }

            _hubContext.Clients.User(userId.ToString()).SendAsync("StatisticsUpdated", new
            {
                MatchesPlayed = userStats.MatchesPlayed,
                MatchesWon = userStats.MatchesWon,
                MatchesLost = userStats.MatchesLost,
                WinPercentage = userStats.WinPercentage
            });
        }


        /*---------------------------------------------- Game Logic ----------------------------------------------*/

        // Hantera turordning:
        // Kontrollera om det är spelarens tur.
        // Låt spelaren välja en position att skjuta på.

    }
}
