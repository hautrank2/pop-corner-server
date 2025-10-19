using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PopCorner.Models.Common;
using PopCorner.Repositories.Interfaces;
using System.Security;

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
            var webRoot = webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");

            var imagesDir = Path.Combine(webRoot, "Images");

            Directory.CreateDirectory(imagesDir);

            var safeName = Path.GetFileNameWithoutExtension(image.FileName);
            var ext = image.FileExtension.StartsWith(".") ? image.FileExtension : "." + image.FileExtension;
            var fileName = $"{safeName}{ext}";

            var localFilePath = Path.Combine(imagesDir, fileName);

            using (var stream = new FileStream(localFilePath, FileMode.Create))
                await image.File.CopyToAsync(stream);

            var req = httpContextAccessor.HttpContext!.Request;
            var urlFolder = string.IsNullOrEmpty(image.Folder) ? "" : image.Folder.TrimEnd('/') + "/";
            var fullPath = $"/Images/{urlFolder}{fileName}";
            image.FilePath = $"{req.Scheme}://{req.Host}{fullPath}";
            image.FullPath = fullPath;
            return image;
        }
    }
}
