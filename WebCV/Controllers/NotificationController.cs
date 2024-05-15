using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebCV.Helpers;
using WebCV.Models;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll()
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            List<Notification> notifications = await _cvContext.Notifications.Where(n => n.Hide == 0 && n.UserId == currentUser.Id).ToListAsync();

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
