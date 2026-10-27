using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolAppCoreMVC.Models
{
    public class CourseViewModel
    {
        public int SelectedCourseID { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }
            = new List<SelectListItem>();
    }
}