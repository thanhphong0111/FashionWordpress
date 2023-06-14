using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Group1_CourseOnline.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SWP391_BlueEduContext _db;
        public LoginModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Account Account { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("admin") != null)
            {
                return RedirectToPage("./Admin/Index");
            }
            if (HttpContext.Session.GetString("customer") != null)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Mã hóa mật khẩu sử dụng MD5
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(Account.Password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                Account.Password = sb.ToString();
            }

            var member = await _db.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Account.Email) && a.Password.Equals(Account.Password));
            if (member != null)
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                // Chuyển đổi tài khoản thành chuỗi JSON với tùy chọn ReferenceHandler.Preserve
                var serializedMember = JsonSerializer.Serialize(member, options);
                var accounts = JsonSerializer.Deserialize<Account>(serializedMember, options);
                HttpContext.Session.SetString("Account", serializedMember);
                if (member.Role == 1)
                {
                        HttpContext.Session.SetString("admin", serializedMember);
                        return RedirectToPage("./Admin/Index");
                }
                if (member.Role == 2)
                {
                var getCusById = _db.Customers.SingleOrDefault(e => e.CustomerId == member.CustomerId);
                    if (getCusById != null && getCusById.Status == true)
                    {
                    HttpContext.Session.SetString("customer", serializedMember);
                    HttpContext.Session.SetString("email", member.Email);
                    return RedirectToPage("/Index");
                    }
                    else
                    {
                        TempData["msg"] = "Your account has been locked!";
                        return Page();
                    }
                }
            }
            TempData["msg"] = "Email or password invalid!";
            return Page();
        }
    }
}
