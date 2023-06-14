using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages.Admin.Orders
{
    public class IndexModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;

        public IndexModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        [BindProperty]
        public Customer Customers { get; set; }
        public string CustomerID { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("admin") != null)
            {
                IList<Customer> customerList = await _db.Customers.ToListAsync();

                Orders = await _db.Orders.Include(c => c.OrderDetails).ToListAsync();
                for (int i = 0; i < Orders.Count; i++)
                {
                    for (int j = 0; j < customerList.Count; j++)
                    {
                        if (Orders[i].CustomerId == customerList[j].CustomerId)
                        {
                            Orders[i].OrderNavigation = customerList[j];
                        }
                    }
                }
                
            }
            else
            {
                return NotFound();
            }
            return Page();
        }
    }
}
