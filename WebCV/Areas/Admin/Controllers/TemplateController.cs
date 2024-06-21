using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using WebCV.Helpers;
using WebCV.Models;


namespace WebCV.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class TemplateController : Controller
    {
        private readonly CvContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public TemplateController(CvContext context, IWebHostEnvironment environment, FileService fileService, UserManager<User> userManager)
        {
            _context = context;
            _environment = environment;
            _fileService = fileService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search = "", int pageNumber = 1)
        {
            IQueryable<Template> templatesIQeuryable = _context.Templates.Where(t => t.Hide == 0 && t.Title.Contains(search));
            var paginatedTemplates = await PaginatedList<Template>.CreateAsync(templatesIQeuryable, pageNumber, Enums.PageSize);
            ViewBag.Search = search;
            return View(paginatedTemplates);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Template template, IFormFile file, IFormFile image)
        {

            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);
            string fileName = await _fileService.SaveUniqueFileNameAsync(file, "templates");
            string imageName = await _fileService.SaveUniqueFileNameAsync(image, "templates");

            Template? find = _context.Templates.FirstOrDefault(t => t.Link == template.Link);
            if (find != null)
            {
                return Content("Link đã được sử dụng");
            }

            Template NewTemplate = new Template()
            {
                Title = template.Title,
                File = fileName,
                CreatedAt = DateTime.Now,
                Hide = 0,
                UploadedBy = currentUser.FullName,
                LastUpdatedAt = DateTime.Now,
                Link = template.Link,
                Image = imageName,
            };
            _context.Templates.Add(NewTemplate);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Template? template = _context.Templates.FirstOrDefault(t => t.TemplateId == id);
            if (template == null)
            {
                return NotFound("Không thể tìm thấy template này!");
            }
            return View(template);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Template UpdatedTemplate, IFormFile file, IFormFile image)
        {
            Template? template = _context.Templates.FirstOrDefault(t => t.TemplateId == UpdatedTemplate.TemplateId);
            if (template == null)
            {
                return NotFound("Không thể tìm thấy template này!");
            }

            if (file != null)
            {
                _ = _fileService.DeleteFileAsync(template.File, "templates");
                string fileName = await _fileService.SaveUniqueFileNameAsync(file, "templates");
                template.File = fileName;
            }

            if (image != null)
            {
                _ = _fileService.DeleteFileAsync(template.Image, "templates");
                string imageName = await _fileService.SaveUniqueFileNameAsync(image, "templates");
                template.Image = imageName;
            }

            template.Title = UpdatedTemplate.Title;
            template.Link = UpdatedTemplate.Link;
            template.LastUpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Template? template = _context.Templates.FirstOrDefault(t => t.TemplateId == id);

            return View(template);
        }

        [HttpPost]
        public Task<IActionResult> Delete(Template deleteTemplate)
        {
            Template? template = _context.Templates.FirstOrDefault(t => t.TemplateId == deleteTemplate.TemplateId);
            if (template == null)
            {
                return Task.FromResult<IActionResult>(NotFound("Không thể tìm thấy template này!"));
            }

            template.Hide = 1;
            _context.SaveChanges();

            return Task.FromResult<IActionResult>(RedirectToAction("Index"));
        }

    }
}
