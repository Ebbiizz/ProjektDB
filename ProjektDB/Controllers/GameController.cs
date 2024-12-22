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
            //Skapa spel
        }

        [HttpGet]
        public IActionResult JoinGame()
        {
            //Gå med i spel
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
    }
}
