using Microsoft.AspNetCore.Mvc;
using ProjektDB.Models;

namespace ProjektDB.Controllers
{
    public class GameController : Controller
    {
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

            // SignalR notis = hub?

            return RedirectToAction("Game", new { gameId = availableGame.Id });
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

        // Placera skepp
        // Tillåt spelarna att placera sina skepp i tur och ordning.
        // Validera placering:, Skeppen får inte överlappa varandra, skeppen måste ligga inom brädets gränser.
        // När båda spelarnas placeringar är validerade = spela

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
