using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebCV.Helpers;
using WebCV.Models;

namespace WebCV.Controllers
{
    public class RecruitmentController : Controller
    {
        private readonly CvContext _cvContext;
        private readonly ILogger<CVController> _logger;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public RecruitmentController(ILogger<CVController> logger, CvContext cvContext, FileService fileService, UserManager<User> userManager)
        {
            _logger = logger;
            _cvContext = cvContext;
            _fileService = fileService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string id)
        {

            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            Job? findJob = _cvContext.Jobs.Include(j => j.Company).FirstOrDefault(j => j.Status == Enums.JobRecruiting && j.Link == id);
            if(findJob == null) {
                return NotFound("Không tim thấy công việc này");
            }
            if(findJob.Company.UserId != currentUser.Id)
            {
                return BadRequest("Bạn không sở hữu công ty đăng công việc này");
            }
            var Recruitments = _cvContext.Recruitments.Include(r => r.User).Include(r => r.Job).Where(r => r.JobId == findJob.JobId).ToList();
            return View(Recruitments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int userId, int jobId)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);
            Recruitment? recruitment = _cvContext.Recruitments.FirstOrDefault(r => r.JobId == jobId && r.UserId == userId);
            if(recruitment == null)
            {
                return NotFound("Không tìm thấy công việc này");
            }
            _cvContext.Recruitments.Remove(recruitment);
            _cvContext.SaveChanges();
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int UserId, int JobId, IFormFile image)
        {
            Job? findJob = _cvContext.Jobs.Include(j => j.Company).FirstOrDefault(j => j.JobId == JobId);
            User? owner = _cvContext.Users.FirstOrDefault(u => u.Id == findJob.Company.UserId);
            Recruitment recruitment = new Recruitment();
            recruitment.UserId = UserId;
            recruitment.JobId = JobId;
            string imgName = await _fileService.SaveUniqueFileNameAsync(image, "recruitments");
            recruitment.FileCv = imgName;
            recruitment.SendAt = DateTime.Now;
            _cvContext.Recruitments.Add(recruitment);

            Notification UserNotification = new Notification();
            UserNotification.UserId = UserId;
            UserNotification.SendAt = DateTime.Now;
            UserNotification.Hide = 0;
            UserNotification.Content = "Hồ sơ của bạn đã được nộp. Công ty sẽ phản hồi trong thời gian tới";
            _cvContext.Notifications.Add(UserNotification);

            Notification CompanyNotification = new Notification();
            CompanyNotification.UserId = owner.Id;
            CompanyNotification.SendAt = DateTime.Now;
            CompanyNotification.Hide = 0;
            CompanyNotification.Content = "Đã có người vừa gửi yêu cầu vào công ty bạn";
            _cvContext.Notifications.Add(CompanyNotification);
            _cvContext.SaveChanges();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
