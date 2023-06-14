using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Group1_CourseOnline.Pages
{

    public class UserProfileModel : PageModel
    {




        private readonly SWP391_BlueEduContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public UserProfileModel(SWP391_BlueEduContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public string ImagePath { get; set; }

        [BindProperty]
        public Customer Customers { get; set; }
        [BindProperty]
        public Employee Employees { get; set; }
        [BindProperty]
        public Account Accounts { get; set; }

        public void OnGet()
        {
            var customer = HttpContext.Session.GetString("customer");
            var employee = HttpContext.Session.GetString("employee");

            int id = 0;
            if (customer != null)
            {
                var customerData = JsonSerializer.Deserialize<Customer>(customer);
                id = customerData.CustomerId;
                Accounts = _db.Accounts.SingleOrDefault(e => e.CustomerId == id);
                Customers = _db.Customers.Where(e => e.CustomerId == id).FirstOrDefault();
            }
            if (employee != null)
            {
                Accounts = JsonSerializer.Deserialize<Account>(employee);
                id = (int)Accounts.EmployeeId;
                Employees = _db.Employees.Where(e => e.EmployeeId == id).FirstOrDefault();
            }
        }

        public async Task<IActionResult> OnPostAsync(IFormFile imageFile)
        {
            var jsonString = HttpContext.Session.GetString("customer");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var myObject = JsonSerializer.Deserialize<Account>(jsonString, options);
            Accounts = myObject as Account;

            int id = (int)Accounts.CustomerId;

            Customer customer = _db.Customers.FirstOrDefault(e => e.CustomerId == id);

            if (customer == null)
            {
                ViewData["fail"] = "Customer not found.";
                return Page();
            }

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
                var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", "Customer", imageFile.FileName);
                var oldImgPath = Path.Combine(_hostingEnvironment.WebRootPath, customer.Avatar ?? string.Empty);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                try
                {
                    if (customer.Avatar != ("/img/Customer/" + imageFile.FileName) && System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.SetAttributes(oldImgPath, FileAttributes.Normal);
                        System.IO.File.Delete(oldImgPath);
                    }
                }
                catch (IOException ex)
                {
                    ViewData["fail"] = ex.Message;
                    return Page();
                }

                customer.Avatar = "/img/Customer/" + imageFile.FileName;
            }

            try
            {
                customer.Phone = Customers.Phone;
                customer.LastName = Customers.LastName;
                customer.FirstName = Customers.FirstName;
                customer.BirthDate = Customers.BirthDate;
                customer.Address = Customers.Address;

                _db.Customers.Update(customer);
                await _db.SaveChangesAsync();

                ViewData["success"] = "Update Profile Successful!";
            }
            catch (Exception e)
            {
                ViewData["fail"] = e.Message;
                return Page();
            }
            OnGet();
            return Page();
        }



    }
}
