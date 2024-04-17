using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Claims;
using WebCV.Helpers;
using WebCV.Models;
using WebCV.ViewModels;

namespace WebCV.Controllers
{
    public class UserController : Controller
    {
        private readonly CvContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(CvContext context, IWebHostEnvironment environment, FileService fileService, UserManager<User> userManager)
        {
            _context = context;
            _environment = environment;
            _fileService = fileService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserProfile()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(IFormFile avatar)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Use ClaimTypes.NameIdentifier to get the user's unique identifier
            User user = _context.Users.FirstOrDefault(u => u.Id.ToString() == currentUserId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.Avatar != null)
            {
                await _fileService.DeleteFileAsync(user.Avatar, "avatars");
            }

            string newName = await _fileService.SaveUniqueFileNameAsync(avatar, "avatars");
            user.Avatar = newName;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult EditProfile()
        {
            // Fetch the current user's profile details and pass it to the view
            var currentUser = _userManager.GetUserAsync(User).Result;
            return View(currentUser);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User model)
        {
            // Update the user's profile details and save changes to the database
            var currentUser = await _userManager.GetUserAsync(User);
            currentUser.FullName = model.FullName;
            currentUser.Gender = model.Gender;
            currentUser.Address = model.Address;
            currentUser.Email = model.Email;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.Link = model.Link;

            // Save the updated user profile
            await _userManager.UpdateAsync(currentUser);

            return RedirectToAction("UserProfile");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Log in");
                }
                var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
                if(!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                
                return View("ChangePasswordConfirmation");
            }
            return View(changePassword);
        }

    }
}
