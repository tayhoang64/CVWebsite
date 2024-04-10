using Microsoft.AspNetCore.Mvc;

namespace WebCV.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
