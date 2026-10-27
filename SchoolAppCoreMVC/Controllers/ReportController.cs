using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SchoolAppCoreMVC.Models;

namespace SchoolAppCoreMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly SchoolContext _context;
        private readonly IWebHostEnvironment _environment;

        public ReportController(
            SchoolContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var model = new CourseViewModel
            {
                Courses = new SelectList(
                    _context.Courses,
                    "CourseID",
                    "Title")
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Generate(int selectedCourseID, string actionType)
        {
            var reportPath = Path.Combine(
                _environment.ContentRootPath,
                "Reports",
                "StudentReport.rdlc");

            using var reportStream = System.IO.File.OpenRead(reportPath);

            var localReport = new LocalReport();
            localReport.LoadReportDefinition(reportStream);

            var students = (
                from e in _context.Enrollments
                join s in _context.Students
                    on e.StudentID equals s.StudentID
                where e.CourseID == selectedCourseID
                select new
                {
                    s.StudentID,
                    s.FirstName,
                    s.LastName,
                    e.Grade
                }
            ).ToList();

            localReport.DataSources.Add(
                new ReportDataSource("StudentDataSet", students));

            var reportBytes = localReport.Render("PDF");

            if (actionType == "preview")
            {
                return File(reportBytes, "application/pdf");
            }

            return File(
                reportBytes,
                "application/pdf",
                "StudentReport.pdf");
        }
    }
}