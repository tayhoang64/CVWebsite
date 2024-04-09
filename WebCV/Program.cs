using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebCV.Helpers;
using WebCV.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CvContext>(options
=> options.UseSqlServer(builder.Configuration.GetConnectionString("CVConnection")));

//builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<CvContext>();

builder.Services.AddTransient<FileService>();

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
options.SignIn.RequireConfirmedAccount = false)
.AddEntityFrameworkStores<CvContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


var app = builder.Build();
app.MapRazorPages();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = app.MapControllerRoute(
        name: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

    _ = app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

}); 


app.Run();
