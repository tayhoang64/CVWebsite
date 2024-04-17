using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCV.Helpers;
using WebCV.Models;

namespace WebCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EvaluateController : Controller
    {
        private readonly CvContext _cvContext;
        private readonly FileService _fileService;
        

        public EvaluateController(CvContext cvContext, FileService fileService)
        {
            _cvContext = cvContext;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            List<Evaluate> evaluates = await _cvContext.Evaluates.Where(e => e.Hide == 0).ToListAsync();
            return View(evaluates); 
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Evaluate evaluate, IFormFile image)
        {
            string imageName = await _fileService.SaveUniqueFileNameAsync(image, "evaluates");
            Evaluate Eva = new Evaluate()
            {
                Content = evaluate.Content,
                Profession = evaluate.Profession,
                ClientName = evaluate.ClientName,
                Image = imageName,
                Hide = 0,
                Order = 1
            };
            _cvContext.Evaluates.Add(Eva);
            _cvContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
