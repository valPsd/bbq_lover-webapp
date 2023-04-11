using BBQLover.webapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BBQLover.webapp.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel userModel)
        {
            if (userModel.Email == "admin@bbqlover.com" && userModel.Password == "bbq123456")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.showAlert = true;
                ViewBag.alertMessage = "Incorrect Email or password";
                return View();
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}