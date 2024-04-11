using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCV.Migrations;
using WebCV.Models;

namespace WebCV.Controllers
{
    public class CVController : Controller
    {
        private readonly CvContext _cvContext;
        private readonly ILogger<CVController> _logger;

        public CVController(ILogger<CVController> logger, CvContext cvContext)
        {
            _logger = logger;
            _cvContext = cvContext;
        }
        public async Task<IActionResult> Index()
        {
            List<Template> templates = await _cvContext.Templates.Where(p => p.Hide == 0).ToListAsync(); 
            return View(templates);
        }

        public async Task<IActionResult> Edit(string link)
        {
            Template? template = _cvContext.Templates.FirstOrDefault(p => p.Link.ToLower() == link.ToLower());
            if (template == null)
            {
                return Content("Error when getting template");
            }

            return View(template);
        }


    }
}
