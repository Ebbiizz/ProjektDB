using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjektDB.Models;

namespace ProjektDB.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Users user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                ViewBag.ErrorMessage = "Användarnamn och lösenord krävs.";
                return View();
            }

            string hashedPassword = HashPassword(user.Password);

            UsersMethods usersMethods = new UsersMethods();
            Users foundUser = usersMethods.GetUser(user.Username, hashedPassword, out string error);

            if (foundUser == null)
            {
                ViewBag.ErrorMessage = error ?? "Fel användarnamn eller lösenord.";
                return View();
            }

            HttpContext.Session.SetString("Username", foundUser.Username);
            HttpContext.Session.SetInt32("UserId", foundUser.Id);

            return RedirectToAction("Lobby", "Game");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Users user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                ViewBag.ErrorMessage = "Både användarnamn och lösenord krävs.";
                return View();
            }

            UsersMethods usersMethods = new UsersMethods();

            if (usersMethods.IsUsernameTaken(user.Username, out string error))
            {
                if (error.IsNullOrEmpty())
                {
                    ViewBag.ErrorMessage = error ?? "Användarnamnet är upptaget.";
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = error ?? "Användarnamnet kunde inte skapas.";
                    return View();
                }
            }

            string hashedPassword = HashPassword(user.Password);

            Users newUser = new Users
            {
                Username = user.Username,
                Password = hashedPassword
            };

            int success = usersMethods.InsertUser(newUser, out string insertError);

            if (success == 1)
            {
                ViewBag.SuccessMessage = "Du är registrerad! Logga in nedan.";
                return View("Login");
            }

            ViewBag.ErrorMessage = insertError ?? "Ett fel uppstod vid registreringen. Prova igen.";
            return View();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
