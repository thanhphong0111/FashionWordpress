using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group1_CourseOnline.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("admin");
            HttpContext.Session.Remove("customer");
            HttpContext.Session.Remove("Account");
            HttpContext.Session.Remove("Cart");

            return RedirectToPage("/Index");
        }
    }
}
