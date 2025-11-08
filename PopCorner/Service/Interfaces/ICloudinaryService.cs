using PopCorner.Models.Common;

namespace PopCorner.Service.Interfaces
{
    public interface ICloudinaryService
    {
        Task<FileImage> UploadImage(FileImage image);
        Task<bool> RemoveFileByPathName(string pathname);
        Task<Dictionary<string, string>> RemoveFilesByPathNames(List<string> pathnames);

    }
}
