using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;
using QuickBite.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Retry up to 5 times
            maxRetryDelay: TimeSpan.FromSeconds(10), // Delay between retries
            errorCodesToAdd: null) // Add specific error codes if needed
    ));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // enable support for authorization and roles (off by default)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout period for the session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make session cookies essential
});
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseMiddleware<RestaurantAreaAuthorizationMiddleware>();

app.UseRouting();

app.UseAuthorization();




app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
  );
    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


});
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedData.SeedRoles(roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedData.SeedUsers(userManager);

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

app.Run();
