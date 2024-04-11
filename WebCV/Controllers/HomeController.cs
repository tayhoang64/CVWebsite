using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebCV.Models;
using WebCV.ViewModels;

namespace WebCV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CvContext _cvContext;

        public HomeController(ILogger<HomeController> logger, CvContext cvContext)
        {
            _logger = logger;
            _cvContext = cvContext;
        }
        public async Task<IActionResult> Index()
        {
            var slide = await _cvContext.Sliders.Where(p => p.Hide == 0).OrderBy(p => p.Order).ToListAsync();
            var viewModel = new HomeViewModel
            {
                Sliders = slide,
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
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
