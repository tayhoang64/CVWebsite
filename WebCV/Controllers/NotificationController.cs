using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebCV.Helpers;
using WebCV.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebCV.Controllers
{
    public class NotificationController : Controller
    {
        private readonly CvContext _cvContext;
        private readonly ILogger<CVController> _logger;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public NotificationController(ILogger<CVController> logger, CvContext cvContext, FileService fileService, UserManager<User> userManager)
        {
            _logger = logger;
            _cvContext = cvContext;
            _fileService = fileService;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMail(int UserId, string Content)
        {
            Notification notification = new Notification();
            notification.UserId = UserId;
            notification.Content = Content;
            notification.SendAt = DateTime.Now;
            notification.Hide = 0;
            _cvContext.Notifications.Add(notification);
            _cvContext.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll()
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            List<Notification> notifications = await _cvContext.Notifications.Where(n => n.Hide == 0 && n.UserId == currentUser.Id).OrderByDescending(n => n.SendAt).ToListAsync();

            return Json(JsonHelper.ToJson<List<Notification>>(notifications));
        }

        public async Task<IActionResult> Delete(int id)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            Notification notification = _cvContext.Notifications.FirstOrDefault(n => n.Hide == 0 && n.NotificationId == id);
            if(notification.UserId != currentUser.Id)
            {
                return Json(new
                {
                    error = "Can't delete another notification",
                });
            }
            notification.Hide = 1;
            _cvContext.SaveChanges();

            List<Notification> notifications = await _cvContext.Notifications.Where(n => n.Hide == 0 && n.UserId == currentUser.Id).ToListAsync();

            return Json(JsonHelper.ToJson<List<Notification>>(notifications));
        }
    }
}
