using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieApp.Core.Interfaces;

namespace MovieApp.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            // Create folder if not exists
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path
            return $"uploads/{folder}/{uniqueFileName}";
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder)
        {
            var filePaths = new List<string>();

            if (files == null || !files.Any())
                return filePaths;

            foreach (var file in files)
            {
                var path = await UploadFileAsync(file, folder);
                if (!string.IsNullOrEmpty(path))
                {
                    filePaths.Add(path);
                }
            }

            return filePaths;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public void DeleteFiles(List<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            foreach (var filePath in filePaths)
            {
                DeleteFile(filePath);
            }
        }

        public bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);
            return File.Exists(fullPath);
        }
    }
}