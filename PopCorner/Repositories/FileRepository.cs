using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PopCorner.Helpers;
using PopCorner.Models.Common;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using System.Security;
using static Microsoft.EntityFrameworkCore.Query.Internal.ExpressionTreeFuncletizer;
using static System.Net.Mime.MediaTypeNames;

namespace PopCorner.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        public FileRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<FileImage> UploadImage(FileImage image)
        {
            // 1. Ensure root folder exists
            var webRoot = webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");

            // 2. Sanitize and ensure target folder
            var folder = FileHelpers.SanitizeFolder(image.Folder);
            var imagesDir = string.IsNullOrEmpty(folder)
                ? webRoot
                : Path.Combine(webRoot, folder);
            Directory.CreateDirectory(imagesDir);

            // 3. Safe filename
            var safeName = Path.GetFileNameWithoutExtension(image.FileName);
            var ext = image.FileExtension.StartsWith(".") ? image.FileExtension : "." + image.FileExtension;
            var fileName = $"{safeName}{ext}";
            var localFilePath = Path.Combine(imagesDir, fileName); // ✅ fixed

            // 4. Save file
            using (var stream = new FileStream(localFilePath, FileMode.Create))
                await image.File.CopyToAsync(stream);

            // 5. Set paths
            var req = httpContextAccessor.HttpContext!.Request;
            var urlFolder = string.IsNullOrEmpty(folder) ? "" : folder.Replace('\\', '/').Trim('/') + "/";
            var fullPath = $"/{urlFolder}{fileName}";
            image.FilePath = $"{req.Scheme}://{req.Host}{fullPath}";
            image.FullPath = fullPath;

            return image;
        }

        public async Task<bool> RemoveFileByPathName(string pathName)
        {
            if (string.IsNullOrWhiteSpace(pathName))
                return false;

            var webRoot = webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");

            // Chuẩn hóa lại đường dẫn tuyệt đối
            var fullPath = Path.Combine(webRoot, pathName.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar));

            return await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        Console.WriteLine($"🗑️ Remove file successfully: {fullPath}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ File not found: {fullPath}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error while removing file {fullPath}: {ex.Message}");
                    return false;
                }
            });
        }
    }
}
