using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCV.Helpers;
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
        public async Task<IActionResult> Index(string search = "", int pageNumber = 1)
        {
            IQueryable<Template> templatesIQeuryable = _cvContext.Templates.Where(t => t.Hide == 0 && t.Title.Contains(search));
            var paginatedTemplates = await PaginatedList<Template>.CreateAsync(templatesIQeuryable, pageNumber, Enums.PageSize);
            ViewBag.Search = search;
            return View(paginatedTemplates);
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
