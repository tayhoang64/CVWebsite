using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using WebCV.Helpers;
using WebCV.Migrations;
using WebCV.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebCV.Controllers
{
    public class CVController : Controller
    {
        private readonly CvContext _cvContext;
        private readonly ILogger<CVController> _logger;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public CVController(ILogger<CVController> logger, CvContext cvContext, FileService fileService, UserManager<User> userManager)
        {
            _logger = logger;
            _cvContext = cvContext;
            _fileService = fileService;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Saved()
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            List<Cv> cvs = await _cvContext.Cvs.Include(c => c.Template).Where(c => c.UserId == currentUser.Id).ToListAsync();
            return View(cvs);
        }

        public async Task<IActionResult> Index(string search = "", int pageNumber = 1)
        {
            IQueryable<Template> templatesIQeuryable = _cvContext.Templates.Where(t => t.Hide == 0 && t.Title.Contains(search));
            var paginatedTemplates = await PaginatedList<Template>.CreateAsync(templatesIQeuryable, pageNumber, Enums.PageSize);
            ViewBag.Search = search;
            return View(paginatedTemplates);
        }
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            Template? template = _cvContext.Templates.FirstOrDefault(p => p.Link.ToLower() == id.ToLower());
            if (template == null)
            {
                return Content("Error when getting template");
            }

            return View(template);
        }
        [Authorize]
        public async Task<IActionResult> EditCV(int id)
        {
            Cv? cv = _cvContext.Cvs.FirstOrDefault(c => c.CvId == id);
            if(cv == null)
            {
                return NotFound("Can not found this CV");
            }
            return View(cv);
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage(IFormFile? image)
        {
            string name = await _fileService.SaveUniqueFileNameAsync(image, "cvs");
            return Json(new
            {
                image = name,
            });
        }   

        [HttpPost]
        public async Task<IActionResult> SaveCV(string htmlBody, int TemplateId)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            string filename = await _fileService.SaveFileWithExtension("html", "cvs", htmlBody);
            Cv cv = new Cv()
            {
                File = filename,
                LastUpdatedAt = DateTime.Now,
                UserId = currentUser.Id,
                TemplateId = TemplateId,
            };
            _cvContext.Cvs.Add(cv);
            _cvContext.SaveChanges();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveEditedCV(string htmlBody, int CvId)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            string filename = await _fileService.SaveFileWithExtension("html", "cvs", htmlBody);
            Cv? cv = _cvContext.Cvs.FirstOrDefault(c => c.CvId == CvId);
            if(cv == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            cv.LastUpdatedAt = DateTime.Now;
            cv.File = filename;
            await _cvContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
