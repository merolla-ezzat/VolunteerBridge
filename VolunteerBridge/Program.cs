using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VolunteerBridge.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Increase session timeout for a better development experience and ensure essential cookies - Coded by Yousef
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<VolunteerBridge.Services.EmailService>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// Diagnostic: log EF Core resolved connection at startup so developer can see which LocalDB/instance is used.
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<VolunteerBridge.Models.AppDbContext>();
    var conn = ctx.Database.GetDbConnection();
    System.Console.WriteLine("EF ConnectionString: " + conn.ConnectionString);
    System.Console.WriteLine("EF DataSource: " + conn.DataSource);
    System.Console.WriteLine("EF Database: " + conn.Database);
    var applied = ctx.Database.GetAppliedMigrations();
    System.Console.WriteLine("Applied migrations: " + string.Join(", ", applied));
}


// Request culture (Arabic default)
var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("ar-EG") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); 
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();