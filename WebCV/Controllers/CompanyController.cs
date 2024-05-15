using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebCV.Helpers;
using WebCV.Models;

namespace WebCV.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {

        private readonly CvContext _cvContext;
        private readonly ILogger<CompanyController> _logger;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public CompanyController(ILogger<CompanyController> logger, CvContext cvContext, FileService fileService, UserManager<User> userManager)
        {
            _logger = logger;
            _cvContext = cvContext;
            _fileService = fileService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company company, IFormFile Image)
        {
            Company? find = _cvContext.Companies.FirstOrDefault(c => c.Hide == 0 && c.Link == company.Link);
            if (find != null)
            {
                ViewBag.Error = "Link has been used";
                return View();
            }
            ViewBag.Error = null;

            string imgName = await _fileService.SaveUniqueFileNameAsync(Image, "companies");
            company.Image = imgName;
            company.Hide = 0;
            company.Status = 0;
            _cvContext.Companies.Add(company);
            Notification notification = new Notification();
            notification.UserId = (int)company.UserId;
            notification.SendAt = DateTime.Now;
            notification.Hide = 0;
            notification.Content = "Your request has been sent, please wait for it to be approved";
            _cvContext.Notifications.Add(notification);
            _cvContext.SaveChanges();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            Company? company = _cvContext.Companies.FirstOrDefault(c => c.Hide == 0 && c.Status == Enums.CompanyAccepted && c.CompanyId == id && c.UserId == currentUser.Id);
            if (company == null)
            {
                return BadRequest("May khong so huu cong ty nay");
            }
            
            return View(company);
        }

        [HttpPost]
        public async Task<ActionResult> Update(Company companyUpdate, IFormFile image) {
            Company? company = _cvContext.Companies.FirstOrDefault(c => c.CompanyId == companyUpdate.CompanyId);
            if (company == null)
            {
                return NotFound("Not found");
            }

            if (image != null)
            {
                _ = _fileService.DeleteFileAsync(company.Image, "companies");
                string imageName = await _fileService.SaveUniqueFileNameAsync(image, "companies");
                company.Image = imageName;
            }

            company.CopmpanyName = companyUpdate.CopmpanyName;
            company.Address = companyUpdate.Address;
            company.Country = companyUpdate.Country;
            company.Website = companyUpdate.Website;
            company.Hotline = companyUpdate.Hotline;
            company.Information = companyUpdate.Information;
            company.Link = companyUpdate.Link;
            _cvContext.SaveChanges();
            return RedirectToAction("Detail", "Company", new { area = "", id = company.Link });
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            Company? company = _cvContext.Companies.FirstOrDefault(c => c.Status == Enums.CompanyAccepted && c.Hide == 0 && c.Link == id);
            if(company == null)
            {
                return NotFound("Company not found");
            }
            company.Jobs = _cvContext.Jobs.Where(j => j.Status == Enums.JobRecruiting && j.EndDay > DateTime.Now && j.CompanyId == company.CompanyId).ToList(); 
            return View(company);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(string id)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            Company? company = _cvContext.Companies.FirstOrDefault(c => c.Status == Enums.CompanyAccepted && c.Hide == 0 && c.Link == id);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            if(currentUser.Id != company.UserId)
            {
                return BadRequest("Thats not your company. get out, dog!!");
            }
            return View(company);
        }
    }
}
