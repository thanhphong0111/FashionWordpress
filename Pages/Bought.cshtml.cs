using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Group1_CourseOnline.Pages.Admin
{
    public class BoughtModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public BoughtModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Customer Customers { get; set; }
        [BindProperty]
        public Employee Employees { get; set; }
        [BindProperty]
        public Account Accounts { get; set; }
        public async Task<IActionResult> OnGet()

        {
            var customer = HttpContext.Session.GetString("customer");
            var employee = HttpContext.Session.GetString("employee");

            int id = 0;
            if (customer != null)
            {
                var customerData = JsonSerializer.Deserialize<Customer>(customer);
                id = customerData.CustomerId;
                
            }
            if (employee != null)
            {
                Accounts = JsonSerializer.Deserialize<Account>(employee);
                id = (int)Accounts.EmployeeId;
                
            }
            var owmCourse = (from o in _db.Orders
                             join od in _db.OrderDetails on o.OrderId equals od.OrderId
                             join p in _db.Products on od.ProductId equals p.ProductId
                             where o.CustomerId == id
                             orderby o.OrderDate descending
                             select new
                             {
                                 p.ProductName,
                                 p.ProductId,
                                 p.Image,
                                 FirstName = p.Employee.FirstName,
                                 LastName = p.Employee.LastName,
                                 OrderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                 OrderTime = o.OrderDate.Value.Hour + ":" + o.OrderDate.Value.Minute
                             }).ToList();
            ViewData["owmCourse"] = owmCourse;
            return Page();
        }
    }
}
