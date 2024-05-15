using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebCV.Helpers;
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
            var evaluate = await _cvContext.Evaluates.Where(p => p.Hide == 0).OrderBy(p => p.Order).ToListAsync();
            var companies = await _cvContext.Companies.Where(c => c.Status == Enums.CompanyAccepted && c.Hide == 0).ToListAsync();
            var jobs = await _cvContext.Jobs.Where(j => j.Status == Enums.JobRecruiting).ToListAsync();
            var viewModel = new HomeViewModel
            {
                Sliders = slide,
                Evaluates = evaluate,
                Companies = companies,
                Jobs = jobs
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
