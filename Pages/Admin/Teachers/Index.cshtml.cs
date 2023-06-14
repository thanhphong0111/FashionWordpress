using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Group1_CourseOnline.Models;
using System.Security.Cryptography;
using System.Text;

namespace Group1_CourseOnline.Pages.Admin.Teachers
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public readonly SWP391_BlueEduContext _db;
        public IndexModel(SWP391_BlueEduContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }
        [BindProperty]
        public Account Account { get; set; }
        [BindProperty]
        public Employee Employee { get; set; }
        public int id { get; set; }
        [BindProperty]
        public List<Employee> Employees { get; set; }


        public IActionResult OnGet(int Id)
        {

            id = Id;

            Employee = _db.Employees.Where(e => e.EmployeeId == Id).FirstOrDefault();

            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            Employees = _db.Employees.Include(e => e.Department).Where(e => e.DepartmentId == 1).ToList();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int employeeId, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(imageFile.FileName);
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ViewData["fail"] = "Invalid file format. Only JPG and PNG files are allowed.";
                        return Page();
                    }

                    // Kiểm tra kích thước tệp tin
                    var maxSize = 2 * 1024 * 1024; // 2MB
                    if (imageFile.Length > maxSize)
                    {
                        ViewData["fail"] = "File size exceeds the limit. Maximum file size is 2MB.";
                        return Page();
                    }
                    // Xử lý tệp tin
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", "Teacher", imageFile.FileName);


                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                }

                if (employeeId == 0)
                {
                    Employee emp = new Employee
                    {

                        FirstName = Employee.FirstName,
                        LastName = Employee.LastName,
                        Phone = Employee.Phone,
                        Title = Employee.Title,
                        TitleOfCourtesy = Employee.TitleOfCourtesy,
                        HireDate = Employee.HireDate,
                        Avatar = "/img/Teacher/" + imageFile.FileName,
                        DepartmentId = 1,
                        Status = true,
                    };
                    _db.Employees.Add(emp);
                    _db.SaveChanges();
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] inputBytes = Encoding.ASCII.GetBytes("@Teacher123");
                        byte[] hashBytes = md5.ComputeHash(inputBytes);
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < hashBytes.Length; i++)
                        {
                            sb.Append(hashBytes[i].ToString("X2"));
                        }

                        Account ac = new Account
                        {
                            Email = Account.Email,
                            Password = sb.ToString(),
                            CreateDate = DateTime.Now,
                            Role = 1,
                            EmployeeId = emp.EmployeeId,
                            IsSocialAccount = false,
                        };
                        _db.Accounts.Add(ac);
                        _db.SaveChanges();
                    }

                    MessageBox("Success", "Create a successful course catalog.", "alert-success");
                }
                else
                {
                    var emp = _db.Employees.FirstOrDefault(c => c.EmployeeId == employeeId);
                    if (emp != null)
                    {
                        emp.FirstName = Employee.FirstName;
                        emp.LastName = Employee.LastName;
                        emp.Phone = Employee.Phone;
                        emp.Title = Employee.Title;
                        emp.Avatar = "/img/Teacher/" + imageFile.FileName;
                        emp.TitleOfCourtesy = Employee.TitleOfCourtesy;
                        emp.HireDate = Employee.HireDate;
                        _db.SaveChanges();
                        MessageBox("Success", "Update the course catalog successfully.", "alert-success");
                    }
                }

                return RedirectToPage("/Admin/Teachers/Index");


            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the process
                MessageBox("Error", "An error occurred while processing the request.", "alert-danger");
            }

            return Page();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostLockEmployeesAsync(List<int> selectedProductIds)
        {
            var categoriesToDelete = await _db.Employees
                .Where(c => selectedProductIds.Contains(c.EmployeeId))
                .ToListAsync();

            if (categoriesToDelete.Count == 0)
            {
                MessageBox("Warning", "Please select the employee you want to delete.", "alert-warning");
                return RedirectToPage("/Admin/teachers/Index");
            }

            try
            {
                foreach (var category in categoriesToDelete)
                {
                    category.Status = !category.Status;
                    _db.Employees.Update(category);
                }

                await _db.SaveChangesAsync();

                MessageBox("Success", "Change status successfully.", "alert-success");
            }
            catch (Exception ex)
            {
                MessageBox("Error", "An error occurred while change status.", "alert-danger");
            }
            return RedirectToPage("/Admin/teachers/Index");
        }

        public void MessageBox(string Notification, string AlertMessage, string AlertType)
        {
            TempData["Notification"] = Notification;
            TempData["AlertMessage"] = AlertMessage;
            TempData["AlertType"] = AlertType;
        }
    }
}
