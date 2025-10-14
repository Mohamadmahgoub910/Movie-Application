using Microsoft.EntityFrameworkCore;

namespace MovieApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Many-to-Many relationship (Movie <-> Actor)
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure One-to-Many: Category -> Movies
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure One-to-Many: Cinema -> Movies
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Cinema)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure One-to-Many: Movie -> MovieImages
            modelBuilder.Entity<MovieImage>()
                .HasOne(mi => mi.Movie)
                .WithMany(m => m.SubImages)
                .HasForeignKey(mi => mi.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "أكشن", ImageUrl = "/images/categories/action.jpg" },
                new Category { Id = 2, Name = "كوميديا", ImageUrl = "/images/categories/comedy.jpg" },
                new Category { Id = 3, Name = "دراما", ImageUrl = "/images/categories/drama.jpg" },
                new Category { Id = 4, Name = "رعب", ImageUrl = "/images/categories/horror.jpg" },
                new Category { Id = 5, Name = "رومانسي", ImageUrl = "/images/categories/romance.jpg" }
            );

            // Seed Cinemas
            modelBuilder.Entity<Cinema>().HasData(
                new Cinema { Id = 1, Name = "سينما مصر", Logo = "/images/cinemas/misr.jpg", Address = "القاهرة" },
                new Cinema { Id = 2, Name = "جالاكسي سينما", Logo = "/images/cinemas/galaxy.jpg", Address = "الإسكندرية" },
                new Cinema { Id = 3, Name = "رينيسانس", Logo = "/images/cinemas/renaissance.jpg", Address = "الجيزة" }
            );

            // Seed Actors
            modelBuilder.Entity<Actor>().HasData(
                new Actor { Id = 1, Name = "أحمد عز", ProfilePicture = "/images/actors/ahmed-ezz.jpg" },
                new Actor { Id = 2, Name = "منى زكي", ProfilePicture = "/images/actors/mona-zaki.jpg" },
                new Actor { Id = 3, Name = "محمد رمضان", ProfilePicture = "/images/actors/mohamed-ramadan.jpg" },
                new Actor { Id = 4, Name = "ياسمين عبد العزيز", ProfilePicture = "/images/actors/yasmine.jpg" },
                new Actor { Id = 5, Name = "كريم عبد العزيز", ProfilePicture = "/images/actors/karim.jpg" }
            );

            // Seed Movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Name = "الاختيار 3",
                    Description = "عمل درامي يتناول أحداث تاريخية مهمة",
                    Price = 75.00m,
                    Status = MovieStatus.NowShowing,
                    ReleaseDateTime = new DateTime(2024, 12, 1),
                    MainImage = "/images/movies/choice3.jpg",
                    Duration = 150,
                    CategoryId = 3,
                    CinemaId = 1
                },
                new Movie
                {
                    Id = 2,
                    Name = "الحرب العالمية الثالثة",
                    Description = "فيلم كوميدي اجتماعي يناقش قضايا معاصرة",
                    Price = 80.00m,
                    Status = MovieStatus.NowShowing,
                    ReleaseDateTime = new DateTime(2024, 11, 15),
                    MainImage = "/images/movies/ww3.jpg",
                    Duration = 120,
                    CategoryId = 2,
                    CinemaId = 2
                },
                new Movie
                {
                    Id = 3,
                    Name = "الأسطورة",
                    Description = "فيلم أكشن مليء بالإثارة والتشويق",
                    Price = 85.00m,
                    Status = MovieStatus.ComingSoon,
                    ReleaseDateTime = new DateTime(2025, 1, 1),
                    MainImage = "/images/movies/legend.jpg",
                    Duration = 135,
                    CategoryId = 1,
                    CinemaId = 3
                }
            );

            // Seed MovieActors (Many-to-Many relationships)
            modelBuilder.Entity<MovieActor>().HasData(
                new MovieActor { MovieId = 1, ActorId = 1 },
                new MovieActor { MovieId = 1, ActorId = 2 },
                new MovieActor { MovieId = 2, ActorId = 3 },
                new MovieActor { MovieId = 2, ActorId = 4 },
                new MovieActor { MovieId = 3, ActorId = 5 },
                new MovieActor { MovieId = 3, ActorId = 1 }
            );
        }
    }
}