using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Group1_CourseOnline.Pages
{
    public class RegisterModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public RegisterModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Account Account { get; set; }
        public Customer Customer { get; set; }

        public IActionResult OnGet()
        {
            // Check if the user is already logged in
            string? customer = HttpContext.Session.GetString("customer");
            string? admin = HttpContext.Session.GetString("admin");
            if (customer != null || admin != null)
            {
                return Redirect("/Index");
            }

            ViewData["title"] = "Register";
            return Page();
        }


        // POST: /Signup
        public async Task<IActionResult> OnPost(string? confirm)
        {
            if (Account.Email == null || Account.Password == null || Account.Customer.FirstName == null || Account.Customer.LastName == null)
            {
                TempData["msg"] = "You must enter all the fields";
                return Page();
            }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string patternPass = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

            if (!Regex.IsMatch(Account.Email, pattern))
            {
                TempData["msg"] = "Please enter the correct email format ex: abc@gmail.com";
                return Page();
            }

            if (!Regex.IsMatch(Account.Password, patternPass))
            {
                TempData["msg"] = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and have a minimum length of 8 characters.";
                return Page();
            }
            if (string.IsNullOrEmpty(confirm) || !confirm.Equals(Account.Password))
            {
                TempData["msg"] = "Password does not match";
                return Page();
            }
            var accountExists = await _db.Accounts.AnyAsync(a => a.Email == Account.Email);
            if (accountExists)
            {
                TempData["msg"] = "An account with this email already exists. Please choose another email";
                return Page();
            }

            if (string.IsNullOrEmpty(Account.Customer.Phone) || !Regex.IsMatch(Account.Customer.Phone, @"^0\d{9}$"))
            {
                TempData["msg"] = "Please enter a valid phone number starting with 0 and containing exactly 10 digits";
                return Page();
            }
            if (!IsValidPassword(Account.Password))
            {
                TempData["msg"] = "Password must contain at least one special character, one digit, one uppercase letter, and be at least 8 characters long.";
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

            var newCus = new Customer()
            {
                LastName = Account.Customer.LastName,
                FirstName = Account.Customer.FirstName,
                Phone = Account.Customer.Phone,
                BirthDate = null,
                Address = null,
                Avatar = null,
                Status = true
            };

            _db.Customers.AddAsync(newCus);
            await _db.SaveChangesAsync();

            var newAcc = new Account()
            {
                Email = Account.Email,
                Password = Account.Password,
                CustomerId = newCus.CustomerId, // Lấy giá trị CustomerId đã được tạo tự động
                Role = 2,
                CreateDate = DateTime.Now,

                IsSocialAccount = false
            };

            _db.Accounts.AddAsync(newAcc);
            await _db.SaveChangesAsync();

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
                    HttpContext.Session.SetString("customer", serializedMember);
                    HttpContext.Session.SetString("email", member.Email);
                    return RedirectToPage("/Index");
                }
            }
            return RedirectToPage("/Login");
        }

        private bool IsValidPassword(string password)
        {

            string pattern = @"^(?=.*[!@#$%^&*()\-_=+{}[\]|\\;:'""<>,./?])(?=.*\d)(?=.*[A-Z]).{8,}$";

            return Regex.IsMatch(password, pattern);
        }
    }
}