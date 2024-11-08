using Microsoft.EntityFrameworkCore;
using DACN_N3.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MovieDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache(); // Cấu hình bộ nhớ tạm thời cho session
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn của session (30 phút)
	options.Cookie.HttpOnly = true;  // Chỉ cho phép truy cập session qua HTTP, không qua JavaScript
	
	options.Cookie.IsEssential = true;  // Đảm bảo session được lưu trữ trong mọi trường hợp
	
});

builder.Services.AddAuthentication(op =>
{
	op.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	op.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	op.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
.AddCookie(op =>
{
	op.LoginPath = "/Authority/Login";
	op.LogoutPath = "/Authority/Logout";
	
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
