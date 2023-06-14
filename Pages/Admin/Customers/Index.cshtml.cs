using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Group1_CourseOnline.Pages.Admin.Customers
{


    public class IndexModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public IndexModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]

        public List<Customer> Customers { get; set; }
      
        public async Task<IActionResult> OnGet(int id, bool status)
        {
            if (HttpContext.Session.GetString("admin") == null)
            {
                return RedirectToPage("/Login");
            }
            string returnUrl = Request.Headers["Referer"];

            if (id!=null && status != null)
            {
                Customer customer = _db.Customers.Find(id);
                if (customer != null)
                {
                    if(status== true)
                    {
                        customer.Status = status;
                        MessageBox("Success", "You have successfully unlocked the user account.", "alert-success");
                    }
                    else
                    {
                        customer.Status = status;
                        MessageBox("Success", "You have successfully locked the user account.", "alert-success");
                    }
                   
                    _db.Customers.UpdateRange(customer);
                    await _db.SaveChangesAsync();
                    return Redirect(returnUrl); // Trả về kết quả thành công (HTTP status code 200)
                }
            }
            
            Customers = _db.Customers.ToList(); 
           
            return Page();

        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int customerId, bool status)
        {
            string returnUrl = Request.Headers["Referer"];
            Customer customer = _db.Customers.Find(customerId);
            if (customer != null)
            {
                customer.Status = status;
                _db.Customers.UpdateRange(customer);
                await _db.SaveChangesAsync();
                return Redirect(returnUrl); // Trả về kết quả thành công (HTTP status code 200)
            }
            else
            {
                return NotFound(); // Trả về kết quả không tìm thấy (HTTP status code 404) hoặc xử lý lỗi khác
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteProductsAsync(List<int> selectedProductIds)
        {
            string returnUrl = Request.Headers["Referer"];
            if (selectedProductIds == null || selectedProductIds.Count == 0)
            {
                return Redirect(returnUrl);
            }
            var productsToDelete = await _db.Customers
           .Where(p => selectedProductIds.Contains(p.CustomerId))
           .ToListAsync();

            if (productsToDelete.Count == 0)
            {
                return Redirect(returnUrl);
            }
            foreach (var product in productsToDelete)
            {
                if (product.Status == true)
                {
                    product.Status = false;
                }
                else
                {
                    product.Status = true;
                }
            }
            // Remove the products from the database
            _db.Customers.UpdateRange(productsToDelete);
            await _db.SaveChangesAsync();
            return Redirect(returnUrl);
        }

        public void MessageBox(string Notification, string AlertMessage, string AlertType)
        {
            TempData["Notification"] = Notification;
            TempData["AlertMessage"] = AlertMessage;
            TempData["AlertType"] = AlertType;
        }
    }
}