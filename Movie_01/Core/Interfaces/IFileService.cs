/// <summary>
/// File Service Interface
/// SOLID: Single Responsibility - handles file operations only
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Upload single file
    /// </summary>
    Task<string> UploadFileAsync(IFormFile file, string folder);

    /// <summary>
    /// Upload multiple files
    /// </summary>
    Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder);

    /// <summary>
    /// Delete file
    /// </summary>
    void DeleteFile(string filePath);

    /// <summary>
    /// Delete multiple files
    /// </summary>
    void DeleteFiles(List<string> filePaths);

    /// <summary>
    /// Check if file exists
    /// </summary>
    bool FileExists(string filePath);
}

