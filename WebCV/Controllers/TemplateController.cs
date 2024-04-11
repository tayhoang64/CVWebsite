using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebCV.Helpers;
using WebCV.Models;

namespace WebCV.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TemplateController : Controller
    {
        private readonly CvContext _context; 
        private readonly IWebHostEnvironment _environment;
        private readonly FileService _fileService;

        public TemplateController(CvContext context, IWebHostEnvironment environment, FileService fileService)
        {
            _context = context;
            _environment = environment;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            List<Template> templates = await _context.Templates.Where(t => t.Hide == 0).ToListAsync();
            return View(templates);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Template template, IFormFile file, IFormFile image)
        {
            string fileName = await _fileService.SaveUniqueFileNameAsync(file, "templates");
            string imageName = await _fileService.SaveUniqueFileNameAsync(image, "templates");

            Template? find = _context.Templates.FirstOrDefault(t => t.Link == template.Link);
            if(find != null) {
                return Content("Link have been used");
            }

            Template NewTemplate = new Template()
            {
                Title = template.Title,
                File = fileName,
                CreatedAt = DateTime.Now,
                Hide = 0,
                UploadedBy = template.UploadedBy,
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
            if(template == null)
            {
                return NotFound("Can't find this template");
            }
            return View(template);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Template UpdatedTemplate, IFormFile file, IFormFile image)
        {
            Template? template = _context.Templates.FirstOrDefault(t => t.TemplateId == UpdatedTemplate.TemplateId);
            if (template == null)
            {
                return NotFound("Can't find this template");
            }

            if(file != null)
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
                return Task.FromResult<IActionResult>(NotFound("Can't find this template"));
            }

            template.Hide = 1;
            _context.SaveChanges();

            return Task.FromResult<IActionResult>(RedirectToAction("Index"));
        }

    }
}
