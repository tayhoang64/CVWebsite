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

            List<Cv> cvs = await _cvContext.Cvs.Include(c => c.Template).Where(c => c.UserId == currentUser.Id && c.Hide == 0).ToListAsync();
            return View(cvs);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Template> templates = await _cvContext.Templates.Where(p => p.Hide == 0).ToListAsync(); 
            return View(templates);
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
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            Cv? cv = _cvContext.Cvs.FirstOrDefault(c => c.CvId == id && c.UserId == currentUser.Id);
            if(cv == null)
            {
                return NotFound("You dont have permision to modify this CV");
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
                Hide = 0,
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

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Cv? cv = _cvContext.Cvs.FirstOrDefault(c => c.CvId == id);
            if (cv == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            cv.Hide = 1;
            await _cvContext.SaveChangesAsync();
            return RedirectToAction("Saved", "CV", new { area = "" });
        }
    }
}
