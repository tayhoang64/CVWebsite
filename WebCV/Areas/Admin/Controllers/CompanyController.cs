﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebCV.Helpers;
using WebCV.Models;

namespace WebCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {

        private readonly CvContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public CompanyController(CvContext context, IWebHostEnvironment environment, FileService fileService, UserManager<User> userManager)
        {
            _context = context;
            _environment = environment;
            _fileService = fileService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<Company> companies = _context.Companies.Where(c => c.Status == Enums.CompanyPending).ToList();
            return View(companies);
        }

        public IActionResult ListAccepted()
        {
            List<Company> companies = _context.Companies.Where(c => c.Status == Enums.CompanyAccepted && c.Hide == 0).ToList();
            return View(companies);
        }
       
        public IActionResult Accept(int id)
        {
            
                Company company = _context.Companies.FirstOrDefault(c => c.CompanyId == id);
                if (company == null)
                {
                    ViewBag.Error = "Công ty không được tìm thấy hoặc đã bị từ chối!";
                    return RedirectToAction("Index");
                }
                company.Status = Enums.CompanyAccepted;
                
                Notification notification = new Notification();
                notification.UserId = (int)company.UserId;
                notification.SendAt = DateTime.Now;
                notification.Hide = 0;
                notification.Link = "/Company/Detail/" + company.Link;
                notification.Content = "Yêu cầu trở thành công ty của bạn đang đợi Admin xác nhận! Nhấn để xem chi tiết.";
                _context.Notifications.Add(notification);

                _context.SaveChanges();
                ViewBag.SuccessMessage = "Đã chấp nhận công ty này!";
            
            
            return RedirectToAction("Index");
        }

        public IActionResult Reject(int id)
        {
            
                Company company = _context.Companies.FirstOrDefault(c => c.CompanyId == id);
                if (company == null)
                {
                    ViewBag.Error = "Company Not Found";
                    return RedirectToAction("Index");
                }
                company.Status = Enums.CompanyRejected;

                Notification notification = new Notification();
                notification.UserId = (int)company.UserId;
                notification.SendAt = DateTime.Now;
                notification.Hide = 0;
                notification.Content = "Yêu cầu của bạn đã bị từ chối";
                _context.Notifications.Add(notification);

            _context.SaveChanges();
                ViewBag.SuccessMessage = "Đã từ chối thành công!";
            
            
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Company? company = _context.Companies.FirstOrDefault(c => c.CompanyId == Id);
            if (company == null)
            {
                return NotFound("Not found");
            }
            return View(company);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(Company deleteCompany)
        {
            Company? company = _context.Companies.FirstOrDefault(c => c.CompanyId == deleteCompany.CompanyId);
            if (company == null)
            {
                return NotFound("Not found");
            }
            company.Hide = 1;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
