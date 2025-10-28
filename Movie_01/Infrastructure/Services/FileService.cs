using MovieApp.Core.Interfaces;

namespace MovieApp.Infrastructure.Services
{
    /// <summary>
    /// File Service Implementation
    /// SOLID: Single Responsibility - handles file operations only
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            // 1. Validate file
            if (file == null || file.Length == 0)
                throw new ArgumentException("الملف غير صالح");

            // 2. Create folder path
            string uploadsFolder = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "images",
                folder
            );

            // 3. Ensure folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 4. Generate unique filename
            string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 5. Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 6. Return relative path
            return $"/images/{folder}/{uniqueFileName}";
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder)
        {
            var uploadedPaths = new List<string>();

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var path = await UploadFileAsync(file, folder);
                    uploadedPaths.Add(path);
                }
            }

            return uploadedPaths;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            string fullPath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                filePath.TrimStart('/')
            );

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public void DeleteFiles(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                DeleteFile(filePath);
            }
        }

        public bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string fullPath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                filePath.TrimStart('/')
            );

            return File.Exists(fullPath);
        }
    }
}