using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Group1_CourseOnline.Pages.Admin.Orders
{
    public class OrderDetailsModel : PageModel
    {
        private readonly SWP391_BlueEduContext _db;

        public OrderDetailsModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public Product Products { get; set; }
        public int OrderID { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (HttpContext.Session.GetString("admin") != null)
            {
                IList<Customer> customerList = await _db.Customers.ToListAsync();
                OrderID = id;
                OrderDetails = await _db.OrderDetails.Include(c => c.Order).Include(c => c.Product).Where(o => o.OrderId == id).ToListAsync();
                foreach (var orderDetail in OrderDetails)
                {
                    var matchingCustomer = customerList.FirstOrDefault(c => c.CustomerId == orderDetail.Order.CustomerId);
                    if (matchingCustomer != null)
                    {
                        orderDetail.Order.OrderNavigation = matchingCustomer;
                    }
                }
                if (OrderDetails == null)
                {
                    return NotFound();
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