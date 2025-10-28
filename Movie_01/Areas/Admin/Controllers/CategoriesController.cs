using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;

namespace MovieApp.Areas.Admin.Controllers
{
    // ═══════════════════════════════════════════════════════════
    // CategoriesController
    // ═══════════════════════════════════════════════════════════

    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public CategoriesController(ICategoryService categoryService, IFileService fileService)
        {
            _categoryService = categoryService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _categoryService.GetCategoryWithMoviesAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        category.ImageUrl = await _fileService.UploadFileAsync(imageFile, "categories");
                    }

                    await _categoryService.CreateCategoryAsync(category);
                    TempData["Success"] = "تم إضافة التصنيف بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"خطأ: {ex.Message}";
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? imageFile)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
                    if (existingCategory == null) return NotFound();

                    if (imageFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingCategory.ImageUrl))
                        {
                            _fileService.DeleteFile(existingCategory.ImageUrl);
                        }
                        category.ImageUrl = await _fileService.UploadFileAsync(imageFile, "categories");
                    }
                    else
                    {
                        category.ImageUrl = existingCategory.ImageUrl;
                    }

                    await _categoryService.UpdateCategoryAsync(category);
                    TempData["Success"] = "تم تحديث التصنيف بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"خطأ: {ex.Message}";
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _categoryService.GetCategoryWithMoviesAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["Success"] = "تم حذف التصنيف بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

