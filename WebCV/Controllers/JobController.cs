using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebCV.Helpers;
using WebCV.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebCV.Controllers
{
    public class JobController : Controller
    {

        private readonly CvContext _cvContext;
        private readonly ILogger<CVController> _logger;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public JobController(ILogger<CVController> logger, CvContext cvContext, FileService fileService, UserManager<User> userManager)
        {
            _logger = logger;
            _cvContext = cvContext;
            _fileService = fileService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            List<Company> companies = await _cvContext.Companies.Where(c => c.Hide == 0 && c.Status == Enums.CompanyAccepted && c.UserId == currentUser.Id).ToListAsync();
            if(companies.Count == 0)
            {
                return BadRequest("You dont have any company yet!!!");
            }
            return View(companies);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            List<Company> companies = await _cvContext.Companies.Where(c => c.Hide == 0 && c.Status == Enums.CompanyAccepted && c.UserId == currentUser.Id).ToListAsync();
            if (companies.Count == 0)
            {
                return BadRequest("You don't have any company yet!!!");
            }
            Company? company = _cvContext.Companies.FirstOrDefault(c => c.Hide == 0 && c.Status == Enums.CompanyAccepted && c.Link == id);
            if(company == null)
            {
                return NotFound("Not Found");
            }

            List<Category> categories = await _cvContext.Categories.ToListAsync();
            List<Level> levels = await _cvContext.Levels.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.Levels = levels;
            ViewBag.Company = company;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string id, Job job, string skills)
        {
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            List<Company> companies = await _cvContext.Companies.Where(c => c.Hide == 0 && c.Status == Enums.CompanyAccepted && c.UserId == currentUser.Id).ToListAsync();
            if (companies.Count == 0)
            {
                ViewBag.Error = "You don't have any company yet!!!";
                return View();
            }
            Company? company = _cvContext.Companies.FirstOrDefault(c => c.Hide == 0 && c.Status == Enums.CompanyAccepted && c.Link == id);
            if (company == null)
            {
                ViewBag.Error = "Not Found";
                return View();
            }

            Job newJob = new Job();
            newJob.JobName = job.JobName;
            newJob.Jd = job.Jd;
            newJob.CategoryId = job.CategoryId;
            newJob.LevelId = job.LevelId;
            newJob.Status = Enums.JobRecruiting;
            newJob.CompanyId = job.CompanyId;
            newJob.ExperienceYear = job.ExperienceYear;
            newJob.UpdateDay = DateTime.Now;
            newJob.EndDay = job.EndDay;
            newJob.RecruitmentCount = job.RecruitmentCount;
            newJob.Salary = job.Salary;
            newJob.Link = job.Link;

            _cvContext.Jobs.Add(newJob);
            await _cvContext.SaveChangesAsync();

            string[] SkillRequired = skills.Split(",");
            foreach (string skill in SkillRequired)
            {
                Skill newSkill = new Skill();
                newSkill.SkillName = skill;
                newSkill.JobId = newJob.JobId;
                _cvContext.Skills.Add(newSkill);
            }
            await _cvContext.SaveChangesAsync();

            ViewBag.Success = "Job has been posted";
            List<Category> categories = await _cvContext.Categories.ToListAsync();
            List<Level> levels = await _cvContext.Levels.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.Levels = levels;
            ViewBag.Company = company;
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            Job? job = _cvContext.Jobs.FirstOrDefault(j => j.JobId == jobId);
            if(job == null)
            {
                return NotFound("Khong tim thay job");
            }
            ClaimsPrincipal user = HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);
            if(currentUser.Id != job.Company.UserId)
            {
                return BadRequest("Bạn không sở hữu công ty này");
            }
            _cvContext.Jobs.Remove(job);
            return RedirectToAction("Index", "Job", new { area = "" });
        }


        [HttpGet]
        public async Task<IActionResult> JobList(int CategoryId = 0, string search = "", string LevelName = "", int pageNumber = 1)
        {
            List<Category> categories = _cvContext.Categories.ToList();
            List<Level> levels = _cvContext.Levels.ToList();
            IQueryable<Job> jobsIQeuryable = CategoryId == 0 ? _cvContext.Jobs.Include(j => j.Company).Include(j => j.Level).Where(j => (j.Status == Enums.JobRecruiting || j.Status == Enums.JobFull) && j.JobName.Contains(search) && j.Level.LevelName.Contains(LevelName)) : _cvContext.Jobs.Include(j => j.Company).Include(j => j.Level).Where(j => (j.Status == Enums.JobRecruiting || j.Status == Enums.JobFull) && j.JobName.Contains(search) && j.CategoryId == CategoryId && j.Level.LevelName.Contains(LevelName));
            var paginatedJobs = await PaginatedList<Job>.CreateAsync(jobsIQeuryable, pageNumber, Enums.JobPageSize);
            ViewBag.Search = search;
            ViewBag.Categories = categories;
            ViewBag.Levels = levels;
            return View(paginatedJobs);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            Job? job = await _cvContext.Jobs.Include(j => j.Company).Include(j => j.Level).Include(j => j.Skills).FirstOrDefaultAsync(j => j.Status != Enums.JobDeleted && j.Link == id);

            if (job == null)
            {
                return NotFound("Khong tim thay Cong Viec");
            }

            List<Job> jobs = await _cvContext.Jobs.Include(j => j.Company).Include(j => j.Level).Include(j => j.Skills).Where(j => j.Status == Enums.JobRecruiting && j.Link != id).ToListAsync();
            jobs = jobs.Where(j => j.CategoryId == job.CategoryId || j.LevelId == job.LevelId || j.CompanyId == job.CompanyId).ToList();

            ViewBag.Jobs = jobs;
            return View(job);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteJob(Job deleteJob)
        {
            Job? job = await _cvContext.Jobs.Include(j => j.Company).Include(j => j.Level).Include(j => j.Skills).FirstOrDefaultAsync(j => j.Status == Enums.JobRecruiting && j.JobId == deleteJob.JobId);

            if (job == null)
            {
                return NotFound("Khong tim thay Cong Viec");
            }
            job.Status = Enums.JobDeleted;
            Notification notification = new Notification();
            notification.UserId = (int)job.Company.UserId;
            notification.SendAt = DateTime.Now;
            notification.Hide = 0;
            notification.Content = "Công ty của bạn đã vi phạm điều khoản của chúng tôi, tạm thời đã bị vô hiệu hóa, nếu bạn thấy không ổn vui lòng liện hệ với chúng tôi";
            _cvContext.Notifications.Add(notification);
            _cvContext.SaveChanges();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetJobFull(Job jobFull)
        {
            Job? job = await _cvContext.Jobs.Include(j => j.Company).Include(j => j.Level).Include(j => j.Skills).FirstOrDefaultAsync(j => j.Status == Enums.JobRecruiting && j.JobId == jobFull.JobId);

            if (job == null)
            {
                return NotFound("Khong tim thay Cong Viec");
            }
            job.Status = Enums.JobFull;
            _cvContext.SaveChanges();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        
    }
}
