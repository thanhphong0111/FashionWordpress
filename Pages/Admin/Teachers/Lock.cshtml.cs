using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages.Admin.Teachers
{
    public class LockModel : PageModel
    {

        public readonly SWP391_BlueEduContext _db;
        public LockModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Employee Employee { get; set; }
        public int id { get; set; }
        [BindProperty]
        public List<Employee> Employees { get; set; }
       
        public IActionResult OnGet(int Id, bool status)
        {
            Employee = _db.Employees.Where(e => e.EmployeeId == Id).FirstOrDefault();
            if (status != null && Id != 0)
            {
                if (status == true)
                {
                    Employee.Status = status;
                    MessageBox("Success", "You have successfully unlocked the user account.", "alert-success");
                }
                else
                {
                    Employee.Status = status;
                    MessageBox("Success", "You have successfully locked the user account.", "alert-success");
                }

                _db.Employees.Update(Employee);
                _db.SaveChanges();
            }
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            Employees = _db.Employees.Include(e => e.Department).Where(e => e.DepartmentId == 1).ToList();
            return  RedirectToPage("index");
        }
        public void MessageBox(string Notification, string AlertMessage, string AlertType)
        {
            TempData["Notification"] = Notification;
            TempData["AlertMessage"] = AlertMessage;
            TempData["AlertType"] = AlertType;
        }
    }
}
