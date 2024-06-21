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
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public CompanyController(ILogger<CompanyController> logger, CvContext cvContext, FileService fileService, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _logger = logger;
            _cvContext = cvContext;
            _fileService = fileService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "", int pageNumber = 1)
        {
            IQueryable<Company> companiesIQeuryable = _cvContext.Companies.Where(c => c.Status == Enums.CompanyAccepted && c.Hide == 0 && c.CopmpanyName.Contains(search));
            var companies = await PaginatedList<Company>.CreateAsync(companiesIQeuryable, pageNumber, Enums.CompanyPageSize);
            ViewBag.Search = search;
            return View(companies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company company, IFormFile Image)
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var adminUserIds = _cvContext.UserRoles
                                    .Where(ur => ur.RoleId == adminRole.Id)
                                    .Select(ur => ur.UserId)
                                    .ToList();
            var adminUsers = _cvContext.Users
                                 .Where(u => adminUserIds.Contains(u.Id))
                                 .ToList();
            Company? find = _cvContext.Companies.FirstOrDefault(c => c.Hide == 0 && c.Link == company.Link);
            
            if (find != null)
            {
                ViewBag.Error = "Link has been used";
                return View();
            }
            ViewBag.Error = null;
            string imgName = await _fileService.SaveUniqueFileNameAsync(Image, "companies");
            company.CopmpanyName = company.CopmpanyName;
            company.Image = imgName;
            company.Hide = 0;
            company.Status = 0;
            _cvContext.Companies.Add(company);
            Notification notification = new Notification();
            notification.UserId = (int)company.UserId;
            notification.SendAt = DateTime.Now;
            notification.Hide = 0;
            notification.Content = "Yêu cầu của bạn đã được gửi! Vui lòng đợi!!";
            _cvContext.Notifications.Add(notification);

            foreach (var item in adminUsers)
            {
                Notification AdminNotification = new Notification();
                AdminNotification.UserId = item.Id;
                AdminNotification.SendAt = DateTime.Now;
                AdminNotification.Link = "/Admin/Company";
                AdminNotification.Hide = 0;
                AdminNotification.Content = "Có một yêu cầu muốn trở thành công ty trên website của bạn";
                _cvContext.Notifications.Add(AdminNotification);
            }

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
                return BadRequest("Ban khong so huu cong ty nay");
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
                return BadRequest("ban không thuộc quyền sỡ hữu công ty này!!");
            }
            return View(company);
        }
    }
}
