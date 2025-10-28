using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Infrastructure.Data;

namespace MovieApp.Infrastructure.Services
{
    // ═══════════════════════════════════════════════════════════
    // CategoryService Implementation
    // ═══════════════════════════════════════════════════════════

    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public CategoryService(IUnitOfWork unitOfWork,
                              ApplicationDbContext context,
                              IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _fileService = fileService;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Movies)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<Category?> GetCategoryWithMoviesAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Movies)
                    .ThenInclude(m => m.Cinema)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryWithMoviesAsync(id);
            if (category == null)
                throw new Exception("التصنيف غير موجود");

            if (category.Movies.Any())
                throw new Exception("لا يمكن حذف التصنيف لأنه يحتوي على أفلام");

            // Delete image
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                _fileService.DeleteFile(category.ImageUrl);
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CanDeleteCategoryAsync(int id)
        {
            return !await _unitOfWork.Movies.AnyAsync(m => m.CategoryId == id);
        }
    }