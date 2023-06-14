using Group1_CourseOnline.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// bo sung hoat dong 
builder.Services.AddRazorPages();
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMilliseconds(10));
builder.Services.AddSignalR();

//Cấu hình cho ứng dụng web lafmm việc với CSDL
builder.Services.AddDbContext<SWP391_BlueEduContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SWP391DB"));
});
builder.Services.AddSession(x =>
{
    x.IdleTimeout = TimeSpan.FromMinutes(10);
});

// Cấu hình login bằng Google Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
        options.CallbackPath = "/LoginGoogle";
    });


var app = builder.Build();

app.MapRazorPages();
app.UseAuthorization();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();

app.Run();