using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Application.Services;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Settings;
using MovieApp.Infrastructure.Data;
using MovieApp.Infrastructure.Repositories;
using MovieApp.Infrastructure.Services;
using MovieApp.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════
// 1. Add services to the container
// ═══════════════════════════════════════════════════════════════

builder.Services.AddControllersWithViews();

// ═══════════════════════════════════════════════════════════════
// 2. Database Context
// ═══════════════════════════════════════════════════════════════

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ═══════════════════════════════════════════════════════════════
// 3. Identity Configuration
// ═══════════════════════════════════════════════════════════════

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false; 
    options.SignIn.RequireConfirmedAccount = false; 
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied"; 
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.SameSite = SameSiteMode.Lax; 
});

// ═══════════════════════════════════════════════════════════════
// 4. Email Settings
// ═══════════════════════════════════════════════════════════════

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ═══════════════════════════════════════════════════════════════
// 5. Register Repositories
// ═══════════════════════════════════════════════════════════════

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ═══════════════════════════════════════════════════════════════
// 6. Register Services
// ═══════════════════════════════════════════════════════════════

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════
// 7. Seed Roles and Admin User
// ═══════════════════════════════════════════════════════════════

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ═══════════════════════════════════════════════════════════════
// 8. Configure the HTTP request pipeline
// ═══════════════════════════════════════════════════════════════

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

// ═══════════════════════════════════════════════════════════════
// 9. Configure Routes
// ═══════════════════════════════════════════════════════════════

// Area routes (Admin, Public)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route (Public Area)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Public" });

app.Run();

// ═══════════════════════════════════════════════════════════════
// 10. Seed Data Method
// ═══════════════════════════════════════════════════════════════

async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create Roles
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create Admin User
    var adminEmail = "admin@movieapp.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "مدير النظام",
            EmailConfirmed = true, 
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    
    var testUserEmail = "user@movieapp.com";
    var testUser = await userManager.FindByEmailAsync(testUserEmail);

    if (testUser == null)
    {
        testUser = new ApplicationUser
        {
            UserName = testUserEmail,
            Email = testUserEmail,
            FullName = "مستخدم تجريبي",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        var result = await userManager.CreateAsync(testUser, "User@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }
}
