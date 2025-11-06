using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PopCorner.Models.Common;
using PopCorner.Service.Interfaces;
using System.Text.Json;

namespace PopCorner.Service
{
    public class CloudinaryService : ICloudnaryService
    {
        private readonly Cloudinary cloudinary;
        public CloudinaryService(Cloudinary cloudinary) => this.cloudinary = cloudinary;

        public Task<FileImage> UploadImage(FileImage image)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(image.FileName, image.File.OpenReadStream()),
                QualityAnalysis = true,
                UseFilename = true,
                UniqueFilename = true,
                Folder = image.Folder,
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            image.FullPath = $"/{uploadResult.PublicId}";

            return Task.FromResult(image);
        }

        public async Task<bool> RemoveFileByPathName(string pathname)
        {
            if (string.IsNullOrWhiteSpace(pathname)) return false;

            // Trường hợp client gửi d1%2Fd2%2Fabc -> unescape về d1/d2/abc
            var publicId = Uri.UnescapeDataString(pathname).TrimStart('/');

            var delParams = new DelResParams
            {
                PublicIds = new List<string> { publicId },
                ResourceType = ResourceType.Image, // đổi nếu là Raw/Video
                                                   // Invalidate = true // nếu muốn xoá cache CDN
            };

            var res = await cloudinary.DeleteResourcesAsync(delParams);

            // Xác nhận xóa thành công
            var ok = res.Deleted != null
                     && res.Deleted.TryGetValue(publicId, out var status)
                     && string.Equals(status, "deleted", StringComparison.OrdinalIgnoreCase);

            // (tuỳ chọn) bạn cũng có thể kiểm tra thêm DeletedCounts nếu thích
            // var countsOk = res.DeletedCounts != null && res.DeletedCounts.ContainsKey(publicId);

            return ok;
        }

        public async Task<Dictionary<string, string>> RemoveFilesByPathNames(List<string> pathnames)
        {
            if (pathnames == null || !pathnames.Any())
                return new Dictionary<string, string>();

            // Chuẩn hóa publicId (bỏ / đầu & URL decode)
            var publicIds = pathnames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Uri.UnescapeDataString(x).TrimStart('/'))
                .ToList();

            // Xóa nhiều file 1 lần
            var delParams = new DelResParams
            {
                PublicIds = publicIds,
                ResourceType = ResourceType.Image, // hoặc Raw/Video tùy bạn upload
                                                   // Invalidate = true // nếu muốn xoá cache trên CDN luôn
            };

            var res = await cloudinary.DeleteResourcesAsync(delParams);

            // Kết quả Cloudinary trả về trong res.Deleted
            // Ví dụ: { "folder1/a" : "deleted", "folder1/b" : "not_found" }
            return res.Deleted ?? new Dictionary<string, string>();
        }

    }
}
