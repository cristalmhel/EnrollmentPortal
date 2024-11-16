using EnrollmentPortal.Data;
using EnrollmentPortal.Models;
using EnrollmentPortal.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EnrollmentPortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            int courseCount = _context.Courses.Where(c => c.Status == "Active").Count();
            int studentCount = _context.StudentFiles.Where(s => s.STFSTUDSTATUS == "Active").Count();
            int subjectCount = _context.SubjectFiles.Where(s => s.SFSUBJSTATUS == "Active").Count();
            int enrolledCount = _context.EnrollmentHeaderFiles.Where(s => s.ENRHFSTUDSTATUS == "Enrolled").Count();

            var homeViewModel = new HomeViewModel 
            {
                TotalCourse = courseCount,
                TotalStudents = studentCount,
                TotalSubjects = subjectCount,
                TotalEnrollees = enrolledCount
            };

            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
