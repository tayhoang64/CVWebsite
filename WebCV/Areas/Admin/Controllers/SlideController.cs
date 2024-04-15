using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCV.Helpers;
using WebCV.Models;

namespace WebCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SlideController : Controller
    {
        private readonly CvContext _cvContext;
        private readonly FileService _fileService;
        public SlideController (CvContext cvContext, FileService fileService)
        {
            _cvContext = cvContext;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _cvContext.Sliders.Where(p => p.Hide == 0).ToListAsync();
            return View(sliders);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (Slider slide, IFormFile image)
        {

            string imageName = await _fileService.SaveUniqueFileNameAsync(image, "slides");
            Slider? find = _cvContext.Sliders.FirstOrDefault(t => t.Link == slide.Link);
            if (find != null)
            {
                return Content("Link have been used");
            }
            Slider slider = new Slider()
            {
                Title = slide.Title,
                Image = imageName,
                Description = slide.Description,
                Link = slide.Link,
                Hide = 0,
                Order = 1
            };
            _cvContext.Sliders.Add(slider);
            _cvContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]

        public IActionResult Edit(int Id)
        {
            Slider? slider = _cvContext.Sliders.FirstOrDefault(p => p.SlideID == Id);
            if(slider == null)
            {
                return NotFound("Can't find slide");
            }
            return View(slider);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Slider updateSlider, IFormFile img)
        {
            Slider? slider = _cvContext.Sliders.FirstOrDefault(p => p.SlideID == updateSlider.SlideID);
            if(slider == null)
            {
                return NotFound("Not found");
            }
            if (img != null)
            {
                _ = _fileService.DeleteFileAsync(slider.Image, "slides");
                string imageName = await _fileService.SaveUniqueFileNameAsync(img, "slides");
                slider.Image = imageName;
            }
            slider.Title = updateSlider.Title;
            slider.Description = updateSlider.Description;
            slider.Link = updateSlider.Link;
            slider.Order = updateSlider.Order;

            _cvContext?.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Slider? slider = _cvContext.Sliders.FirstOrDefault(p => p.SlideID==Id);
            if( slider == null) 
            {
                return NotFound("Not found");
            }
            return View(slider);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(Slider deleteSlider)
        {
            Slider? slider = _cvContext.Sliders.FirstOrDefault(p => p.SlideID == deleteSlider.SlideID);
            if (slider == null)
            {
                return NotFound("Not found");
            }
            slider.Hide = 1;
            _cvContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
