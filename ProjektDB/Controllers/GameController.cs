using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

        [HttpPost]
        public IActionResult CreateGame()
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            GamesMethods gamesMethods = new GamesMethods();

            bool success = gamesMethods.CreateNewGame(userId, out string error); //Ska brädet initialiseras här eller ska det hanteras på eget sätt?

            if (!success)
            {
                ViewBag.ErrorMessage = "Spelet kunde inte skapas: " + error;
                return View("Lobby"); 
            }

            return RedirectToAction("Game"); // omdirigera till spelet
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

            var availableGame = gamesMethods.GetAvailableGame(out string error); // GetAvailableGames = hämta ett spel som är waiting och väntar på en spelare som ska joina.

            if (availableGame == null)
            {
                ViewBag.ErrorMessage = "Inga tillgängliga spel.";
                return View("Lobby");
            }

            bool success = gamesMethods.JoinGame(availableGame.Id, userId, out error); // JoinGame = Lägg till spelaren i tabellen kopplat till spelet och ändra status till active

            if (!success)
            {
                ViewBag.ErrorMessage = "Kunde inte ansluta till spelet: " + error;
                return View("Lobby");
            }

            _hubContext.Clients.Group(availableGame.Id.ToString()).SendAsync("PlayerJoined", userId);

            return RedirectToAction("Game", new { gameId = availableGame.Id });
        }

        [HttpPost]
        public IActionResult PlaceShip(int gameId, int startX, int startY, int endX, int endY, ShipType shipType)
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            BoardsMethods boardsMethods = new BoardsMethods();

            bool success = boardsMethods.PlaceShip(gameId, userId, startX, startY, endX, endY, shipType, out string error);
            //Validera placering:, Skeppen får inte överlappa varandra, skeppen måste ligga inom brädets gränser.


            if (!success)
            {
                return Json(new { success = false, message = error });
            }

            _hubContext.Clients.Group(gameId.ToString()).SendAsync("ShipPlaced", new { userId, startX, startY, endX, endY, shipType });

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult FireShot(int gameId, int targetX, int targetY)
        {
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            ShotsMethods shotsMethods = new ShotsMethods();

            bool hit = shotsMethods.FireShot(gameId, userId, targetX, targetY, out string error);

            if (!string.IsNullOrEmpty(error))
            {
                return Json(new { success = false, message = error });
            }

            bool gameOver = shotsMethods.CheckIfGameOver(gameId, out error);

            _hubContext.Clients.Group(gameId.ToString()).SendAsync("ShotFired", new { userId, targetX, targetY, hit, gameOver });

            return Json(new { success = true, hit, gameOver });
        }


        private Statistics UserStatistics(int userId)
        {
            GamesMethods gamesMethods = new GamesMethods();
            string error;
            var userStats = gamesMethods.GetUsersStatistics(userId, out error);
            //förmodligen bäst att göra ett DAL som ex. heter StatisticsMethods som kombinerar Users och Games osv.
            //Byt ut GamesMethods till StatisticsMethods isf.

            if (error != null)
            {
                return new Statistics //Göra en ny modell som heter Statistics för att enkelt lagra statistik
                {
                    MatchesPlayed = 0,
                    MatchesWon = 0,
                    MatchesLost = 0,
                    WinPercentage = 0.0
                };
            }

            return userStats;
        }

        /*---------------------------------------------- Game Logic ----------------------------------------------*/

        // Hantera turordning:
        // Kontrollera om det är spelarens tur.
        // Låt spelaren välja en position att skjuta på.

        // Placera skott
        // Validera om skottet är på en giltig position.
        // Kontrollera om skottet träffar ett skepp = markera vilka delar som är träffade, om alla delar av ett skepp är träffade, markera det som sänkt.

        //Kontrollera om spelet är slut

        // Hantera att återuppta ett spel
        // Ladda tidigare spelstatus, inklusive: skeppens placering, skott, vems tur det är

        //När spelet är slut: Uppdatera statistiken


    }
}
