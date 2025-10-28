using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Interfaces;
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
// 3. Register Repositories (SOLID: Dependency Inversion)
// ═══════════════════════════════════════════════════════════════

// Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ═══════════════════════════════════════════════════════════════
// 4. Register Services (SOLID: Dependency Inversion)
// ═══════════════════════════════════════════════════════════════

// File Service
builder.Services.AddScoped<IFileService, FileService>();

// Movie Service
builder.Services.AddScoped<IMovieService, MovieService>();

// Category Service
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Actor Service
builder.Services.AddScoped<IActorService, ActorService>();

// Cinema Service
builder.Services.AddScoped<ICinemaService, CinemaService>();

// Analytics Service
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════
// 5. Configure the HTTP request pipeline
// ═══════════════════════════════════════════════════════════════

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ═══════════════════════════════════════════════════════════════
// 6. Configure Routes (with Areas support)
// ═══════════════════════════════════════════════════════════════

// Admin Area Route
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Default Route (Public Area)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();


/*
═══════════════════════════════════════════════════════════════════
📝 شرح Dependency Injection Registration
═══════════════════════════════════════════════════════════════════

1️⃣ AddScoped:
   - يتم إنشاء instance واحد لكل HTTP Request
   - مناسب للـ DbContext والـ Repositories والـ Services
   - Example: كل request يأخذ MovieService جديد

2️⃣ AddSingleton:
   - يتم إنشاء instance واحد لكل التطبيق
   - مناسب للـ Configuration والـ Services التي لا تتغير
   - Example: IConfiguration

3️⃣ AddTransient:
   - يتم إنشاء instance جديد في كل مرة يتم طلبه
   - مناسب للـ Lightweight services
   - Example: Helper classes


═══════════════════════════════════════════════════════════════════
🎯 SOLID في DI Registration
═══════════════════════════════════════════════════════════════════

✅ Dependency Inversion Principle:
   - نسجل Interface → Implementation
   - Controllers تعتمد على IMovieService وليس MovieService

✅ Single Responsibility:
   - كل Service له مسؤولية واحدة
   - MovieService → Movie operations
   - FileService → File operations

✅ Interface Segregation:
   - Interfaces صغيرة ومتخصصة
   - IMovieService, ICategoryService (ليس IService واحد كبير)


═══════════════════════════════════════════════════════════════════
🗺️ Areas Routing
═══════════════════════════════════════════════════════════════════

Admin Area:
  URL: /Admin/Movies/Index
  Maps to: Areas/Admin/Controllers/MoviesController.Index()

Public Area (Default):
  URL: /Movies/Index
  Maps to: Controllers/MoviesController.Index()


═══════════════════════════════════════════════════════════════════
📌 Usage في Controller
═══════════════════════════════════════════════════════════════════

public class MoviesController : Controller
{
    private readonly IMovieService _movieService;

    // ASP.NET Core يحقن IMovieService تلقائياً
    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> Index()
    {
        // استخدام الـ Service
        var movies = await _movieService.GetAllMoviesAsync();
        return View(movies);
    }
}

لا حاجة لـ:
var movieService = new MovieService(); ❌

ASP.NET Core يعمل كل شيء تلقائياً! ✅
*/