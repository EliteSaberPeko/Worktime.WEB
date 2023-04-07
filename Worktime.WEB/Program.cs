using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Worktime.Core;
using Worktime.WEB;
using Worktime.WEB.Models;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserManager<AuthUserManager<User>>()
    .AddSignInManager<SignInManager<User>>()
    .AddErrorDescriber<AuthIdentityErrorDescriber>();

// Add services to the container.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Authentication/Login");
    options.AccessDeniedPath = new PathString("/Authentication/Login");
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
    .AddControllersWithViews();

builder.Services.AddTransient<Startup>();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
