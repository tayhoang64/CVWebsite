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

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Evaluate? find = _cvContext.Evaluates.FirstOrDefault(f => f.EvaluateId == Id);
            return View(find);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(Evaluate evaluateUpdate, IFormFile img)
        {
            Evaluate? evaluate = _cvContext.Evaluates.FirstOrDefault(p => p.EvaluateId == evaluateUpdate.EvaluateId);
            if(evaluate == null) 
            {
                return NotFound("Not found!");
            }
            if (img != null)
            {
                _ = _fileService.DeleteFileAsync(evaluate.Image, "evaluates");
                string imageName = await _fileService.SaveUniqueFileNameAsync(img, "evaluates");
                evaluate.Image = imageName;
            }
            evaluate.ClientName = evaluateUpdate.ClientName;
            evaluate.Content = evaluateUpdate.Content;
            evaluate.Profession = evaluateUpdate.Profession;
            evaluate.Order = evaluateUpdate.Order;
            _cvContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Evaluate? evaluate = _cvContext.Evaluates.FirstOrDefault(p => p.EvaluateId == Id);
            if (evaluate == null)
            {
                return NotFound("Not found");
            }
            return View(evaluate);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(Evaluate deleteEvaluate)
        {
            Evaluate? evaluate = _cvContext.Evaluates.FirstOrDefault(p => p.EvaluateId == deleteEvaluate.EvaluateId);
            if (evaluate == null)
            {
                return NotFound("Not found");
            }
            evaluate.Hide = 1;
            _cvContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
