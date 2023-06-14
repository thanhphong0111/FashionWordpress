using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Group1_CourseOnline.Helpers;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Group1_CourseOnline.Pages
{
    public class ExternalLoginModel : PageModel
    {
        public readonly SWP391_BlueEduContext _db;
        public ExternalLoginModel(SWP391_BlueEduContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Account Account { get; set; }
        public Customer Customer { get; set; }

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            var redirectUrl = Url.Page("/ExternalLogin", pageHandler: "Callback", values: new { returnUrl = "https://localhost:7093/LoginGoogle" });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, "Google");
        }

        public async Task<IActionResult> OnGetCallbackAsync()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                // Xử lý khi xác thực không thành công
                return RedirectToPage("/Login");
            }

            var googleAccount = authenticateResult.Principal;

            var email = googleAccount.FindFirstValue(ClaimTypes.Email);
            var fullName = googleAccount.FindFirstValue(ClaimTypes.Name);
            var nameParts = fullName.Split(' ');
            var lastName = string.Join(" ", nameParts[..^1]);

            var firstName = nameParts[^1];
            string passwordGenerate = Password_encryption.GeneratePassword(8);
            var password = Password_encryption.HashPassWord(passwordGenerate);

            // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu hay chưa
            var existingUser = _db.Accounts.FirstOrDefault(a => a.Email == email);
            if (existingUser == null)
            {
                Account = new Account
                {
                    Email = email,
                    Password= password,
                    Customer = new Customer
                    {
                        FirstName = firstName,
                        LastName = lastName,    
                    }
                };
                return Page();
            }
                var member = await _db.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(existingUser.Email) && a.Password.Equals(existingUser.Password));
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
                            return RedirectToPage("/Login");
                        }
                    }
                }
                TempData["msg"] = "Email or password invalid!";
                return Page();
            }
        public async Task<IActionResult> OnPostConfirmationAsync()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                // Xử lý khi xác thực không thành công
                return RedirectToPage("/Login");
            }

            var googleAccount = authenticateResult.Principal;

            var email = googleAccount.FindFirstValue(ClaimTypes.Email);
            Account.Email= email;
            var fullName = googleAccount.FindFirstValue(ClaimTypes.Name);
            var nameParts = fullName.Split(' ');
            var lastName = string.Join(" ", nameParts[..^1]);

            var firstName = nameParts[^1];
            string passwordGenerate = Password_encryption.GeneratePassword(8);
            var password = Password_encryption.HashPassWord(passwordGenerate);

            if (string.IsNullOrEmpty(Account.Customer.FirstName) || string.IsNullOrEmpty(Account.Customer.LastName))
            {
                TempData["msg"] = "You must enter your first name and last name";
                return Page();
            }
            if (string.IsNullOrEmpty(Account.Customer.Phone) || !Regex.IsMatch(Account.Customer.Phone, @"^0\d{9}$"))
            {
                TempData["msg"] = "Please enter a valid phone number starting with 0 and containing exactly 10 digits";
                return Page();
            }

            var newCus = new Customer()
            {
                LastName = Account.Customer.LastName,
                FirstName = Account.Customer.FirstName,
                Phone = Account.Customer.Phone,
                BirthDate = Account.Customer.BirthDate,
                Address = null,
                Avatar = null,
                Status = true
            };

            _db.Customers.AddAsync(newCus);
            await _db.SaveChangesAsync();

            var newAcc = new Account()
            {
                Email = email,
                Password = password,
                CustomerId = newCus.CustomerId, // Lấy giá trị CustomerId đã được tạo tự động
                Role = 2,
                IsSocialAccount= true
            };

            _db.Accounts.AddAsync(newAcc);
            await _db.SaveChangesAsync();

            var member = await _db.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(email) && a.Password.Equals(password));
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
    }
}

