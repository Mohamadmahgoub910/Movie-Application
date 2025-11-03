using Microsoft.EntityFrameworkCore;
using MovieApp.Application.Services;
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
// 6. Configure Routes (IMPORTANT: Order matters!)
// ═══════════════════════════════════════════════════════════════

// ⚠️ الترتيب مهم جداً! Areas أولاً ثم Default

// Admin Area Route (أول حاجة)
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Public Area Route (تاني حاجة - اختياري)
app.MapControllerRoute(
    name: "public",
    pattern: "Public/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Public" }
);

// Default Route (آخر حاجة)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Public" } // ← هنا الإضافة المهمة
);

app.Run();

/*
═══════════════════════════════════════════════════════════════════
📝 شرح Routes Configuration
═══════════════════════════════════════════════════════════════════

🎯 الترتيب الصحيح:
1. Areas Route → يبحث أولاً في Areas
2. Public Area Route → للـ Public Area بشكل مباشر
3. Default Route → للصفحة الرئيسية

═══════════════════════════════════════════════════════════════════
🌐 URL Examples
═══════════════════════════════════════════════════════════════════

Admin Area:
  ✅ https://localhost:5001/Admin
  ✅ https://localhost:5001/Admin/Home/Index
  ✅ https://localhost:5001/Admin/Movies/Index
  ✅ https://localhost:5001/Admin/Movies/Create

Public Area:
  ✅ https://localhost:5001
  ✅ https://localhost:5001/Public
  ✅ https://localhost:5001/Public/Home/Index
  ✅ https://localhost:5001/Home/Index (مع default area)

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

✅ Open/Closed Principle:
   - يمكن إضافة Services جديدة بدون تعديل الموجود
   - فقط نضيف AddScoped جديد

✅ Liskov Substitution:
   - يمكن استبدال MovieService بـ MockMovieService للتجربة
   - طالما يطبق IMovieService

═══════════════════════════════════════════════════════════════════
📌 Usage في Controller
═══════════════════════════════════════════════════════════════════

// Admin Controller
namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")] // ← مهم جداً!
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
            var movies = await _movieService.GetAllMoviesAsync();
            return View(movies);
        }
    }
}

// Public Controller
namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")] // ← مهم جداً!
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _analyticsService.GetDashboardStatisticsAsync();
            return View(dashboard);
        }
    }
}


═══════════════════════════════════════════════════════════════════
🔥 Lifetime Types
═══════════════════════════════════════════════════════════════════

1️⃣ AddScoped: (الأكثر استخداماً)
   - Instance واحد لكل HTTP Request
   - مناسب للـ DbContext والـ Repositories والـ Services
   - Example: MovieService, UnitOfWork

2️⃣ AddSingleton: (للحاجات الثابتة)
   - Instance واحد لكل التطبيق
   - Example: IConfiguration, Caching Services

3️⃣ AddTransient: (للحاجات الخفيفة)
   - Instance جديد في كل مرة
   - Example: Helper classes, Validators

═══════════════════════════════════════════════════════════════════
✅ Best Practices
═══════════════════════════════════════════════════════════════════

1. استخدم AddScoped للـ Services العادية
2. استخدم AddSingleton للحاجات اللي مش بتتغير
3. استخدم AddTransient للـ Lightweight services
4. دايماً سجل Interface → Implementation
5. لا تستخدم new للـ Services في Controllers

*/