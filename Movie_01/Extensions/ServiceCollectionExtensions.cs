using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Interfaces;
using MovieApp.Infrastructure.Data;
using MovieApp.Infrastructure.Repositories;
using MovieApp.Infrastructure.Services;
using MovieApp.Infrastructure.UnitOfWork;

namespace MovieApp.Extensions
{
    /// <summary>
    /// Service Collection Extensions
    /// SOLID: Single Responsibility - centralized DI configuration
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register all application services
        /// استخدام: builder.Services.AddApplicationServices(configuration);
        /// </summary>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ═══ Database ═══
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // ═══ Repositories ═══
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ═══ Services ═══
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IActorService, ActorService>();
            services.AddScoped<ICinemaService, CinemaService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();

            return services;
        }
    }
}

/*
═══════════════════════════════════════════════════════════════════
🎯 الاستخدام في Program.cs
═══════════════════════════════════════════════════════════════════

قبل (بدون Extension):
─────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(...);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
// ... 20 سطر آخر


بعد (مع Extension):
───────────────────
builder.Services.AddApplicationServices(builder.Configuration);

سطر واحد فقط! ✨


═══════════════════════════════════════════════════════════════════
✅ الفوائد:
═══════════════════════════════════════════════════════════════════

1. Program.cs أنظف وأقصر
2. سهولة الصيانة - كل الـ DI في مكان واحد
3. إعادة الاستخدام - يمكن استخدامه في مشاريع أخرى
4. SOLID: Single Responsibility - ملف واحد مسؤول عن DI
*/