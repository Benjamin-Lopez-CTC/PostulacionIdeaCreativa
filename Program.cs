using IdeasCreativasApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connUrl = builder.Configuration.GetConnectionString("DefaultConnection");
if (connUrl != null && (connUrl.StartsWith("postgres://") || connUrl.StartsWith("postgresql://")))
{
    var uri = new Uri(connUrl);
    var userInfo = uri.UserInfo.Split(':');
    connUrl = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connUrl));

// Authentication for Profesor
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Profesor/Login";
        options.LogoutPath = "/Profesor/Logout";
        options.AccessDeniedPath = "/Profesor/Login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
